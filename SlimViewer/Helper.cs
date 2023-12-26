/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViewer/Helper.cs
 * PURPOSE:     Basic Helper Methods that are used across controls
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */


using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Imaging;

namespace SlimViewer
{
    internal static class Helper
    {
        /// <summary>
        ///     The render
        /// </summary>
        internal static readonly ImageRender Render = new();

        /// <summary>
        ///     Saves the image.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="btm">The BTM.</param>
        /// <returns>success Status</returns>
        internal static bool SaveImage(string path, string extension, Bitmap btm)
        {
            if (File.Exists(path)) return false;

            if (string.Equals(extension, SlimViewerResources.JpgExtAlt, StringComparison.CurrentCultureIgnoreCase))
                extension = ImagingResources.JpgExt;

            path = Path.ChangeExtension(path, extension);

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
        ///     Converts the gif to images action.
        /// </summary>
        /// <param name="gifPath"></param>
        /// <param name="imageExport"></param>
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
                    _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
                }
                catch (IOException ex)
                {
                    Trace.WriteLine(ex);
                    _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
                }
                catch (ExternalException ex)
                {
                    Trace.WriteLine(ex);
                    _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
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
    }
}