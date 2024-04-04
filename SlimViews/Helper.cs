/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/Helper.cs
 * PURPOSE:     Basic Helper Methods that are used across controls
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using CommonControls;
using ExtendedSystemObjects;
using FileHandler;
using Imaging;

namespace SlimViews
{
    /// <summary>
    ///     Handle some repeating tasks or help to reduce the size of some classes
    /// </summary>
    internal static class Helper
    {
        /// <summary>
        ///     The render
        /// </summary>
        internal static readonly ImageRender Render = new();

        /// <summary>
        ///     Unpacks the folder.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileNameWithoutExt">The file name without extension.</param>
        /// <returns>Target Folder</returns>
        internal static string UnpackFolder(string path, string fileNameWithoutExt)
        {
            //create Temp Folder
            var root = Path.Combine(Directory.GetCurrentDirectory(), SlimViewerResources.TempFolder);
            if (!Directory.Exists(root)) _ = Directory.CreateDirectory(root);

            root = Path.Combine(root, fileNameWithoutExt);
            if (!Directory.Exists(root))
            {
                //if the folder exists which should not happen, we clear it out
                _ = FileHandleDelete.DeleteAllContents(root, true);
                _ = Directory.CreateDirectory(root);
            }

            _ = FileHandleCompress.OpenZip(path, root, false);

            return root;
        }

        /// <summary>
        ///     Unpacks the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The first Image</returns>
        internal static string UnpackFile(string path)
        {
            var lst = FileHandleSearch.GetFilesByExtensionFullPath(path, ImagingResources.Appendix, true);
            if (lst.IsNullOrEmpty()) return null;

            // ReSharper disable once PossibleNullReferenceException. is checked
            return lst.IsNullOrEmpty() ? null : lst[0];
        }

        /// <summary>
        ///     Converts the gif to images action.
        /// </summary>
        /// <param name="gifPath">path of the gif.</param>
        /// <param name="imageExport">target path for the converted Image</param>
        internal static void ConvertGifAction(string gifPath, string imageExport)
        {
            foreach (var image in Render.SplitGif(gifPath))
                try
                {
                    var check = SaveImage(imageExport, ImagingResources.JpgExt, image);
                    if (!check) _ = MessageBox.Show(SlimViewerResources.ErrorCouldNotSaveFile);
                }
                catch (ArgumentException ex)
                {
                    Trace.WriteLine(ex);
                    _ = MessageBox.Show(ex.ToString(), string.Concat(SlimViewerResources.MessageError, nameof(ConvertGifAction)));
                }
                catch (IOException ex)
                {
                    Trace.WriteLine(ex);
                    _ = MessageBox.Show(ex.ToString(), string.Concat(SlimViewerResources.MessageError, nameof(ConvertGifAction)));
                }
                catch (ExternalException ex)
                {
                    Trace.WriteLine(ex);
                    _ = MessageBox.Show(ex.ToString(), string.Concat(SlimViewerResources.MessageError, nameof(ConvertGifAction)));
                }
        }

        /// <summary>
        ///     Converts to GIF action.
        /// </summary>
        /// <param name="folder">Source Folder.</param>
        /// <param name="gifPath">Target Folder.</param>
        /// <returns>Path to new gif file</returns>
        internal static string ConvertToGifAction(string folder, string gifPath)
        {
            var target = Path.Combine(gifPath, SlimViewerResources.NewGif);
            Render.CreateGif(folder, target);

            return target;
        }

        /// <summary>
        ///     Converts the GIF action.
        /// </summary>
        /// <param name="images">The Paths to the images.</param>
        /// <param name="filePath">The file path.</param>
        internal static void ConvertGifAction(List<string> images, string filePath)
        {
            Render.CreateGif(images, filePath);
        }

