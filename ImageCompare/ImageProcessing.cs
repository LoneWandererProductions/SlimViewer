/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ImageCompare
 * FILE:        ImageCompare/ImageProcessing.cs
 * PURPOSE:     Basic Processing of Images, in this case mostly for Similar Images
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading.Tasks;
using ExtendedSystemObjects;
using Imaging;

namespace ImageCompare
{
    /// <summary>
    ///     Helper Class that handles the specific way to get the similarity of two Images
    /// </summary>
    internal static class ImageProcessing
    {
        /// <summary>
        ///     The Image render
        /// </summary>
        private static readonly ImageRender Render = new();

        /// <summary>
        ///     Find all duplicate images from in list
        /// </summary>
        /// <param name="imageToCompareTo">The path of image to compare to.</param>
        /// <param name="images">The paths to the images to check for duplicates</param>
        /// <param name="maximumDifferenceInPercentage">The maximum difference in percentage.</param>
        /// <returns>
        ///     A list of paths to all the duplicates found.
        /// </returns>
        [return: MaybeNull]
        internal static List<ImageSimilar> FindSimilarImages(ImageSimilar imageToCompareTo,
            IEnumerable<ImageSimilar> images,
            float maximumDifferenceInPercentage)
        {
            var similarImagesFound = new List<ImageSimilar>();

            _ = Parallel.ForEach(images, image =>
            {
                var percentageDiff =
                    GetPercentageDifference(image, imageToCompareTo);
                if (percentageDiff >= maximumDifferenceInPercentage)
                {
                    similarImagesFound.Add(image);
                }
            });

            if (similarImagesFound.IsNullOrEmpty())
            {
                return null;
            }

            return similarImagesFound.Count == 1 ? null : similarImagesFound;
        }

        /// <summary>
        ///     Generates the data. No need to resize! Only Change to  Greyscale.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>Image Object to compare</returns>
        internal static ImageSimilar GenerateData(Bitmap bitmap, int id)
        {
            //resize
            bitmap = Render.BitmapScaling(bitmap, ImageResources.DuplicateSize, ImageResources.DuplicateSize);

            //use our new Format
            var dbm = DirectBitmap.GetInstance(bitmap);
            var totalPixels = ImageResources.DuplicateSize * ImageResources.DuplicateSize;

            //get the average Color Value
            var r = 0;
            var b = 0;
            var g = 0;

            var hash = new byte[totalPixels];

            //TODO replace with custom dbm Method

            for (var y = 0; y < ImageResources.DuplicateSize; y++)
            for (var x = 0; x < ImageResources.DuplicateSize; x++)
            {
                var pixel = dbm.GetPixel(x, y);
                r += pixel.R;
                g += pixel.G;
                b += pixel.B;

                // Calculate grayscale value directly from the current pixel
                hash[y * ImageResources.DuplicateSize + x] = (byte)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
            }

            r /= totalPixels;
            b /= totalPixels;
            g /= totalPixels;

            return new ImageSimilar
            {
                R = r,
                G = g,
                B = b,
                Id = id,
                Hash = hash
            };
        }

        /// <summary>
        ///     Gets the percentage difference.
        /// </summary>
        /// <param name="imageToCompareTo">The image to compare to.</param>
        /// <param name="targetBitmap">The target bitmap.</param>
        /// <returns>Difference in Percentage</returns>
        internal static float GetPercentageDifference(ImageSimilar imageToCompareTo, ImageSimilar targetBitmap)
        {
            var diff = 0;
            var totalPixels = ImageResources.DuplicateSize * ImageResources.DuplicateSize;

            // Compare grayscale hashes instead of 2D image array
            for (var i = 0; i < totalPixels; i++)
            {
                int one = imageToCompareTo.Hash[i];
                int two = targetBitmap.Hash[i];

                if (one.Interval(two, ImageResources.ColorThreshold))
                {
                    diff++;
                }
            }

            // Calculate the pixel similarity percentage
            var pixelSimilarity = (float)diff / totalPixels * 100;

            // Calculate color similarity based on RGB differences
            var colorSimilarity = (float)
                (((ImageResources.MaxColor - Math.Abs(imageToCompareTo.R - targetBitmap.R)) / ImageResources.MaxColor) +
                 ((ImageResources.MaxColor - Math.Abs(imageToCompareTo.G - targetBitmap.G)) / ImageResources.MaxColor) +
                 ((ImageResources.MaxColor - Math.Abs(imageToCompareTo.B - targetBitmap.B)) / ImageResources.MaxColor)) / 3 * 100;

            // Return the average of pixel similarity and color similarity
            return (pixelSimilarity + colorSimilarity) / 2;
        }
    }
}
