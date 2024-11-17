﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageGifHandler.cs
 * PURPOSE:     Some processing stuff for Gif Images, not perfect, the files are slightly bigger though.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://stackoverflow.com/questions/18719302/net-creating-a-looping-gif-using-gifbitmapencoder
 *              https://debugandrelease.blogspot.com/2018/12/creating-gifs-in-c.html
 *              http://www.matthewflickinger.com/lab/whatsinagif/bits_and_bytes.asp
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ExtendedSystemObjects;
using FileHandler;
using Size = System.Drawing.Size;

namespace Imaging
{
    public static class ImageGifHandler
    {
        /// <summary>
        ///     Gets the image information.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>gif Infos</returns>
        public static ImageGifInfo GetImageInfo(string path)
        {
            ImageGifInfo info = null;

            try
            {
                info = ImageGifMetadataExtractor.ExtractGifMetadata(path);
            }
            catch (FileNotFoundException ex)
            {
                Trace.WriteLine(ex.Message);
                //TODo fill up
            }


            return info;
        }

        /// <summary>
        ///     Splits the GIF.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List of Images from gif</returns>
        internal static async Task<List<Bitmap>> SplitGifAsync(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorMissingFile, innerException);
            }

            var lst = new List<Bitmap>();

            try
            {
                using var image = Image.FromFile(path);

                var numberOfFrames = image.GetFrameCount(FrameDimension.Time);

                for (var i = 0; i < numberOfFrames; i++)
                {
                    image.SelectActiveFrame(FrameDimension.Time, i);

                    // Process each frame asynchronously
                    await Task.Run(() =>
                    {
                        var bmp = new Bitmap(image);
                        lst.Add(bmp);
                    });
                }
            }
            catch (OutOfMemoryException ex)
            {
                var currentProcess = Process.GetCurrentProcess();
                var memorySize = currentProcess.PrivateMemorySize64;

                Trace.WriteLine(string.Concat(ex, ImagingResources.Separator, ImagingResources.ErrorMemory,
                    memorySize));
                lst.Clear();
                GC.Collect();

                ImageRegister.Count++;

                if (ImageRegister.Count < 3)
                {
                    await SplitGifAsync(path);
                }
                else
                {
                    ImageRegister.Count = 0;
                    throw;
                }
            }

            return lst;
        }


        /// <summary>
        ///     Loads the GIF.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List of Images from gif as ImageSource</returns>
        internal static async Task<List<ImageSource>> LoadGif(string path)
        {
            // Await the result from the async SplitGifAsync method
            var bitmapList = await SplitGifAsync(path);

            // Convert each Bitmap to ImageSource and return the list
            return bitmapList.Select(image => image.ToBitmapImage()).Cast<ImageSource>().ToList();
        }


        /// <summary>
        ///     Creates the gif.
        ///     The gif is slightly bigger for now
        ///     Sources:
        ///     https://stackoverflow.com/questions/18719302/net-creating-a-looping-gif-using-gifbitmapencoder
        /// </summary>
        /// <param name="path">The path to the folder.</param>
        /// <param name="target">The target path.</param>
        internal static void CreateGif(string path, string target)
        {
            //get all allowed files from target folder
            var lst = FileHandleSearch.GetFilesByExtensionFullPath(path, ImagingResources.Appendix, false);

            lst = lst.PathSort();

            //collect and convert all images
            var btm = lst.ConvertAll(ImageStream.GetOriginalBitmap);

            if (btm.IsNullOrEmpty()) return;

            CreateGif(btm, target);
        }

        /// <summary>
        ///     Creates the GIF.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="target">The target.</param>
        internal static void CreateGif(List<string> path, string target)
        {
            //collect and convert all images
            var btm = path.ConvertAll(ImageStream.GetOriginalBitmap);

            if (btm.IsNullOrEmpty()) return;

            CreateGif(btm, target);
        }

        /// <summary>
        ///     Create the gif.
        /// </summary>
        /// <param name="btm">A list of Bitmaps.</param>
        /// <param name="target">The target.</param>
        private static void CreateGif(IEnumerable<Bitmap> btm, string target)
        {
            var gEnc = new GifBitmapEncoder();

            //TODO encode and change to one size, add more sanity checks
            //TODO possible Thumbnail
            //TODO add more encoding

            foreach (var src in btm.Select(bmpImage => bmpImage.GetHbitmap()).Select(bmp =>
                         System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                             bmp,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions())))
                gEnc.Frames.Add(BitmapFrame.Create(src));

            using var ms = new MemoryStream();
            gEnc.Save(ms);
            var fileBytes = ms.ToArray();
            // write custom header
            // This is the NETSCAPE2.0 Application Extension.
            var applicationExtension =
                new byte[] { 33, 255, 11, 78, 69, 84, 83, 67, 65, 80, 69, 50, 46, 48, 3, 1, 0, 0, 0 };
            var newBytes = new List<byte>();
            newBytes.AddRange(fileBytes.Take(13));
            newBytes.AddRange(applicationExtension);
            newBytes.AddRange(fileBytes.Skip(13));
            File.WriteAllBytes(target, newBytes.ToArray());
        }
    }
}