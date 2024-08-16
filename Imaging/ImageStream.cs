﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageStream.cs
 * PURPOSE:     Does all the leg work for the Image operations
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using ExtendedSystemObjects;
using Mathematics;

namespace Imaging
{
    /// <summary>
    ///     Loads a BitMapImage out of a specific path
    ///     Can Combine two Images and returns a new one
    /// </summary>
    public static class ImageStream
    {
        /// <summary>
        ///     Loads File one Time
        ///     Can only used to load an Image Once
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <returns>An Image as <see cref="BitmapImage" />.</returns>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="IOException">Could not find the File</exception>
        public static BitmapImage GetBitmapImage(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorMissingFile, innerException);
            }

            try
            {
                var bmp = new BitmapImage { CreateOptions = BitmapCreateOptions.DelayCreation };
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(path);
                bmp.EndInit();
                return bmp;
            }
            catch (UriFormatException ex)
            {
                throw new UriFormatException(path, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                throw new NotSupportedException(ex.Message);
            }
        }

        /// <summary>
        ///     Loads File one Time
        ///     Can only used to load an Image Once
        ///     Use this for huge amounts of image that will be resized, else we will break memory limits
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>
        ///     An Image as <see cref="BitmapImage" />.
        /// </returns>
        /// <exception cref="IOException">Could not find the File</exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        public static BitmapImage GetBitmapImage(string path, int width, int height)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorMissingFile, innerException);
            }

            try
            {
                var bmp = new BitmapImage { CreateOptions = BitmapCreateOptions.DelayCreation };
                bmp.BeginInit();
                bmp.DecodePixelHeight = height;
                bmp.DecodePixelWidth = width;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = new Uri(path);
                bmp.EndInit();
                return bmp;
            }
            catch (UriFormatException ex)
            {
                throw new UriFormatException(path, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                throw new NotSupportedException(ex.Message);
            }
        }

        /// <summary>
        ///     Loads File in a Stream
        ///     takes longer but can be changed on Runtime
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <returns>An Image as <see cref="BitmapImage" />.</returns>
        /// <exception cref="ArgumentException">No Correct Argument were provided</exception>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        /// <exception cref="IOException">Error while we try to access the File</exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="IOException">Could not find the File</exception>
        public static BitmapImage GetBitmapImageFileStream(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorMissingFile, innerException);
            }

            var bmp = new BitmapImage { CreateOptions = BitmapCreateOptions.DelayCreation };

            try
            {
                using var flStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = flStream;
                bmp.EndInit();

                return bmp;
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(e.ToString());
            }
            catch (NotSupportedException e)
            {
                throw new NotSupportedException(e.ToString());
            }
            catch (IOException e)
            {
                throw new IOException(e.ToString());
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(e.ToString());
            }
        }

        /// <summary>
        ///     Loads File in a Stream
        ///     takes longer but can be changed on Runtime
        /// </summary>
        /// <param name="path">path to the File</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>
        ///     An Image as <see cref="BitmapImage" />.
        /// </returns>
        /// <exception cref="IOException">
        /// </exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IOException">Error while we try to access the File</exception>
        /// <exception cref="ArgumentException">No Correct Argument were provided</exception>
        /// <exception cref="NotSupportedException">File Type provided was not supported</exception>
        /// <exception cref="InvalidOperationException">Could not get correct access to the Object</exception>
        /// <exception cref="IOException">Error while we try to access the File</exception>
        public static BitmapImage GetBitmapImageFileStream(string path, int width, int height)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorMissingFile, innerException);
            }

            var bmp = new BitmapImage { CreateOptions = BitmapCreateOptions.DelayCreation };

            try
            {
                using var flStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.DecodePixelHeight = height;
                bmp.DecodePixelWidth = width;
                bmp.StreamSource = flStream;
                bmp.EndInit();

                return bmp;
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(e.ToString());
            }
            catch (NotSupportedException e)
            {
                throw new NotSupportedException(e.ToString());
            }
            catch (FileFormatException e)
            {
                Trace.Write(e.ToString());
                return null;
            }
            catch (IOException e)
            {
                throw new IOException(e.ToString());
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(e.ToString());
            }
        }

        /// <summary>
        ///     Get the bitmap file.
        ///     Will  leak like crazy. Only use it if we load Icons or something.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///     The Image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="IOException">File not Found</exception>
        internal static Bitmap GetBitmapFile(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                return new Bitmap(path, true);
            }

            var innerException = path != null
                ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                : new IOException(nameof(path));
            throw new IOException(ImagingResources.ErrorMissingFile, innerException);
        }

        /// <summary>
        ///     Gets the original bitmap file.
        ///     Takes longer but produces a higher quality
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///     The Image in Original size
        /// </returns>
        /// <exception cref="IOException">
        /// </exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        internal static Bitmap GetOriginalBitmap(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                var innerException = path != null
                    ? new IOException(string.Concat(nameof(path), ImagingResources.Spacing, path))
                    : new IOException(nameof(path));
                throw new IOException(ImagingResources.ErrorMissingFile, innerException);
            }

            try
            {
                using var flStream = new FileStream(path, FileMode.Open);
                // Original picture information
                var original = new Bitmap(flStream);

                var bmp = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppPArgb);

                using var graph = Graphics.FromImage(bmp);
                graph.Clear(Color.Transparent);
                graph.CompositingMode = CompositingMode.SourceCopy;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graph.DrawImage(original, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0,
                    original.Width,
                    original.Height, GraphicsUnit.Pixel);

                return bmp;
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(e.ToString());
            }
            catch (NotSupportedException e)
            {
                throw new NotSupportedException(e.ToString());
            }
            catch (IOException e)
            {
                throw new IOException(e.ToString());
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(e.ToString());
            }
        }

        /// <summary>
        ///     Resizes an image
        /// </summary>
        /// <param name="image">The image to resize</param>
        /// <param name="width">The new width in pixels</param>
        /// <param name="height">The new height in pixels</param>
        /// <returns>
        ///     A resized version of the original image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="InsufficientMemoryException"></exception>
        public static Bitmap BitmapScaling(Bitmap image, int width, int height)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(BitmapScaling), ImagingResources.Spacing,
                        nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            var btm = new Bitmap(width, height);
            //fix Resolution
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            try
            {
                using var graph = Graphics.FromImage(btm);
                graph.CompositingMode = CompositingMode.SourceCopy;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graph.DrawImage(image, 0, 0, width, height);
                return new Bitmap(btm);
            }
            catch (InsufficientMemoryException ex)
            {
                Trace.WriteLine(ex);
                throw new InsufficientMemoryException(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                throw new ArgumentException(ex.Message);
            }
            catch (OutOfMemoryException ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        ///     Bitmaps the scaling.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="scaling">The scaling.</param>
        /// <returns>
        ///     A resized version of the original image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        internal static Bitmap BitmapScaling(Bitmap image, float scaling)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(BitmapScaling), ImagingResources.Spacing,
                        nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            var width = (int)(image.Width * scaling);
            var height = (int)(image.Height * scaling);

            //needed because of: A Graphics object cannot be created from an image that has an indexed pixel format
            var btm = new Bitmap(width, height);
            //fix Resolution
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graph = Graphics.FromImage(btm))
            {
                graph.CompositingMode = CompositingMode.SourceCopy;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var rect = new Rectangle(0, 0, width, height);

                using var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graph.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }

            return new Bitmap(btm);
        }

        /// <summary>
        ///     Combines the bitmaps overlays them and merges them into one Image
        ///     Can and will throw Exceptions as part of nameof(ToBitmapImage)
        ///     The size will be trimmed to the size of the first Image
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>
        ///     a new Bitmap with all combined Images
        /// </returns>
        /// <exception cref="ArgumentNullException">if file is null or Empty</exception>
        [return: MaybeNull]
        internal static Bitmap CombineBitmap(List<string> files)
        {
            if (files.IsNullOrEmpty())
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(CombineBitmap), ImagingResources.Spacing,
                        nameof(files)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //read all images into memory
            var images = new List<Bitmap>();

            foreach (var image in files)
            {
                //create a Bitmap from the file and add it to the list
                if (!File.Exists(image))
                {
                    Trace.WriteLine(string.Concat(ImagingResources.ErrorMissingFile, image));
                    continue;
                }

                var bitmap = new Bitmap(image);

                images.Add(bitmap);
            }

            if (images.IsNullOrEmpty())
            {
                Trace.WriteLine(ImagingResources.ErrorMissingFile);
                return null;
            }

            //get the correct size of the Final Image
            var bmp = images[0];

            //create a bitmap to hold the combined image
            var btm = new Bitmap(bmp.Width, bmp.Height);

            //get a graphics object from the image so we can draw on it
            using (var graph = Graphics.FromImage(btm))
            {
                //go through each image and draw it on the final image
                foreach (var image in images)
                {
                    graph.DrawImage(image,
                        new Rectangle(0, 0, image.Width, image.Height));
                }
            }

            foreach (var image in images)
            {
                image.Dispose();
            }

            //before return please Convert
            return btm;
        }

        /// <summary>
        ///     Combines the bitmaps.
        /// </summary>
        /// <param name="original">The original image.</param>
        /// <param name="overlay">The overlay image.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>Combined Image</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static Bitmap CombineBitmap(Bitmap original, Bitmap overlay, int x, int y)
        {
            if (original == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(CombineBitmap),
                    ImagingResources.Spacing, nameof(original)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (overlay == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(CombineBitmap),
                    ImagingResources.Spacing, nameof(overlay)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //get a graphics object from the image so we can draw on it
            using var graph = Graphics.FromImage(original);

            graph.DrawImage(overlay,
                new Rectangle(x, y, overlay.Width, overlay.Height));

            return original;
        }

        /// <summary>
        ///     Cuts a piece out of a bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>The cut Image</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static Bitmap CutBitmap(Bitmap image, int x, int y, int height, int width)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(
                        string.Concat(nameof(CutBitmaps), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (height == 0 || width == 0)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(CombineBitmap),
                    ImagingResources.Spacing, nameof(height), ImagingResources.Spacing, nameof(width)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            var btm = new Bitmap(width, height);

            using var graph = Graphics.FromImage(btm);
            graph.CompositingMode = CompositingMode.SourceCopy;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graph.SmoothingMode = SmoothingMode.HighQuality;
            graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graph.Clear(Color.White);
            var rect = new Rectangle(x, y, width, height);

            graph.DrawImage(image, 0, 0, rect, GraphicsUnit.Pixel);

            return btm;
        }

        /// <summary>
        ///     Cuts a bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>List of cut Images</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<Bitmap> CutBitmaps(Bitmap image, int x, int y, int height, int width)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(
                        string.Concat(nameof(CutBitmaps), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //read all images into memory
            var images = new List<Bitmap>(x * y);

            for (var j = 0; j < y; j++)
            for (var i = 0; i < x; i++)
            {
                var img = CutBitmap(image, i * width, j * height, height, width);
                images.Add(img);
            }

            return images;
        }

        /// <summary>
        ///     Erases the rectangle from an Image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>Original Image with the erased area</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Bitmap EraseRectangle(Bitmap image, int x, int y, int height, int width)
        {
            if (image == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(EraseRectangle),
                    ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            using var graph = Graphics.FromImage(image);

            graph.CompositingMode = CompositingMode.SourceCopy;

            using var br = new SolidBrush(Color.FromArgb(0, 255, 255, 255));

            graph.FillRectangle(br, new Rectangle(x, y, width, height));

            return image;
        }

        /// <summary>
        ///     Converts an image to gray scale
        ///     Source:
        ///     https://web.archive.org/web/20110525014754/http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
        /// </summary>
        /// <param name="image">The image to gray scale</param>
        /// <param name="filter">Image Filter</param>
        /// <returns>
        ///     A filtered version of the image
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        /// <exception cref="OutOfMemoryException"></exception>
        [return: MaybeNull]
        internal static Bitmap FilterImage(Bitmap image, ImageFilter filter)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(FilterImage), ImagingResources.Spacing,
                        nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //create a blank bitmap the same size as original
            var btm = new Bitmap(image.Width, image.Height);
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //get a graphics object from the new image
            using var graph = Graphics.FromImage(btm);
            //create some image attributes
            using var atr = new ImageAttributes();

            //set the color matrix attribute
            switch (filter)
            {
                case ImageFilter.GrayScale:
                    atr.SetColorMatrix(ImageRegister.GrayScale);
                    break;
                case ImageFilter.Invert:
                    atr.SetColorMatrix(ImageRegister.Invert);
                    break;
                case ImageFilter.Sepia:
                    atr.SetColorMatrix(ImageRegister.Sepia);
                    break;
                case ImageFilter.BlackAndWhite:
                    atr.SetColorMatrix(ImageRegister.BlackAndWhite);
                    break;
                case ImageFilter.Polaroid:
                    atr.SetColorMatrix(ImageRegister.Polaroid);
                    break;
                case ImageFilter.Contour:
                    return ApplySobel(image);
                case ImageFilter.Brightness:
                    atr.SetColorMatrix(ImageRegister.Brightness);
                    break;
                case ImageFilter.Contrast:
                    atr.SetColorMatrix(ImageRegister.Contrast);
                    break;
                case ImageFilter.HueShift:
                    atr.SetColorMatrix(ImageRegister.HueShift);
                    break;
                case ImageFilter.ColorBalance:
                    atr.SetColorMatrix(ImageRegister.ColorBalance);
                    break;
                case ImageFilter.Vintage:
                    atr.SetColorMatrix(ImageRegister.Vintage);
                    break;
                // New convolution-based filters
                case ImageFilter.Sharpen:
                    return ApplyFilter(image, ImageRegister.SharpenFilter);
                case ImageFilter.GaussianBlur:
                    return ApplyFilter(image, ImageRegister.GaussianBlur, 1.0 / 16.0);
                case ImageFilter.Emboss:
                    return ApplyFilter(image, ImageRegister.EmbossFilter);
                case ImageFilter.BoxBlur:
                    return ApplyFilter(image, ImageRegister.BoxBlur, 1.0 / 9.0);
                case ImageFilter.Laplacian:
                    return ApplyFilter(image, ImageRegister.LaplacianFilter);
                case ImageFilter.EdgeEnhance:
                    return ApplyFilter(image, ImageRegister.EdgeEnhance);
                case ImageFilter.MotionBlur:
                    return ApplyFilter(image, ImageRegister.MotionBlur, 1.0 / 5.0);
                case ImageFilter.UnsharpMask:
                    return ApplyFilter(image, ImageRegister.UnsharpMask);
                default:
                    return null;
            }

            try
            {
                //draw the original image on the new image
                //using the gray scale color matrix
                graph.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    0, 0, image.Width, image.Height, GraphicsUnit.Pixel, atr);
            }
            catch (OutOfMemoryException ex)
            {
                Trace.WriteLine(ex);
                throw;
            }

            //convert to BitmapImage
            return btm;
        }

        /// <summary>
        ///     Bitmaps the image2 bitmap.
        /// </summary>
        /// <param name="image">The bitmap image.</param>
        /// <returns>
        ///     The Image as <see cref="Bitmap" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        internal static Bitmap BitmapImageToBitmap(BitmapImage image)
        {
            if (image == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(BitmapImageToBitmap),
                    ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            using var outStream = new MemoryStream();
            var enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(image));
            enc.Save(outStream);
            var bitmap = new Bitmap(outStream);

            return new Bitmap(bitmap);
        }

        /// <summary>
        ///     Converts to bitmap image.
        ///     https://stackoverflow.com/questions/5199205/how-do-i-rotate-image-then-move-to-the-top-left-0-0-without-cutting-off-the-imag/5200280#5200280
        /// </summary>
        /// <param name="image">The bitmap.</param>
        /// The Image as
        /// <see cref="BitmapImage" />
        /// .
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        internal static BitmapImage BitmapToBitmapImage(Bitmap image)
        {
            if (image == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(BitmapToBitmapImage),
                    ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            using var memory = new MemoryStream();
            image.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        /// <summary>
        ///     Rotates the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="degree">The angle in degrees</param>
        /// <returns>
        ///     Rotated Image
        /// </returns>
        /// <exception cref="ArgumentNullException">if Image is null</exception>
        /// <exception cref="OverflowException">Degrees have a certain allowed radius</exception>
        internal static Bitmap RotateImage(Bitmap image, int degree)
        {
            //no need to do anything
            if (degree is 360 or 0)
            {
                return image;
            }

            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(RotateImage), ImagingResources.Spacing,
                        nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (degree is > 360 or < -360)
            {
                var innerException = new OverflowException(nameof(degree));
                throw new OverflowException(string.Concat(ImagingResources.ErrorWrongParameters, degree),
                    innerException);
            }

            //Calculate the size of the new Bitmap because if we rotate the Image it will be bigger
            var wOver = image.Width / 2.0f;
            var hOver = image.Height / 2.0f;

            // Get the coordinates of the corners, taking the origin to be the centre of the bitmap.
            PointF[] corners = { new(-wOver, -hOver), new(+wOver, -hOver), new(+wOver, +hOver), new(-wOver, +hOver) };

            for (var i = 0; i < 4; i++)
            {
                var point = corners[i];
                corners[i] =
                    new PointF(
                        (float)((point.X * ExtendedMath.CalcCos(degree)) - (point.Y * ExtendedMath.CalcSin(degree))),
                        (float)((point.X * ExtendedMath.CalcSin(degree)) + (point.Y * ExtendedMath.CalcCos(degree))));
            }

            // Find the min and max x and y coordinates.
            var minX = corners[0].X;
            var maxX = minX;
            var minY = corners[0].Y;
            var maxY = minY;

            for (var i = 1; i < 4; i++)
            {
                var p = corners[i];
                minX = Math.Min(minX, p.X);
                maxX = Math.Max(maxX, p.X);
                minY = Math.Min(minY, p.Y);
                maxY = Math.Max(maxY, p.Y);
            }

            // Get the size of the new bitmap.
            var newSize = new SizeF(maxX - minX, maxY - minY);
            // create it.
            var btm = new Bitmap((int)Math.Ceiling(newSize.Width), (int)Math.Ceiling(newSize.Height));
            //fix Resolution
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //  draw the old bitmap on it.
            using var graph = Graphics.FromImage(btm);
            graph.TranslateTransform(newSize.Width / 2.0f, newSize.Height / 2.0f);
            graph.RotateTransform(degree);
            graph.TranslateTransform(-image.Width / 2.0f, -image.Height / 2.0f);
            graph.DrawImage(image, 0, 0);

            return btm;
        }

        /// <summary>
        ///     Crops the image.
        ///     Non Edges are defined as transparent
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Coped Image</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static Bitmap CropImage(Bitmap image)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(CropImage), ImagingResources.Spacing,
                        nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            var first = new Point();
            var second = new Point();

            var dbm = DirectBitmap.GetInstance(image);

            // determine new left
            var left = -1;
            var right = -1;
            var top = -1;
            var bottom = -1;

            //Get the Top
            for (var x = 0; x < image.Width; x++)
            {
                for (var y = 0; y < image.Height; y++)
                {
                    var color = dbm.GetPixel(x, y);
                    if (CheckTransparent(color))
                    {
                        continue;
                    }

                    // this pixel is either not white or not fully transparent
                    top = x;
                    break;
                }

                if (top != -1)
                {
                    break;
                }
            }

            //Get the Bottom
            for (var x = image.Width - 1; x >= 0; --x)
            {
                for (var y = image.Height - 1; y >= 0; --y)
                {
                    var color = dbm.GetPixel(x, y);
                    if (CheckTransparent(color))
                    {
                        continue;
                    }

                    // this pixel is either not white or not fully transparent
                    bottom = x;
                    break;
                }

                if (bottom != -1)
                {
                    break;
                }
            }

            //Get the left
            for (var x = 0; x < image.Width; x++)
            {
                for (var y = image.Height - 1; y >= 0; --y)
                {
                    var color = dbm.GetPixel(x, y);
                    if (CheckTransparent(color))
                    {
                        continue;
                    }

                    // this pixel is either not white or not fully transparent
                    left = x;
                    break;
                }

                if (left != -1)
                {
                    break;
                }
            }

            //Get the right
            for (var x = image.Width - 1; x >= 0; --x)
            {
                for (var y = 0; y < image.Height; y++)
                {
                    var color = dbm.GetPixel(x, y);
                    if (CheckTransparent(color))
                    {
                        continue;
                    }

                    // this pixel is either not white or not fully transparent
                    right = x;
                    break;
                }

                if (right != -1)
                {
                    break;
                }
            }

            first.X = left;
            first.Y = top;

            second.X = right;
            second.Y = bottom;

            //calculate the measures
            var width = Math.Abs(second.X - first.X);
            var height = Math.Abs(second.Y - first.Y);

            // Create a new bitmap from the crop rectangle, cut out the image
            var cropRectangle = new Rectangle(first.X, first.Y, width, height);
            var btm = new Bitmap(width, height);
            //fix Resolution
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //draw the cut out onto a new image
            using (var graph = Graphics.FromImage(btm))
            {
                graph.DrawImage(image, 0, 0, cropRectangle, GraphicsUnit.Pixel);
            }

            //clear up
            dbm.Dispose();

            return btm;
        }

        /// <summary>
        ///     Saves the bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="path">The path where we want to save it.</param>
        /// <param name="format">The Image format.</param>
        /// <exception cref="ArgumentNullException">Wrong parameters</exception>
        /// <exception cref="IOException">File already exists</exception>
        /// <exception cref="ExternalException">Errors with the Path</exception>
        public static void SaveBitmap(Bitmap image, string path, ImageFormat format)
        {
            if (format == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(SaveBitmap), ImagingResources.Spacing,
                        nameof(format)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(
                        string.Concat(nameof(SaveBitmap), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (string.IsNullOrEmpty(path))
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(SaveBitmap), ImagingResources.Spacing,
                        nameof(path)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (File.Exists(path))
            {
                var count = 1;

                var fileNameOnly = Path.GetFileNameWithoutExtension(path);
                var extension = Path.GetExtension(path);
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    return;
                }

                var newPath = path;

                while (File.Exists(newPath))
                {
                    var tempFileName = $"{fileNameOnly}({count++})";
                    newPath = Path.Combine(directory!, tempFileName + extension);
                }

                SaveBitmap(image, newPath, format);
            }

            try
            {
                image.Save(path, format);
            }
            catch (ExternalException ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        ///     Converts White to Transparent.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="threshold">The threshold when the color is still white.</param>
        /// <returns>The Transparent Image</returns>
        /// <exception cref="ArgumentNullException">Wrong parameters</exception>
        internal static Bitmap ConvertWhiteToTransparent(Bitmap image, int threshold)
        {
            if (image == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(ConvertWhiteToTransparent),
                    ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //use our new Format
            var dbm = DirectBitmap.GetInstance(image);

            //255,255,255 is White
            var replacementColor = Color.FromArgb(255, 255, 255);

            for (var x = 0; x < dbm.Width; x++)
            for (var y = 0; y < dbm.Height; y++)
            {
                var color = dbm.GetPixel(x, y);

                //not in the area? continue, 255 is White
                if (255 - color.R >= threshold || 255 - color.G >= threshold || 255 - color.B >= threshold)
                {
                    continue;
                }

                //replace Value under the threshold with pure White
                dbm.SetPixel(x, y, replacementColor);
            }

            //get the Bitmap
            var btm = new Bitmap(dbm.Bitmap);
            //make Transparent
            btm.MakeTransparent(replacementColor);
            //cleanup
            dbm.Dispose();
            return btm;
        }


        /// <summary>
        ///     Pixelate the specified input image.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="stepWidth">Width of the step.</param>
        /// <returns>Pixelated Image</returns>
        internal static Bitmap Pixelate(Bitmap image, int stepWidth)
        {
            if (image == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(ConvertWhiteToTransparent),
                    ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            // Create a new bitmap to store the processed image
            var dbm = new DirectBitmap(image);
            // Create a new bitmap to store the processed image
            var processedImage = new Bitmap(dbm.Width, dbm.Height);


            // Iterate over the image with the specified step width
            for (var y = 0; y < dbm.Height; y += stepWidth)
            for (var x = 0; x < dbm.Width; x += stepWidth)
            {
                // Get the color of the current rectangle
                var averageColor = GetAverageColor(image, x, y, stepWidth, stepWidth);

                using var g = Graphics.FromImage(processedImage);
                using var brush = new SolidBrush(averageColor);
                g.FillRectangle(brush, x, y, stepWidth, stepWidth);
            }

            return processedImage;
        }

        /// <summary>
        ///     Gets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The Color at the point
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Image was null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Point was out of bound.</exception>
        internal static Color GetPixel(Bitmap image, Point point)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(GetPixel), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            if (point.X < 0 || point.X >= image.Width || point.Y < 0 || point.Y >= image.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(point), ImagingResources.ErrorOutOfBounds);
            }

            //use our new Format
            var dbm = DirectBitmap.GetInstance(image);
            return dbm.GetPixel(point.X, point.Y);
        }

        /// <summary>
        ///     Gets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>
        ///     The Color at the Point
        /// </returns>
        /// <exception cref="System.ArgumentNullException">image was null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     radius or point is out of bounds.
        /// </exception>
        internal static Color GetPixel(Bitmap image, Point point, int radius)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), ImagingResources.ErrorWrongParameters);
            }

            if (radius < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), ImagingResources.ErrorRadius);
            }

            if (point.X < 0 || point.X >= image.Width || point.Y < 0 || point.Y >= image.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(point), ImagingResources.ErrorOutOfBounds);
            }

            var points = GetCirclePoints(point, radius, image.Height, image.Width);

            if (points.Count == 0)
            {
                return GetPixel(image, point);
            }

            int redSum = 0, greenSum = 0, blueSum = 0;

            foreach (var color in points.Select(pointSingle => GetPixel(image, pointSingle)))
            {
                redSum += color.R;
                greenSum += color.G;
                blueSum += color.B;
            }

            return Color.FromArgb(redSum / points.Count, greenSum / points.Count, blueSum / points.Count);
        }

        /// <summary>
        ///     Sets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="color">The color.</param>
        /// <returns>
        ///     The changed image as Bitmap
        /// </returns>
        /// <exception cref="ArgumentNullException">nameof(image)</exception>
        internal static Bitmap SetPixel(Bitmap image, Point point, Color color)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(SetPixel), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            //use our new Format
            var dbm = DirectBitmap.GetInstance(image);
            dbm.SetPixel(point.X, point.Y, color);

            return dbm.Bitmap;
        }

        /// <summary>
        ///     Sets the pixel.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="color">The color.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>The Changed Image</returns>
        /// <exception cref="ArgumentNullException">nameof(image)</exception>
        internal static Bitmap SetPixel(Bitmap image, Point point, Color color, int radius)
        {
            if (image == null)
            {
                var innerException =
                    new ArgumentNullException(string.Concat(nameof(SetPixel), ImagingResources.Spacing, nameof(image)));
                throw new ArgumentNullException(ImagingResources.ErrorWrongParameters, innerException);
            }

            var points = GetCirclePoints(point, radius, image.Height, image.Width);

            return points.Aggregate(image, (current, pointSingle) => SetPixel(current, pointSingle, color));
        }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="sourceBitmap">The source bitmap.</param>
        /// <param name="filterMatrix">
        ///     The filter matrix.
        ///     Matrix Definition: The convolution matrix is typically a 2D array of numbers (weights) that defines how each pixel in the image should be altered based on its neighboring pixels. Common sizes are 3x3, 5x5, or 7x7.
        ///     Placement: Place the center of the convolution matrix on the target pixel in the image.
        ///     Neighborhood Calculation: Multiply the value of each pixel in the neighborhood by the corresponding value in the convolution matrix.
        ///     Summation: Sum all these products.
        ///     Normalization: Often, the result is normalized (e.g., dividing by the sum of the matrix values) to ensure that pixel values remain within a valid range.
        ///     Pixel Update: The resulting value is assigned to the target pixel in the output image.
        ///     Matrix Size: The size of the matrix affects the area of the image that influences each output pixel. For example:
        ///     3x3 Matrix: Considers the pixel itself and its immediate 8 neighbors.
        ///     5x5 Matrix: Considers a larger area, including 24 neighbors and the pixel itself.
        /// </param>
        /// <param name="factor">The factor.</param>
        /// <param name="bias">The bias.</param>
        /// <returns>Image with applied filter</returns>
        private static Bitmap ApplyFilter(Image sourceBitmap, double[,] filterMatrix, double factor = 1.0,
            double bias = 0.0)
        {
            // Use DirectBitmap for easier and faster pixel manipulation
            using var source = new DirectBitmap(sourceBitmap);
            using var result = new DirectBitmap(source.Width, source.Height);

            var filterWidth = filterMatrix.GetLength(1);
            var filterHeight = filterMatrix.GetLength(0);
            var filterOffset = filterWidth / 2;

            for (var y = filterOffset; y < source.Height - filterOffset; y++)
            {
                for (var x = filterOffset; x < source.Width - filterOffset; x++)
                {
                    double blue = 0.0, green = 0.0, red = 0.0;

                    for (var filterY = 0; filterY < filterHeight; filterY++)
                    {
                        for (var filterX = 0; filterX < filterWidth; filterX++)
                        {
                            var imageX = x + (filterX - filterOffset);
                            var imageY = y + (filterY - filterOffset);

                            var pixelColor = source.GetPixel(imageX, imageY);

                            blue += pixelColor.B * filterMatrix[filterY, filterX];
                            green += pixelColor.G * filterMatrix[filterY, filterX];
                            red += pixelColor.R * filterMatrix[filterY, filterX];
                        }
                    }

                    var newBlue = Math.Min(Math.Max((int)((factor * blue) + bias), 0), 255);
                    var newGreen = Math.Min(Math.Max((int)((factor * green) + bias), 0), 255);
                    var newRed = Math.Min(Math.Max((int)((factor * red) + bias), 0), 255);

                    result.SetPixel(x, y, Color.FromArgb(newRed, newGreen, newBlue));
                }
            }

            return result.Bitmap;
        }


        /// <summary>
        ///     Gets the average color.
        /// </summary>
        /// <param name="inputImage">The bitmap.</param>
        /// <param name="startX">The start x.</param>
        /// <param name="startY">The start y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Average Color of the Area</returns>
        private static Color GetAverageColor(Image inputImage, int startX, int startY, int width, int height)
        {
            var totalRed = 0;
            var totalGreen = 0;
            var totalBlue = 0;
            var pixelCount = 0;

            // Create a new bitmap to store the processed image
            var dbm = new DirectBitmap(inputImage);

            // Iterate over the specified rectangle in the image
            for (var y = startY; y < startY + height && y < dbm.Height; y++)
            for (var x = startX; x < startX + width && x < dbm.Width; x++)
            {
                // Get the color of the current pixel
                var pixelColor = dbm.GetPixel(x, y);

                // Accumulate the color components
                totalRed += pixelColor.R;
                totalGreen += pixelColor.G;
                totalBlue += pixelColor.B;
                pixelCount++;
            }

            //cleanup
            dbm.Dispose();

            // Calculate the average color components
            var averageRed = totalRed / pixelCount;
            var averageGreen = totalGreen / pixelCount;
            var averageBlue = totalBlue / pixelCount;

            // Return the average color
            return Color.FromArgb(averageRed, averageGreen, averageBlue);
        }

        // TODO add:
        // Prewitt
        // Roberts Cross
        // Laplacian
        // Laplacian of Gaussain

        /// <summary>
        ///     Applies the Sobel.
        /// </summary>
        /// <param name="originalImage">The original image.</param>
        /// <returns>Contour of an Image</returns>
        private static Bitmap ApplySobel(Bitmap originalImage)
        {
            // Convert the original image to greyscale
            var greyscaleImage = FilterImage(originalImage, ImageFilter.GrayScale);

            // Create a new bitmap to store the result of Sobel operator
            var resultImage = new Bitmap(greyscaleImage.Width, greyscaleImage.Height);

            // Sobel masks for gradient calculation
            int[,] sobelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] sobelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            var dbmBase = new DirectBitmap(greyscaleImage);
            var dbmResult = new DirectBitmap(resultImage);

            // Apply Sobel operator to each pixel in the image
            for (var x = 1; x < greyscaleImage.Width - 1; x++)
            for (var y = 1; y < greyscaleImage.Height - 1; y++)
            {
                var gx = 0;
                var gy = 0;

                // Convolve the image with the Sobel masks
                for (var i = -1; i <= 1; i++)
                for (var j = -1; j <= 1; j++)
                {
                    var pixel = dbmBase.GetPixel(x + i, y + j);
                    int grayValue = pixel.R; // Since it's a greyscale image, R=G=B
                    gx += sobelX[i + 1, j + 1] * grayValue;
                    gy += sobelY[i + 1, j + 1] * grayValue;
                }

                // Calculate gradient magnitude
                var magnitude = (int)Math.Sqrt((gx * gx) + (gy * gy));

                // Normalize the magnitude to fit within the range of 0-255
                magnitude = (int)(magnitude / Math.Sqrt(2)); // Divide by sqrt(2) for normalization
                magnitude = Math.Min(255, Math.Max(0, magnitude));

                // Set the result pixel color
                dbmResult.SetPixel(x, y, Color.FromArgb(magnitude, magnitude, magnitude));
            }

            dbmBase.Dispose();

            return dbmResult.Bitmap;
        }

        /// <summary>
        ///     Checks if the Color is  transparent.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>True if conditions are met</returns>
        private static bool CheckTransparent(Color color)
        {
            //0,0,0 is Black or Transparent
            return color.R == 0 && color.G == 0 && color.B == 0;
        }

        /// <summary>
        ///     Gets all points in a Circle.
        ///     Uses the  Bresenham's circle drawing algorithm.
        ///     https://en.wikipedia.org/wiki/Midpoint_circle_algorithm
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="length">The length.</param>
        /// <param name="width">The height.</param>
        /// <returns>List of Points</returns>
        private static List<Point> GetCirclePoints(Point center, int radius, int length, int width)
        {
            var points = new List<Point>();

            for (var x = Math.Max(0, center.X - radius); x <= Math.Min(width - 1, center.X + radius); x++)
            {
                var dx = x - center.X;
                var height = (int)Math.Sqrt((radius * radius) - (dx * dx));

                for (var y = Math.Max(0, center.Y - height); y <= Math.Min(length - 1, center.Y + height); y++)
                {
                    points.Add(new Point(x, y));
                }
            }

            return points;
        }
    }
}
