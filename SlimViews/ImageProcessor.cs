﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/ImageProcessor.cs
 * PURPOSE:     Basic Helper Methods that are used across controls
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using CommonControls;
using CommonDialogs;
using ExtendedSystemObjects;
using FileHandler;
using Imaging;
using Point = System.Drawing.Point;

namespace SlimViews
{
    /// <summary>
    ///     Provides utility methods for various tasks to reduce code duplication across controls.
    /// </summary>
    internal static class ImageProcessor
    {
        /// <summary>
        ///     Gets the render.
        /// </summary>
        /// <value>
        ///     The render.
        /// </value>
        internal static ImageRender Render { get; } = new();

        /// <summary>
        ///     Gets the generator.
        /// </summary>
        /// <value>
        ///     The generator.
        /// </value>
        private static TextureGenerator Generator { get; } = new();

        /// <summary>
        ///     Unpacks the specified folder.
        /// </summary>
        /// <param name="path">The zip file path.</param>
        /// <param name="fileNameWithoutExt">The folder name without extension.</param>
        /// <returns>The path to the target folder.</returns>
        internal static string UnpackFolder(string path, string fileNameWithoutExt)
        {
            var tempFolder = Path.Combine(Directory.GetCurrentDirectory(), ViewResources.TempFolder);
            Directory.CreateDirectory(tempFolder);

            var targetFolder = Path.Combine(tempFolder, fileNameWithoutExt);
            if (Directory.Exists(targetFolder))
                _ = FileHandleDelete.DeleteAllContents(targetFolder);
            else
                _ = Directory.CreateDirectory(targetFolder);

            _ = FileHandleCompress.OpenZip(path, targetFolder, false);
            return targetFolder;
        }

        /// <summary>
        ///     Unpacks the first image file from a given path.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <returns>The path of the first image found, or null if none found.</returns>
        internal static string UnpackFile(string path)
        {
            var files = FileHandleSearch.GetFilesByExtensionFullPath(path, ImagingResources.Appendix, true);
            return files.IsNullOrEmpty() ? null : files[0];
        }

        /// <summary>
        ///     Converts a GIF to images and saves them to the specified export path.
        /// </summary>
        /// <param name="gifPath">The path of the GIF file.</param>
        /// <param name="imageExport">The target path for converted images.</param>
        internal static async Task ConvertGifActionAsync(string gifPath, string imageExport)
        {
            var images = await Render.SplitGif(gifPath); // Call the asynchronous SplitGifAsync method

            foreach (var image in images)
                try
                {
                    var success = SaveImage(imageExport, ImagingResources.JpgExt, image);
                    if (!success) ShowError(ViewResources.ErrorCouldNotSaveFile);
                }
                catch (Exception ex) when (ex is ArgumentException or IOException or ExternalException)
                {
                    Trace.WriteLine(ex);
                    ShowError(ex.ToString(), nameof(ConvertGifAction));
                }
        }

        /// <summary>
        ///     Converts images in a folder to a GIF.
        /// </summary>
        /// <param name="folder">The source folder.</param>
        /// <param name="gifPath">The target path for the GIF.</param>
        /// <returns>The path to the newly created GIF file.</returns>
        internal static string ConvertToGifAction(string folder, string gifPath)
        {
            var targetGifPath = Path.Combine(gifPath, ViewResources.NewGif);
            Render.CreateGif(folder, targetGifPath);
            return targetGifPath;
        }

        /// <summary>
        ///     Converts a list of image paths to a GIF.
        /// </summary>
        /// <param name="images">The paths of the images.</param>
        /// <param name="filePath">The target file path for the GIF.</param>
        internal static void ConvertGifAction(List<string> images, string filePath)
        {
            Render.CreateGif(images, filePath);
        }

        /// <summary>
        ///     Sets the pixel.
        /// </summary>
        /// <param name="btm">The bitmap.</param>
        /// <param name="point">The point.</param>
        /// <param name="color">The color.</param>
        /// <param name="radius">The optional radius.</param>
        internal static Bitmap SetPixel(Bitmap btm, Point point, Color color, int radius = 1)
        {
            return Render.SetPixel(btm, point, color, radius);
        }

