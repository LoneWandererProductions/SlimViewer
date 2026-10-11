/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        ImageProcessor.cs
 * PURPOSE:     Basic Helper Methods that are used across controls
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable WrongIndentSize

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
using Common.Dialogs;
using Common.Images;
using Extended.Extensions;
using FileHandler;
using Imaging;
using Imaging.Enums;
using SlimControls;
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
        internal static readonly ImageRender Render = new();

        /// <summary>
        ///     Gets the generator.
        /// </summary>
        /// <value>
        ///     The generator.
        /// </value>
        private static readonly TextureGenerator Generator = new();

        /// <summary>
        ///     Unpacks the specified folder.
        /// </summary>
        /// <param name="path">The zip file path.</param>
        /// <param name="fileNameWithoutExt">The folder name without extension.</param>
        /// <returns>The path to the target folder.</returns>
        internal static string? UnpackFolder(string? path, string? fileNameWithoutExt)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(fileNameWithoutExt))
                return null;

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
        internal static string? UnpackFile(string? path)
        {
            var files = FileHandleSearch.GetFilesByExtensionFullPath(path, ImagingResources.Appendix, true);
            return files.IsNullOrEmpty() ? null : files[0];
        }

        /// <summary>
        /// Generates a texture atlas from all images in a specified folder (capped at 200 files), 
        /// handles varying image sizes, and saves it automatically inside the source folder.
        /// </summary>
        /// <param name="folderPath">The folder containing the source images.</param>
        /// <returns>True if the texture atlas was successfully generated and saved; otherwise, false.</returns>
        internal static bool GenerateTextureAtlas(string? folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                return false;

            // 1. Gather all image file paths using FileHandleSearch[cite: 1]
            var files = FileHandleSearch.GetFilesByExtensionFullPath(folderPath, ImagingResources.Appendix, false);
            if (files.IsNullOrEmpty())
            {
                Trace.WriteLine("No images found in the specified folder for texture atlas generation.");
                return false;
            }

            if (files.Count > 300)
            {
                files = files.Take(200).ToList();
            }

            var bitmaps = new List<Bitmap>();
            const int maxTileSize = 512; // Safety threshold to prevent GDI+ canvas allocation crashes

            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        var bmp = new Bitmap(file);
                        // Validate dimensions immediately to prevent GDI+ ArgumentExceptions
                        if (bmp.Width > 0 && bmp.Height > 0)
                        {
                            // If the image is too large, scale it down to protect the atlas canvas size
                            if (bmp.Width > maxTileSize || bmp.Height > maxTileSize)
                            {
                                var scaledBmp = Resize(bmp, maxTileSize, maxTileSize);
                                bmp.Dispose(); // Dispose original large bitmap

                                if (scaledBmp is { Width: > 0, Height: > 0 })
                                {
                                    bitmaps.Add(scaledBmp);
                                }
                            }
                            else
                            {
                                bitmaps.Add(bmp);
                            }
                        }
                        else
                        {
                            bmp.Dispose();
                        }
                    }
                    catch (Exception ex) when (ex is ArgumentException or IOException)
                    {
                        Trace.WriteLine(ex);
                    }
                }
            }

            // Safeguard cleanup if any invalid ones slipped through
            bitmaps.RemoveAll(b => b.Width <= 0 || b.Height <= 0);

            if (bitmaps.Count == 0)
                return false;

            try
            {
                // 2. Account for different sizes by finding maximum dimensions
                var cellWidth = bitmaps.Max(b => b.Width);
                var cellHeight = bitmaps.Max(b => b.Height);

                if (cellWidth <= 0 || cellHeight <= 0)
                    return false;

                // Define a grid layout (maximum of 4 columns per row)
                var columns = Math.Min(bitmaps.Count, 4);
                var rows = (int)Math.Ceiling((double)bitmaps.Count / columns);

                var atlasWidth = columns * cellWidth;
                var atlasHeight = rows * cellHeight;

                // 3. Create the master atlas canvas safely
                using var atlasBitmap = new Bitmap(atlasWidth, atlasHeight);
                using (var graph = Graphics.FromImage(atlasBitmap))
                {
                    // Clear background with transparency
                    graph.Clear(Color.Transparent);

                    // High-quality rendering settings
                    graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    for (var i = 0; i < bitmaps.Count; i++)
                    {
                        var col = i % columns;
                        var row = i / columns;

                        // Center smaller images neatly within their grid cell slot
                        var x = (col * cellWidth) + (cellWidth - bitmaps[i].Width) / 2;
                        var y = (row * cellHeight) + (cellHeight - bitmaps[i].Height) / 2;

                        // Draw the individual image safely onto the atlas canvas
                        graph.DrawImage(bitmaps[i], x, y, bitmaps[i].Width, bitmaps[i].Height);
                    }
                }

                // 4. Automatically build output path and save using SaveImage[cite: 3]
                var outputPath = Path.Combine(folderPath, "TextureAtlas");
                return SaveImage(outputPath, ImagingResources.PngExt, atlasBitmap);
            }
            catch (Exception ex) when (ex is ArgumentException or OutOfMemoryException or ExternalException)
            {
                Trace.WriteLine(ex);
                ShowError(ex.ToString(), nameof(GenerateTextureAtlas));
                return false;
            }
            finally
            {
                // 5. Clean up individual source bitmaps from memory
                foreach (var bmp in bitmaps)
                {
                    bmp.Dispose();
                }
            }
        }

        /// <summary>
        ///     Draws a batch of pencil/eraser points in place, connecting them with the brush.
        /// </summary>
        /// <param name="btm">The bitmap (modified in place).</param>
        /// <param name="points">The points of this batch.</param>
        /// <param name="previous">Last point of the previous batch of the same stroke, or null at stroke start.</param>
        /// <param name="color">The colour (Transparent erases).</param>
        /// <param name="radius">The brush radius.</param>
        internal static Bitmap? DrawStroke(Bitmap? btm, IReadOnlyList<Point> points, Point? previous, Color color,
            int radius)
        {
            return btm == null ? null : Render.DrawStroke(btm, points, previous, color, radius);
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
        /// Asynchronously generates an export file with specified information and an optional difference bitmap.
        /// </summary>
        /// <param name="informationOne">The information one.</param>
        /// <param name="informationTwo">The information two.</param>
        /// <param name="colorOne">The color one.</param>
        /// <param name="colorTwo">The color two.</param>
        /// <param name="similarity">The similarity.</param>
        /// <param name="difference">The difference.</param>
        internal static async Task GenerateExportAsync(string? informationOne, string? informationTwo, string? colorOne,
            string? colorTwo, string? similarity, Bitmap? difference)
        {
            var pathObj = DialogHandler.HandleFileSave(ViewResources.FileOpenTxt, null!);

            if (pathObj == null) return;

            var content = new List<string?>
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
                await File.WriteAllLinesAsync(pathObj.FilePath!, content!).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is IOException or ArgumentException)
            {
                Trace.WriteLine(ex);
                ShowError(ex.ToString(), nameof(GenerateExportAsync));
            }

            if (difference is { })
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
        internal static Bitmap? Resize(Bitmap? bitmap, int width, int height)
        {
            if (bitmap == null) return null;

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
        internal static Bitmap? Filter(Bitmap? bitmap, FiltersType filter)
        {
            if (filter == FiltersType.None) return bitmap;

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
        internal static Bitmap? Texture(Bitmap? bitmap, TextureType texture)
        {
            try
            {
                //just overlay the whole image with the texture, will be changed later
                return Generator.GenerateTextureOverlay(bitmap, bitmap!.Width, bitmap.Height, texture,
                    MaskShape.Rectangle);
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
        public static Bitmap? Pixelate(Bitmap bitmap, int pixelWidth)
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
        internal static bool SaveImage(string? path, string? extension, Bitmap? bitmap)
        {
            path = Path.ChangeExtension(path, extension);

            if (TryGetBuiltInFormat(extension, out var format))
            {
                Render.SaveBitmap(bitmap, path, format);
                return true;
            }

            // Not one of the built-in GDI+ formats - see if a decoder/encoder plugin can write
            // it instead (e.g. saving back out to .webp). This is what actually lets a plugin
            // format round-trip: decoding one already worked via ImageDecoderPluginRegistry, this
            // is the save-side counterpart that was previously just missing entirely - any
            // extension not in the switch below threw, full stop, regardless of what plugins were
            // loaded.
            if (ImageDecoderPluginRegistry.Instance.TryGetEncoder(extension, out var encoderPlugin) &&
                encoderPlugin is { })
            {
                encoderPlugin.Encode(bitmap, path);
                return true;
            }

            throw new ArgumentException(ViewResources.ErrorNotSupported);
        }

        /// <summary>
        ///     Maps a file extension to one of the built-in GDI+ <see cref="ImageFormat" />s.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <param name="format">The resolved format, if <paramref name="extension" /> is a built-in one.</param>
        /// <returns><c>true</c> if <paramref name="extension" /> is a built-in format; otherwise, <c>false</c>.</returns>
        private static bool TryGetBuiltInFormat(string? extension, out ImageFormat? format)
        {
            switch (extension)
            {
                case ImagingResources.PngExt:
                    format = ImageFormat.Png;
                    return true;
                case ImagingResources.JpgExt:
                    format = ImageFormat.Jpeg;
                    return true;
                case ImagingResources.BmpExt:
                    format = ImageFormat.Bmp;
                    return true;
                case ImagingResources.GifExt:
                    format = ImageFormat.Gif;
                    return true;
                case ImagingResources.TifExt:
                    format = ImageFormat.Tiff;
                    return true;
                default:
                    format = default;
                    return false;
            }
        }

        /// <summary>
        ///     Loads an image from the specified path.
        /// </summary>
        /// <param name="path">The path to the image.</param>
        /// <returns>The loaded bitmap, or null if loading fails.</returns>
        internal static Bitmap? LoadImage(string? path)
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
        internal static Bitmap? GenerateImage(string? filePath)
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
        private static void ShowError(string message, string? source = null)
        {
            DialogHandler.ErrorDialog(message, source);
        }

        /// <summary>
        ///     Darkens the specified bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>Darken Image</returns>
        internal static Bitmap? Darken(Bitmap bitmap)
        {
            return Render.AdjustBrightness(bitmap, 0.1f);
        }

        /// <summary>
        ///     ds the brighten.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>Brighten the Image</returns>
        internal static Bitmap? Brighten(Bitmap bitmap)
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
        internal static Bitmap? EraseImage(SelectionFrame frame, Bitmap? btm)
        {
            if (btm == null) return null;

            try
            {
                var erasedBtm = Render.EraseRectangle(btm, frame.X, frame.Y, frame.Width, frame.Height);

                // If EraseRectangle returns a NEW Bitmap instance, dispose the old one to prevent handle leaks
                if (!ReferenceEquals(erasedBtm, btm))
                {
                    btm.Dispose();
                }

                return erasedBtm;
            }
            catch (ArgumentNullException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), string.Concat(ViewResources.ErrorMessage, nameof(EraseImage)));
            }

            return btm;
        }

        /// <summary>
        ///      Fills the area.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="color">The color.</param>
        /// <returns>Changed bitmap or in case of error the original</returns>
        public static Bitmap FillArea(Bitmap bitmap, SelectionFrame frame, Color color)
        {
            // 1. Get the Mask Shape (Rectangle, Circle, Polygon)
            var mask = Translator.MapCodeToTool(frame.Tool);
            var point = new Point(frame.X, frame.Y);

            // 2. Prepare Shape Parameters for Polygon/FreeForm
            object? shapeParams = null;

            if (mask == MaskShape.Polygon)
            {
                if (frame.Points is { Count: > 0 })
                {
                    // CRITICAL STEP: Convert WPF Points (System.Windows.Point)
                    // to GDI+ Points (System.Drawing.Point)
                    shapeParams = frame.Points
                        .Select(p => new Point((int)p.X, (int)p.Y))
                        .ToArray();
                }
            }

            try
            {
                // 3. Pass shapeParams to the Render engine
                bitmap = Render.FillAreaWithColor(
                    bitmap,
                    frame.Width,
                    frame.Height,
                    color,
                    mask,
                    shapeParams: shapeParams, // <--- Pass the array here
                    startPoint: point);
            }
            catch (ArgumentNullException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), string.Concat(ViewResources.ErrorMessage, nameof(FillArea)));
            }
            catch (ArgumentException ex)
            {
                // Catch the "Invalid shape parameters" exception from ImageStream
                Trace.WriteLine(ex);
                _ = MessageBox.Show("Failed to fill polygon: " + ex.Message, ViewResources.ErrorMessage);
            }

            return bitmap;
        }

        /// <summary>
        ///     Fills the texture.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="texture">The texture.</param>
        /// <returns>Changed bitmap or in case of error the original</returns>
        public static Bitmap? FillTexture(Bitmap? bitmap, SelectionFrame frame, TextureType texture)
        {
            var mask = Translator.MapCodeToTool(frame.Tool);
            var point = new Point(frame.X, frame.Y);

            // Prepare Polygon Points
            object? shapeParams = null;
            if (mask == MaskShape.Polygon && frame.Points is { Count: > 0 })
            {
                // Convert WPF Points -> GDI+ Points
                shapeParams = frame.Points
                    .Select(p => new Point((int)p.X, (int)p.Y))
                    .ToArray();
            }

            try
            {
                // Pass shapeParams to the Generator
                // Ensure your GenerateTextureOverlay method accepts this argument!
                return Generator.GenerateTextureOverlay(
                    bitmap,
                    frame.Width,
                    frame.Height,
                    texture,
                    mask,
                    point, // StartPoint
                    shapeParams); // The Polygon Points
            }
            catch (Exception ex) when (ex is ArgumentException or OutOfMemoryException)
            {
                Trace.WriteLine(ex);
                ShowError(ex.ToString(), ViewResources.ErrorMessage);
            }

            return bitmap;
        }

        /// <summary>
        ///     Fills the filter.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>Changed bitmap or in case of error the original</returns>
        public static Bitmap? FillFilter(Bitmap? bitmap, SelectionFrame frame, FiltersType filter)
        {
            var mask = Translator.MapCodeToTool(frame.Tool);
            var point = new Point(frame.X, frame.Y);

            // Prepare Polygon Points
            object? shapeParams = null;
            if (mask == MaskShape.Polygon && frame.Points is { Count: > 0 })
            {
                // Convert WPF Points -> GDI+ Points
                shapeParams = frame.Points
                    .Select(p => new Point((int)p.X, (int)p.Y))
                    .ToArray();
            }

            try
            {
                // Pass shapeParams to the Render engine
                // Ensure your FilterImageArea method accepts this argument!
                return Render.FilterImageArea(
                    bitmap,
                    frame.Width,
                    frame.Height,
                    filter,
                    mask,
                    shapeParams: shapeParams, // <--- The Polygon Points
                    startPoint: point);
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
        public static Bitmap? RotateImage(Bitmap? btm, int degree)
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
        internal static void FolderConvert(string? target, string? source, Dictionary<int, string>? observer)
        {
            if (observer == null)
            {
                Trace.WriteLine("Observer dictionary is null.");
                return;
            }

            try
            {
                var count = 0;
                var error = 0;

                // Query directly - no List allocation needed!
                IEnumerable<string?> matchingImages =
                    observer.Values.Where(element => Path.GetExtension(element) == source);

                foreach (var image in matchingImages)
                {
                    // Use the 'using' statement to guarantee the unmanaged resources are freed immediately
                    using var btm = Render.GetOriginalBitmap(image);

                    if (SaveImage(image, target, btm))
                        count++;
                    else
                        error++;
                }

                _ = MessageBox.Show(string.Concat(ViewResources.InformationConverted, count, Environment.NewLine,
                    ViewResources.InformationErrors, error));
            }
            catch (Exception ex) when (ex is ArgumentException or IOException or ExternalException
                                           or NullReferenceException)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    string.Concat(ViewResources.ErrorMessage, nameof(FolderConvert)));
            }
        }
    }
}