        /// <summary>
        ///     Generates the export asynchronous.
        /// </summary>
        /// <param name="informationOne">The information one.</param>
        /// <param name="informationTwo">The information two.</param>
        /// <param name="colorOne">The color one.</param>
        /// <param name="colorTwo">The color two.</param>
        /// <param name="similarity">The similarity.</param>
        /// <param name="difference">The difference.</param>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal static async Task GenerateExportAsync(string informationOne, string informationTwo, string colorOne,
            string colorTwo, string similarity, Bitmap difference)
        {
            var pathObj = FileIoHandler.HandleFileSave(SlimViewerResources.FileOpenTxt, null);

            var content = new List<string>();

            if (!string.IsNullOrEmpty(informationOne)) content.Add(informationOne);
            if (!string.IsNullOrEmpty(informationOne)) content.Add(colorOne);
            if (!string.IsNullOrEmpty(informationTwo)) content.Add(informationTwo);
            if (!string.IsNullOrEmpty(informationTwo)) content.Add(colorTwo);
            if (!string.IsNullOrEmpty(similarity)) content.Add(similarity);

            try
            {
                await File.WriteAllLinesAsync(pathObj.FilePath, content).ConfigureAwait(false);
            }
            catch (IOException ex)
            {
                Trace.WriteLine(ex.ToString());
                _ = MessageBox.Show(ex.ToString(), string.Concat(SlimViewerResources.MessageError, nameof(GenerateExportAsync)));
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex.ToString());
                _ = MessageBox.Show(ex.ToString(), string.Concat(SlimViewerResources.MessageError, nameof(GenerateExportAsync)));
            }

            if (difference == null) return;

            var path = Path.ChangeExtension(pathObj.FilePath, ImagingResources.PngExt);
            _ = SaveImage(path, ImagingResources.PngExt, difference);
        }

        /// <summary>
        ///     Resizes the specified BTM.
        /// </summary>
        /// <param name="btm">The BTM.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>Resized Image</returns>
        internal static Bitmap Resize(Bitmap btm, int width, int height)
        {
            try
            {
                btm = Render.BitmapScaling(btm, width, height);
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), string.Concat(SlimViewerResources.MessageError, nameof(Resize)));
            }
            catch (InsufficientMemoryException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), string.Concat(SlimViewerResources.MessageError, nameof(Resize)));
            }

            return btm;
        }

        /// <summary>
        ///     Filters the specified BTM.
        /// </summary>
        /// <param name="btm">The BTM.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The Image with an applied filter</returns>
        internal static Bitmap Filter(Bitmap btm, ImageFilter filter)
        {
            try
            {
                switch (filter)
                {
                    case ImageFilter.None:
                        return btm;
                    case ImageFilter.Invert:
                    case ImageFilter.Sepia:
                    case ImageFilter.BlackAndWhite:
                    case ImageFilter.Polaroid:
                    case ImageFilter.GrayScale:
                        return Render.FilterImage(btm, filter);
                    default:
                        return btm;
                }
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (OutOfMemoryException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }

            return btm;
        }

        /// <summary>
        ///     Saves the image.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="btm">The BTM.</param>
        /// <returns>success Status</returns>
        internal static bool SaveImage(string path, string extension, Bitmap btm)
        {
            if (Path.GetExtension(path) != extension) path = Path.ChangeExtension(path, extension);

            switch (extension)
            {
                case ImagingResources.PngExt:
                    Render.SaveBitmap(btm, path, ImageFormat.Png);
                    break;
                case ImagingResources.JpgExt:
                    Render.SaveBitmap(btm, path, ImageFormat.Jpeg);
                    break;
                case ImagingResources.BmpExt:
                    Render.SaveBitmap(btm, path, ImageFormat.Bmp);
                    break;
                case ImagingResources.GifExt:
                    Render.SaveBitmap(btm, path, ImageFormat.Gif);
                    break;
                case ImagingResources.TifExt:
                    Render.SaveBitmap(btm, path, ImageFormat.Tiff);
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Loads the images.
        /// </summary>
        /// <param name="path">The path to the image.</param>
        /// <returns>List of Bitmap Images</returns>
        internal static Bitmap LoadImage(string path)
        {
            try
            {
                return Render.GetBitmapFile(path);
            }
            catch (IOException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), string.Concat(SlimViewerResources.MessageError, nameof(LoadImage)));
            }

            return null;
        }
    }
}