        /// <summary>
        ///     Gets the color.
        /// </summary>
        /// <param name="btm">The bitmap.</param>
        /// <param name="point">The point.</param>
        /// <param name="radius">The optional radius.</param>
        /// <returns>The clicked color.</returns>
        internal static ColorHsv GetPixel(Bitmap btm, Point point, int radius = 1)
        {
            var color = Render.GetPixel(btm, point, radius);

            return new ColorHsv(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        ///     Asynchronously generates an export file with specified information and an optional difference bitmap.
        /// </summary>
        internal static async Task GenerateExportAsync(string informationOne, string informationTwo, string colorOne,
            string colorTwo, string similarity, Bitmap difference)
        {
            var pathObj = FileIoHandler.HandleFileSave(ViewResources.FileOpenTxt, null!);

            if (pathObj == null) return;

            var content = new List<string>
            {
                informationOne,
                colorOne,
                informationTwo,
                colorTwo,
                similarity
            };

            content.RemoveAll(string.IsNullOrEmpty);

            try
            {
                await File.WriteAllLinesAsync(pathObj.FilePath, content).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is IOException or ArgumentException)
            {
                Trace.WriteLine(ex);
                ShowError(ex.ToString(), nameof(GenerateExportAsync));
            }

            if (difference != null)
            {
                var pngPath = Path.ChangeExtension(pathObj.FilePath, ImagingResources.PngExt);
                _ = SaveImage(pngPath, ImagingResources.PngExt, difference);
            }
        }

        /// <summary>
        ///     Resizes the specified bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to resize.</param>
        /// <param name="width">The desired width.</param>
        /// <param name="height">The desired height.</param>
        /// <returns>The resized bitmap.</returns>
        internal static Bitmap Resize(Bitmap bitmap, int width, int height)
        {
            try
            {
                return Render.BitmapScaling(bitmap, width, height);
            }
            catch (Exception ex) when (ex is ArgumentException or InsufficientMemoryException)
            {
                Trace.WriteLine(ex);
                ShowError(ex.ToString(), nameof(Resize));
            }

            return bitmap;
        }

        /// <summary>
        ///     Applies a filter to the specified bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to filter.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The filtered bitmap.</returns>
        internal static Bitmap Filter(Bitmap bitmap, ImageFilters filter)
        {
            if (filter == ImageFilters.None) return bitmap;

            try
            {
                return Render.FilterImage(bitmap, filter);
            }
            catch (Exception ex) when (ex is ArgumentException or OutOfMemoryException)
            {
                Trace.WriteLine(ex);
                ShowError(ex.ToString(), ViewResources.ErrorMessage);
            }

            return bitmap;
        }

        /// <summary>
        ///     Textures the specified bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="texture">The texture.</param>
        /// <returns>The textured bitmap.</returns>
        internal static Bitmap Texture(Bitmap bitmap, TextureType texture)
        {
            try
            {
                //just overlay the whole image with the texture, will be changed later
                var btm = Generator.GenerateTexture(bitmap.Width, bitmap.Height, texture, MaskShape.Rectangle);
                //overlay both images
                bitmap = Render.CombineBitmap(bitmap, btm, 0, 0);
            }
            catch (Exception ex) when (ex is ArgumentException or OutOfMemoryException)
            {
                Trace.WriteLine(ex);
                ShowError(ex.ToString(), ViewResources.ErrorMessage);
            }

            return bitmap;
        }

        /// <summary>
        ///     Pixelate the specified bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to pixelate.</param>
        /// <param name="pixelWidth">The width of the pixelate.</param>
        /// <returns>The pixelated bitmap.</returns>
        public static Bitmap Pixelate(Bitmap bitmap, int pixelWidth)
        {
            return Render.Pixelate(bitmap, pixelWidth);
        }

        /// <summary>
        ///     Saves the bitmap to the specified path with the given extension.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="extension">The desired file extension.</param>
        /// <param name="bitmap">The bitmap to save.</param>
        /// <returns>True if the save was successful; otherwise, false.</returns>
        internal static bool SaveImage(string path, string extension, Bitmap bitmap)
        {
            path = Path.ChangeExtension(path, extension);
            var format = extension switch
            {
                ImagingResources.PngExt => ImageFormat.Png,
                ImagingResources.JpgExt => ImageFormat.Jpeg,
                ImagingResources.BmpExt => ImageFormat.Bmp,
                ImagingResources.GifExt => ImageFormat.Gif,
                ImagingResources.TifExt => ImageFormat.Tiff,
                _ => throw new ArgumentException(ViewResources.ErrorNotSupported) // Handle unsupported formats
            };

            Render.SaveBitmap(bitmap, path, format);
            return true;
        }

        /// <summary>
        ///     Loads an image from the specified path.
        /// </summary>
        /// <param name="path">The path to the image.</param>
        /// <returns>The loaded bitmap, or null if loading fails.</returns>
        internal static Bitmap LoadImage(string path)
        {
            try
            {
                return Render.GetBitmapFile(path);
            }
            catch (IOException ex)
            {
                Trace.WriteLine(ex);
                ShowError(ex.ToString(), nameof(LoadImage));
            }

            return null;
        }

        /// <summary>
        ///     Generates a bitmap from the specified file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The generated bitmap, or null if generation fails.</returns>
        internal static Bitmap GenerateImage(string filePath)
        {
            try
            {
                return Render.GetOriginalBitmap(filePath);
            }
            catch (Exception ex) when (ex is IOException or ArgumentException or NotSupportedException
                                           or InvalidOperationException)
            {
                Trace.WriteLine(ex);
                ShowError(ex.ToString(), nameof(GenerateImage));
            }

            return null;
        }

        /// <summary>
        ///     Displays an error message in a message box.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="source">The source of the error (optional).</param>
        private static void ShowError(string message, string source = null)
        {
            if (source != null) message = $"{ViewResources.ErrorSourceMessage}{source}\n{message}";
            MessageBox.Show(message, ViewResources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        ///     Darkens the specified bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>Darken Image</returns>
        internal static Bitmap Darken(Bitmap bitmap)
        {
            return Render.AdjustBrightness(bitmap, 0.1f);
        }

        /// <summary>
        ///     ds the brighten.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>Brighten the Image</returns>
        internal static Bitmap DBrighten(Bitmap bitmap)
        {
            return Render.AdjustBrightness(bitmap, -0.1f);
        }

        /// <summary>
        ///     Exports the string into clipboard.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        internal static void ExportString(Bitmap bitmap)
        {
            var str = Render.BitmapToBase64(bitmap);
            Clipboard.SetText(str);
        }

        /// <summary>
        ///     Erases part of the image.
        /// </summary>
        /// <param name="frame">The selection frame.</param>
        /// <param name="btm">The bitmap.</param>
        /// <returns>Changed bitmap or in case of error the original</returns>
        internal static Bitmap EraseImage(SelectionFrame frame, Bitmap btm)
        {
            try
            {
                btm = Render.EraseRectangle(btm, frame.X, frame.Y, frame.Height, frame.Width);
            }
            catch (ArgumentNullException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), string.Concat(ViewResources.ErrorMessage, nameof(EraseImage)));
            }

            return btm;
        }

        public static Bitmap FillArea(Bitmap bitmap, SelectionFrame frame, Color color)
        {
            throw new NotImplementedException();
        }

        public static Bitmap FillTexture(Bitmap bitmap, SelectionFrame frame, TextureType texture)
        {
            try
            {
                //TODO generate smaller image from Frame 
                //just overlay the whole image with the texture, will be changed later
                //TODO use frame
                var btm = Generator.GenerateTexture(bitmap.Width, bitmap.Height, texture, MaskShape.Rectangle);
                //overlay both images
                bitmap = Render.CombineBitmap(bitmap, btm, 0, 0);
            }
            catch (Exception ex) when (ex is ArgumentException or OutOfMemoryException)
            {
                Trace.WriteLine(ex);
                ShowError(ex.ToString(), ViewResources.ErrorMessage);
            }

            return bitmap;
        }

        public static Bitmap FillFilter(Bitmap bitmap, SelectionFrame frame, ImageFilters filter)
        {
            if (filter == ImageFilters.None) return bitmap;

            try
            {
                //TODO generate smaller image from Frame 
                return Render.FilterImage(bitmap, filter);
            }
            catch (Exception ex) when (ex is ArgumentException or OutOfMemoryException)
            {
                Trace.WriteLine(ex);
                ShowError(ex.ToString(), ViewResources.ErrorMessage);
            }

            return bitmap;
        }

        /// <summary>
        ///     Rotates the image.
        /// </summary>
        /// <param name="btm">The bitmap.</param>
        /// <param name="degree">The degree.</param>
        /// <returns>Changed bitmap or in case of error the original</returns>
        public static Bitmap RotateImage(Bitmap btm, int degree)
        {
            try
            {
                btm = Render.RotateImage(btm, degree);
            }
            catch (Exception ex) when (ex is ArgumentException or OverflowException)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    string.Concat(ViewResources.ErrorMessage, nameof(RotateImage)));
            }

            return btm;
        }

