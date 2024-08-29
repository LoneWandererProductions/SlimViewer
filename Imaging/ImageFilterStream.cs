﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageFilterStream.cs
 * PURPOSE:     Separate out all Filter Operations
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

// TODO add:
// Prewitt
// Roberts Cross
// Laplacian
// Laplacian of Gaussain
// Anisotropic Kuwahara


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Imaging
{
    /// <summary>
    ///     Here we handle the grunt work for all image filters
    /// </summary>
    internal static class ImageFilterStream
    {
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
        internal static Bitmap FilterImage(Bitmap image, ImageFilters filter)
        {
            ImageHelper.ValidateImage(nameof(FilterImage), image);

            //create a blank bitmap the same size as original
            var btm = new Bitmap(image.Width, image.Height);
            btm.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //get a graphics object from the new image
            using var graph = Graphics.FromImage(btm);
            //create some image attributes
            using var atr = new ImageAttributes();

            ImageFilterConfig settings;

            //set the color matrix attribute
            switch (filter)
            {
                case ImageFilters.GrayScale:
                    atr.SetColorMatrix(ImageRegister.GrayScale);
                    break;
                case ImageFilters.Invert:
                    atr.SetColorMatrix(ImageRegister.Invert);
                    break;
                case ImageFilters.Sepia:
                    atr.SetColorMatrix(ImageRegister.Sepia);
                    break;
                case ImageFilters.BlackAndWhite:
                    atr.SetColorMatrix(ImageRegister.BlackAndWhite);
                    break;
                case ImageFilters.Polaroid:
                    atr.SetColorMatrix(ImageRegister.Polaroid);
                    break;
                case ImageFilters.Contour:
                    return ApplySobel(image);
                case ImageFilters.Brightness:
                    atr.SetColorMatrix(ImageRegister.Brightness);
                    break;
                case ImageFilters.Contrast:
                    atr.SetColorMatrix(ImageRegister.Contrast);
                    break;
                case ImageFilters.HueShift:
                    atr.SetColorMatrix(ImageRegister.HueShift);
                    break;
                case ImageFilters.ColorBalance:
                    atr.SetColorMatrix(ImageRegister.ColorBalance);
                    break;
                case ImageFilters.Vintage:
                    atr.SetColorMatrix(ImageRegister.Vintage);
                    break;
                // New convolution-based filters
                case ImageFilters.Sharpen:
                    settings = ImageRegister.GetSettings(ImageFilters.Sharpen);
                    return ApplyFilter(image, ImageRegister.SharpenFilter, settings.Factor, settings.Bias);
                case ImageFilters.GaussianBlur:
                    settings = ImageRegister.GetSettings(ImageFilters.GaussianBlur);
                    return ApplyFilter(image, ImageRegister.GaussianBlur, settings.Factor, settings.Bias);
                case ImageFilters.Emboss:
                    settings = ImageRegister.GetSettings(ImageFilters.Emboss);
                    return ApplyFilter(image, ImageRegister.EmbossFilter, settings.Factor, settings.Bias);
                case ImageFilters.BoxBlur:
                    settings = ImageRegister.GetSettings(ImageFilters.BoxBlur);
                    return ApplyFilter(image, ImageRegister.BoxBlur, settings.Factor, settings.Bias);
                case ImageFilters.Laplacian:
                    settings = ImageRegister.GetSettings(ImageFilters.Laplacian);
                    return ApplyFilter(image, ImageRegister.LaplacianFilter, settings.Factor, settings.Bias);
                case ImageFilters.EdgeEnhance:
                    settings = ImageRegister.GetSettings(ImageFilters.EdgeEnhance);
                    return ApplyFilter(image, ImageRegister.EdgeEnhance, settings.Factor, settings.Bias);
                case ImageFilters.MotionBlur:
                    settings = ImageRegister.GetSettings(ImageFilters.MotionBlur);
                    return ApplyFilter(image, ImageRegister.MotionBlur, settings.Factor, settings.Bias);
                case ImageFilters.UnsharpMask:
                    settings = ImageRegister.GetSettings(ImageFilters.UnsharpMask);
                    return ApplyFilter(image, ImageRegister.UnsharpMask, settings.Factor, settings.Bias);
                // custom Filter
                case ImageFilters.DifferenceOfGaussians:
                    return ApplyDifferenceOfGaussians(image);
                case ImageFilters.Crosshatch:
                    return ApplyCrosshatch(image);
                case ImageFilters.FloydSteinbergDithering:
                    return ApplyFloydSteinbergDithering(image);
                case ImageFilters.AnisotropicKuwahara:
                    settings = ImageRegister.GetSettings(ImageFilters.AnisotropicKuwahara);
                    return ApplyAnisotropicKuwahara(image, settings.BaseWindowSize);
                case ImageFilters.SupersamplingAntialiasing:
                    settings = ImageRegister.GetSettings(ImageFilters.SupersamplingAntialiasing);
                    return ApplySupersamplingAntialiasing(image, settings.Scale);
                case ImageFilters.PostProcessingAntialiasing:
                    settings = ImageRegister.GetSettings(ImageFilters.PostProcessingAntialiasing);
                    return ApplyPostProcessingAntialiasing(image, settings.Sigma);
                case ImageFilters.PencilSketchEffect:
                    settings = ImageRegister.GetSettings(ImageFilters.PencilSketchEffect);
                    return ApplyPostProcessingAntialiasing(image, settings.Sigma);
				case ImageFilters.None:
					break;
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
        ///     Applies the filter.
        /// </summary>
        /// <param name="sourceBitmap">The source bitmap.</param>
        /// <param name="filterMatrix">
        ///     The filter matrix.
        ///     Matrix Definition: The convolution matrix is typically a 2D array of numbers (weights) that defines how each pixel
        ///     in the image should be altered based on its neighboring pixels. Common sizes are 3x3, 5x5, or 7x7.
        ///     Placement: Place the center of the convolution matrix on the target pixel in the image.
        ///     Neighborhood Calculation: Multiply the value of each pixel in the neighborhood by the corresponding value in the
        ///     convolution matrix.
        ///     Summation: Sum all these products.
        ///     Normalization: Often, the result is normalized (e.g., dividing by the sum of the matrix values) to ensure that
        ///     pixel values remain within a valid range.
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
            // Initialize DirectBitmap instances
            var source = new DirectBitmap(sourceBitmap);
            var result = new DirectBitmap(source.Width, source.Height);

            var filterWidth = filterMatrix.GetLength(1);
            var filterHeight = filterMatrix.GetLength(0);
            var filterOffset = filterWidth / 2;

            // Prepare a list to store the pixels to set in bulk using SIMD
            var pixelsToSet = new List<(int x, int y, Color color)>();

            for (var y = filterOffset; y < source.Height - filterOffset; y++)
            for (var x = filterOffset; x < source.Width - filterOffset; x++)
            {
                double blue = 0.0, green = 0.0, red = 0.0;

                for (var filterY = 0; filterY < filterHeight; filterY++)
                for (var filterX = 0; filterX < filterWidth; filterX++)
                {
                    var imageX = x + (filterX - filterOffset);
                    var imageY = y + (filterY - filterOffset);

                    // Check bounds to prevent out-of-bounds access
                    if (imageX < 0 || imageX >= source.Width || imageY < 0 || imageY >= source.Height) continue;

                    var pixelColor = source.GetPixel(imageX, imageY);

                    blue += pixelColor.B * filterMatrix[filterY, filterX];
                    green += pixelColor.G * filterMatrix[filterY, filterX];
                    red += pixelColor.R * filterMatrix[filterY, filterX];
                }

                var newBlue = ImageHelper.Clamp(factor * blue + bias);
                var newGreen = ImageHelper.Clamp(factor * green + bias);
                var newRed = ImageHelper.Clamp(factor * red + bias);

                // Instead of setting the pixel immediately, add it to the list
                pixelsToSet.Add((x, y, Color.FromArgb(newRed, newGreen, newBlue)));
            }

            // Use SIMD to set all the pixels in bulk
            try
            {
                result.SetPixelsSimd(pixelsToSet);

                return result.Bitmap;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{ImagingResources.ErrorPixel} {ex.Message}");
            }

            return null;
        }

        /// <summary>
        ///     Pixelate the specified input image.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="stepWidth">Width of the step.</param>
        /// <returns>Pixelated Image</returns>
        internal static Bitmap Pixelate(Image image, int stepWidth)
        {
            if (image == null)
            {
                var innerException = new ArgumentNullException(string.Concat(nameof(Pixelate),
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
                var rectacngle = new Rectangle(x, y, stepWidth, stepWidth);
                var averageColor = ImageHelper.GetMeanColor(dbm, rectacngle);

                using var g = Graphics.FromImage(processedImage);
                using var brush = new SolidBrush(averageColor);
                g.FillRectangle(brush, x, y, stepWidth, stepWidth);
            }

            return processedImage;
        }

        /// <summary>
        ///     Applies the Sobel.
        /// </summary>
        /// <param name="originalImage">The original image.</param>
        /// <returns>Contour of an Image</returns>
        private static Bitmap ApplySobel(Bitmap originalImage)
        {
            // Convert the original image to greyscale
            var greyscaleImage = FilterImage(originalImage, ImageFilters.GrayScale);

            // Create a new bitmap to store the result of Sobel operator
            var resultImage = new Bitmap(greyscaleImage.Width, greyscaleImage.Height);

            // Sobel masks for gradient calculation
            int[,] sobelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] sobelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            // Prepare a list to store the pixels to set in bulk using SIMD
            var pixelsToSet = new List<(int x, int y, Color color)>();

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
                var magnitude = (int)Math.Sqrt(gx * gx + gy * gy);

                // Normalize the magnitude to fit within the range of 0-255
                magnitude = ImageHelper.Clamp(magnitude / Math.Sqrt(2)); // Divide by sqrt(2) for normalization

                // Instead of setting the pixel immediately, add it to the list
                pixelsToSet.Add((x, y, Color.FromArgb(magnitude, magnitude, magnitude)));
            }

            // Use SIMD to set all the pixels in bulk
            try
            {
                dbmResult.SetPixelsSimd(pixelsToSet);
                dbmBase.Dispose();

                return dbmResult.Bitmap;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error setting pixels: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        ///     Applies the difference of gaussians.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Filtered Image</returns>
        private static Bitmap ApplyDifferenceOfGaussians(Image image)
        {
            // Gaussian blur with small sigma
            var gaussianBlurSmall = ImageHelper.GenerateGaussianKernel(1.0, 5);

            // Gaussian blur with larger sigma
            var gaussianBlurLarge = ImageHelper.GenerateGaussianKernel(2.0, 5);

            // Apply both Gaussian blurs to the image
            var blurredSmall = ApplyFilter(image, gaussianBlurSmall, 1.0 / 16.0);
            var blurredLarge = ApplyFilter(image, gaussianBlurLarge, 1.0 / 16.0);

            // Subtract the two blurred images to get the DoG result
            return SubtractImages(blurredSmall, blurredLarge);
        }

        /// <summary>
        ///     Applies the crosshatch.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Filtered Image</returns>
        private static Bitmap ApplyCrosshatch(Image image)
        {
            // Apply the 45-degree and 135-degree filters
            var hatch45 = ApplyFilter(image, ImageRegister.Kernel45Degrees);
            var hatch135 = ApplyFilter(image, ImageRegister.Kernel135Degrees);

            // Combine the two hatching directions
            return ImageHelper.CombineImages(hatch45, hatch135);
        }

        /// <summary>
        ///     Applies the anisotropic kuwahara.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="baseWindowSize">Size of the base window.</param>
        /// <returns>Filtered Image</returns>
        private static Bitmap ApplyAnisotropicKuwahara(Bitmap image, int baseWindowSize = 5)
        {
            var dbmBase = new DirectBitmap(image);
            var result = new DirectBitmap(image.Width, image.Height);
            var halfBaseWindow = baseWindowSize / 2;

            // Prepare a list to store the pixels to set in bulk using SIMD
            var pixelsToSet = new List<(int x, int y, Color color)>();


            for (var y = halfBaseWindow; y < dbmBase.Height - halfBaseWindow; y++)
            for (var x = halfBaseWindow; x < dbmBase.Width - halfBaseWindow; x++)
            {
                // Determine region size and shape based on local image characteristics
                DetermineRegionSizeAndShape(dbmBase, x, y, halfBaseWindow, out var regionWidth,
                    out var regionHeight);

                var bestColor = ComputeBestRegionColor(dbmBase, x, y, regionWidth, regionHeight);

                // Instead of setting the pixel immediately, add it to the list
                pixelsToSet.Add((x, y, bestColor));
            }

            // Use SIMD to set all the pixels in bulk
            try
            {
                result.SetPixelsSimd(pixelsToSet);
                dbmBase.Dispose();

                return result.Bitmap;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error setting pixels: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        ///     Applies the floyd steinberg dithering.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Filtered Image</returns>
        private static Bitmap ApplyFloydSteinbergDithering(Bitmap image)
        {
            var dbmBase = new DirectBitmap(image);
            var result = new DirectBitmap(image.Width, image.Height);

            // Convert to grayscale
            var grayBitmap = FilterImage(image, ImageFilters.GrayScale);

            // Define the color palette for dithering
            var palette = new List<Color> { Color.Black, Color.White };

            // Floyd-Steinberg dithering matrix
            int[,] ditherMatrix = { { 0, 0, 7 }, { 3, 5, 1 } };

            // Apply dithering
            for (var y = 0; y < grayBitmap.Height; y++)
            for (var x = 0; x < grayBitmap.Width; x++)
            {
                // Get the original grayscale pixel value
                var oldColor = grayBitmap.GetPixel(x, y);
                var oldIntensity = oldColor.R; // Since it's grayscale, R=G=B

                // Find the closest color in the palette
                var newColor = GetNearestColor(oldIntensity, palette);
                result.SetPixel(x, y, newColor);

                // Calculate the quantization error
                var error = oldIntensity - newColor.R;

                // Distribute the error to neighboring pixels
                DistributeError(dbmBase, x, y, error, ditherMatrix);
            }

            return result.Bitmap;
        }

        /// <summary>
        ///     Applies the supersampling antialiasing.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>Filtered Image</returns>
        private static Bitmap ApplySupersamplingAntialiasing(Bitmap image, int scale = 1)
        {
            // Create a higher-resolution bitmap for supersampling
            var scaledBitmap = new Bitmap(image.Width * scale, image.Height * scale);
            using (var g = Graphics.FromImage(scaledBitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, scaledBitmap.Width, scaledBitmap.Height);
            }

            // Downsample the high-resolution bitmap to the original size
            var resultBitmap = new DirectBitmap(image.Width, image.Height);
            var scaledDbm = new DirectBitmap(scaledBitmap);

            for (var y = 0; y < resultBitmap.Height; y++)
            for (var x = 0; x < resultBitmap.Width; x++)
            {
                var startX = x * scale;
                var startY = y * scale;

                // Average the color values of the sample region
                int sumR = 0, sumG = 0, sumB = 0;
                for (var dy = 0; dy < scale; dy++)
                for (var dx = 0; dx < scale; dx++)
                {
                    var pixelColor = scaledDbm.GetPixel(startX + dx, startY + dy);
                    sumR += pixelColor.R;
                    sumG += pixelColor.G;
                    sumB += pixelColor.B;
                }

                var avgR = sumR / (scale * scale);
                var avgG = sumG / (scale * scale);
                var avgB = sumB / (scale * scale);

                resultBitmap.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
            }

            return resultBitmap.Bitmap;
        }

        /// <summary>
        ///     Applies the post processing antialiasing.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="sigma">The sigma.</param>
        /// <returns>Filtered Image</returns>
        private static Bitmap ApplyPostProcessingAntialiasing(Bitmap image, double sigma = 1.0)
        {
            // Convert the image to DirectBitmap
            var dbmBase = new DirectBitmap(image);

            // Generate a Gaussian kernel
            var gaussianKernel = ImageHelper.GenerateGaussianKernel(sigma, 5);

            // Apply the Gaussian blur filter
            return ApplyFilter(dbmBase.Bitmap, gaussianKernel);
        }

        public static Bitmap PencilSketchEffect(Bitmap originalImage)
        {
            // Step 1: Convert to Grayscale
            var grayscaleImage = FilterImage(originalImage, ImageFilters.GrayScale);

            // Step 2: Invert the Grayscale Image
            var invertedImage = FilterImage(grayscaleImage, ImageFilters.Invert);

            // Step 3: Apply Gaussian Blur to the Inverted Image
            var blurredImage = FilterImage(invertedImage, ImageFilters.GaussianBlur);

            // Step 4: Blend Grayscale Image with the Blurred, Inverted Image using Color Dodge
            var sketchImage = ColorDodgeBlend(grayscaleImage, blurredImage);

            // Step 5: Optional - Adjust Contrast/Brightness if necessary
            // sketchImage = AdjustContrastAndBrightness(sketchImage);

            return sketchImage;
        }


        /// <summary>
        ///     Colors the dodge blend.
        /// </summary>
        /// <param name="baseImage">The base image.</param>
        /// <param name="blendImage">The blend image.</param>
        /// <returns>Color blended Image</returns>
        private static Bitmap ColorDodgeBlend(Bitmap baseImage, Bitmap blendImage)
        {
            var result = new Bitmap(baseImage.Width, baseImage.Height);
            for (var y = 0; y < baseImage.Height; y++)
            for (var x = 0; x < baseImage.Width; x++)
            {
                var baseColor = baseImage.GetPixel(x, y);
                var blendColor = blendImage.GetPixel(x, y);

                var r = blendColor.R == 255 ? 255 : ImageHelper.Clamp((baseColor.R << 8) / (255 - blendColor.R));
                var g = blendColor.G == 255 ? 255 : ImageHelper.Clamp((baseColor.G << 8) / (255 - blendColor.G));
                var b = blendColor.B == 255 ? 255 : ImageHelper.Clamp((baseColor.B << 8) / (255 - blendColor.B));

                result.SetPixel(x, y, Color.FromArgb(r, g, b));
            }

            return result;
        }

        /// <summary>
        ///     Determines the region size and shape.
        /// </summary>
        /// <param name="dbmBase">The DBM base.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="baseHalfWindow">The base half window.</param>
        /// <param name="regionWidth">Width of the region.</param>
        /// <param name="regionHeight">Height of the region.</param>
        private static void DetermineRegionSizeAndShape(DirectBitmap dbmBase, int x, int y, int baseHalfWindow,
            out int regionWidth, out int regionHeight)
        {
            // Placeholder logic to determine region size and shape
            // This is an example, you may need to adjust this based on your specific needs
            // For simplicity, let's assume a fixed size for regions, but in practice, this should be adaptive
            regionWidth = baseHalfWindow * 2;
            regionHeight = baseHalfWindow * 2;
        }

        /// <summary>
        ///     Computes the color of the best region.
        /// </summary>
        /// <param name="dbmBase">The DBM base.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="regionWidth">Width of the region.</param>
        /// <param name="regionHeight">Height of the region.</param>
        /// <returns>Best Color</returns>
        private static Color ComputeBestRegionColor(DirectBitmap dbmBase, int x, int y, int regionWidth,
            int regionHeight)
        {
            var bestColor = Color.Black;
            var bestVariance = double.MaxValue;

            // Define regions within the current window
            var regions = DefineRegions(x, y, regionWidth, regionHeight);

            foreach (var region in regions)
            {
                var (pixels, meanColor) = ImageHelper.GetRegionPixelsAndMeanColor(dbmBase, region);

                // Calculate variance for the current region
                var variance = CalculateVariance(pixels, meanColor);

                if (variance < bestVariance)
                {
                    bestVariance = variance;
                    bestColor = meanColor;
                }
            }

            return bestColor;
        }

        /// <summary>
        ///     Defines the regions.
        /// </summary>
        /// <param name="centerX">The center x.</param>
        /// <param name="centerY">The center y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Area of the image</returns>
        private static IEnumerable<Rectangle> DefineRegions(int centerX, int centerY, int width, int height)
        {
            // Example logic to generate multiple regions
            // This is a placeholder and should be replaced with logic to adapt the shape and size based on local image characteristics
            var regions = new List<Rectangle>
            {
                new(centerX - width / 2, centerY - height / 2, width, height)
                // Add more regions if needed, e.g., smaller regions, different orientations
            };

            return regions;
        }

        /// <summary>
        ///     Calculates the variance.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="meanColor">Color of the mean.</param>
        /// <returns>Variance</returns>
        private static double CalculateVariance(List<Color> pixels, Color meanColor)
        {
            var variance = 0.0;

            foreach (var pixel in pixels)
                variance += Math.Pow(pixel.R - meanColor.R, 2) + Math.Pow(pixel.G - meanColor.G, 2) +
                            Math.Pow(pixel.B - meanColor.B, 2);

            return variance / pixels.Count;
        }

        /// <summary>
        ///     Gets the color of the nearest.
        /// </summary>
        /// <param name="intensity">The intensity.</param>
        /// <param name="palette">The palette.</param>
        /// <returns>Nearest Color</returns>
        private static Color GetNearestColor(int intensity, List<Color> palette)
        {
            var nearestColor = palette[0];
            var minDifference = Math.Abs(intensity - nearestColor.R);

            foreach (var color in palette)
            {
                var difference = Math.Abs(intensity - color.R);
                if (difference < minDifference)
                {
                    minDifference = difference;
                    nearestColor = color;
                }
            }

            return nearestColor;
        }

        /// <summary>
        ///     Distributes the error.
        /// </summary>
        /// <param name="dbmBase">The DBM base.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="error">The error.</param>
        /// <param name="ditherMatrix">The dither matrix.</param>
        private static void DistributeError(DirectBitmap dbmBase, int x, int y, int error, int[,] ditherMatrix)
        {
            var matrixHeight = ditherMatrix.GetLength(0);
            var matrixWidth = ditherMatrix.GetLength(1);

            for (var dy = 0; dy < matrixHeight; dy++)
            for (var dx = 0; dx < matrixWidth; dx++)
            {
                var nx = x + dx - 1;
                var ny = y + dy;

                if (nx >= 0 && nx < dbmBase.Width && ny >= 0 && ny < dbmBase.Height)
                {
                    var pixel = dbmBase.GetPixel(nx, ny);
                    var oldIntensity = pixel.R; // Since it's grayscale, R=G=B
                    var newIntensity = ImageHelper.Clamp(oldIntensity + error * ditherMatrix[dy, dx] / 16);
                    var newColor = Color.FromArgb(newIntensity, newIntensity, newIntensity);
                    dbmBase.SetPixel(nx, ny, newColor);
                }
            }
        }

        /// <summary>
        ///     Subtracts the images.
        /// </summary>
        /// <param name="imgOne">The img1.</param>
        /// <param name="imgTwo">The img2.</param>
        /// <returns>Filtered Image</returns>
        private static Bitmap SubtractImages(Image imgOne, Image imgTwo)
        {
            var result = new DirectBitmap(imgOne.Width, imgOne.Height);
            // Prepare a list to store the pixels to set in bulk using SIMD
            var pixelsToSet = new List<(int x, int y, Color color)>();

            var dbmOne = new DirectBitmap(imgOne);
            var dbmTwo = new DirectBitmap(imgTwo);

            for (var y = 0; y < dbmOne.Height; y++)
            for (var x = 0; x < dbmOne.Width; x++)
            {
                var color1 = dbmOne.GetPixel(x, y);
                var color2 = dbmTwo.GetPixel(x, y);

                var r = Math.Max(0, color1.R - color2.R);
                var g = Math.Max(0, color1.G - color2.G);
                var b = Math.Max(0, color1.B - color2.B);

                // Instead of setting the pixel immediately, add it to the list
                pixelsToSet.Add((x, y, Color.FromArgb(r, g, b)));
            }

            // Use SIMD to set all the pixels in bulk
            try
            {
                result.SetPixelsSimd(pixelsToSet);

                return result.Bitmap;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{ImagingResources.ErrorPixel} {ex.Message}");
            }

            return null;
        }
    }
}