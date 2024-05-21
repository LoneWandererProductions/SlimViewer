﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ImageCompare
 * FILE:        ImageCompare/ImageSlider.cs
 * PURPOSE:     Checks if an Image is contained in another image. Stuff like a cut out.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Drawing;
using Imaging;
using Mathematics;

namespace ImageCompare
{
    /// <summary>
    /// Image Slider to check if Image is part of a smaller one
    /// </summary>
    internal static class ImageSlider
    {
        /// <summary>
        /// Determines whether [is part of] [the specified big image path].
        /// </summary>
        /// <param name="bigImagePath">The big image path.</param>
        /// <param name="smallImagePath">The small image path.</param>
        /// <param name="startCoordinates">The start coordinates.</param>
        /// <returns>
        ///   <c>true</c> if [is part of] [the specified big image path]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsPartOf(string bigImagePath, string smallImagePath, out Coordinate2D startCoordinates)
        {
            using var bigImage = new Bitmap(bigImagePath);
            using var smallImage = new Bitmap(smallImagePath);
            return IsPartOf(bigImage, smallImage, out startCoordinates);
        }

        /// <summary>
        /// Determines whether the small image is part of the big image.
        /// </summary>
        /// <param name="bigImage">The big image.</param>
        /// <param name="smallImage">The small image.</param>
        /// <param name="startCoordinates">The starting coordinates where the small image overlays the big image.</param>
        /// <returns>
        ///   <c>true</c> if the small image is part of the big image; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsPartOf(Bitmap bigImage, Bitmap smallImage, out Coordinate2D startCoordinates)
        {
            int bigHeight = bigImage.Height;
            int bigWidth = bigImage.Width;
            int smallHeight = smallImage.Height;
            int smallWidth = smallImage.Width;

            using var dbmBig = new DirectBitmap(bigImage);
            using var dbmSmall = new DirectBitmap(smallImage);

            var smallImageBottomEdge = new DirectBitmap(smallWidth, 1);

            for (int i = 0; i <= bigHeight - smallHeight; i++)
            {
                for (int j = 0; j <= bigWidth - smallWidth; j++)
                {
                    // Update the bottom edge for the current position in the big image
                    for (int x = 0; x < smallWidth; x++)
                    {
                        smallImageBottomEdge.SetPixel(x, 0, dbmBig.GetPixel(j + x, i + smallHeight - 1));
                    }

                    if (CheckEdges(dbmBig, dbmSmall, i, j, smallImageBottomEdge) && CheckFull(dbmBig, dbmSmall, i, j))
                    {
                        smallImageBottomEdge.Dispose();
                        startCoordinates = new Coordinate2D(j, i);
                        return true;
                    }
                }
            }

            smallImageBottomEdge.Dispose();
            startCoordinates = Coordinate2D.NullPoint;
            return false;
        }

        /// <summary>
        /// Checks the edges of the small image within the big image.
        /// </summary>
        /// <param name="bigImage">The big image.</param>
        /// <param name="smallImage">The small image.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="smallImageBottomEdge">The small image bottom edge.</param>
        /// <returns>Basic check of the edges</returns>
        private static bool CheckEdges(DirectBitmap bigImage, DirectBitmap smallImage, int startRow, int startCol, DirectBitmap smallImageBottomEdge)
        {
            int smallHeight = smallImage.Height;
            int smallWidth = smallImage.Width;

            // Check top edge
            for (int x = 0; x < smallWidth; x++)
            {
                if (bigImage.GetPixel(startCol + x, startRow) != smallImage.GetPixel(x, 0))
                {
                    return false;
                }
            }

            // Check bottom edge
            for (int x = 0; x < smallWidth; x++)
            {
                if (bigImage.GetPixel(startCol + x, startRow + smallHeight - 1) != smallImageBottomEdge.GetPixel(x, 0))
                {
                    return false;
                }
            }

            // Check left edge
            for (int y = 0; y < smallHeight; y++)
            {
                if (bigImage.GetPixel(startCol, startRow + y) != smallImage.GetPixel(0, y))
                {
                    return false;
                }
            }

            // Check right edge
            for (int y = 0; y < smallHeight; y++)
            {
                if (bigImage.GetPixel(startCol + smallWidth - 1, startRow + y) != smallImage.GetPixel(smallWidth - 1, y))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Fully checks if the images match in the defined area.
        /// </summary>
        /// <param name="bigImage">The big image.</param>
        /// <param name="smallImage">The small image.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <returns>
        /// True if images are equal in the defined area, otherwise false.
        /// </returns>
        private static bool CheckFull(DirectBitmap bigImage, DirectBitmap smallImage, int startRow, int startCol)
        {
            int smallHeight = smallImage.Height;
            int smallWidth = smallImage.Width;

            for (int y = 0; y < smallHeight; y++)
            {
                for (int x = 0; x < smallWidth; x++)
                {
                    if (bigImage.GetPixel(startCol + x, startRow + y) != smallImage.GetPixel(x, y))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