        /// <summary>
        ///     Bitmaps the scaling.
        /// </summary>
        /// <param name="btm">The bitmap.</param>
        /// <param name="scaling">The scaling.</param>
        /// <returns>Changed bitmap or in case of error the original</returns>
        internal static Bitmap BitmapScaling(Bitmap btm, float scaling)
        {
            try
            {
                btm = Render.BitmapScaling(btm, scaling);
            }
            catch (Exception ex) when (ex is ArgumentException or OverflowException)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    string.Concat(ViewResources.ErrorMessage, nameof(RotateImage)));
            }

            return btm;
        }

        /// <summary>
        ///     Crops the image.
        /// </summary>
        /// <param name="btm">The bitmap.</param>
        /// <returns>Changed bitmap or in case of error the original</returns>
        public static Bitmap CropImage(Bitmap btm)
        {
            try
            {
                btm = Render.CropImage(btm);
            }
            catch (Exception ex) when (ex is ArgumentException or OverflowException)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    string.Concat(ViewResources.ErrorMessage, nameof(RotateImage)));
            }

            return btm;
        }

        /// <summary>
        ///     Folders the convert.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        /// <param name="observer">The observer.</param>
        internal static void FolderConvert(string target, string source, Dictionary<int, string> observer)
        {
            try
            {
                //do not sear in the folder but instead we use the _observer
                var lst = new List<string>();
                lst.AddRange(observer.Values.Where(element =>
                    Path.GetExtension(element) == source));

                var count = 0;
                var error = 0;

                foreach (var check in from image in lst
                         let btm = Render.GetOriginalBitmap(image)
                         select SaveImage(image, target, btm))
                    if (check)
                        count++;
                    else
                        error++;

                _ = MessageBox.Show(string.Concat(ViewResources.InformationConverted, count, Environment.NewLine,
                    ViewResources.InformationErrors, error));
            }
            catch (Exception ex) when (ex is ArgumentException or IOException or ExternalException)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    string.Concat(ViewResources.ErrorMessage, nameof(FolderConvert)));
            }
        }
    }
}