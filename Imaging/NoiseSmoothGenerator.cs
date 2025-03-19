/*
 * COPYRIGHT:   See COPYING in the top-level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/NoiseSmoothGenerator.cs
 * PURPOSE:     Provides noise generation utilities for procedural texture generation. An Variation of the existing one.
 * AUTHOR:      Peter Geinitz (Wayfarer)
 */

using System;

namespace Imaging
{
    /// <summary>
    /// Generates procedural noise for texture generation.
    /// </summary>
    internal sealed class NoiseSmoothGenerator
    {
        /// <summary>
        /// The noise width
        /// </summary>
        private const int NoiseWidth = 320; // Width of noise map

        /// <summary>
        /// The noise height
        /// </summary>
        private const int NoiseHeight = 240; // Height of noise map

        /// <summary>
        /// 2D array storing precomputed noise values.
        /// </summary>
        private static readonly double[,] Noise;

        /// <summary>
        /// Static constructor to initialize the noise array.
        /// </summary>
        static NoiseSmoothGenerator()
        {
            Noise = GenerateNoise();
        }

        /// <summary>
        /// Generates a 2D noise map with random values.
        /// </summary>
        /// <returns>A 2D array of noise values.</returns>
        private static double[,] GenerateNoise()
        {
            var noiseData = new double[NoiseHeight, NoiseWidth];
            var random = new Random();

            for (var y = 0; y < NoiseHeight; y++)
            {
                for (var x = 0; x < NoiseWidth; x++)
                {
                    noiseData[y, x] = random.NextDouble(); // Random value between 0.0 and 1.0
                }
            }

            return noiseData;
        }

        /// <summary>
        /// Generates turbulence at a given point.
        /// </summary>
        /// <param name="x">X-coordinate in noise space.</param>
        /// <param name="y">Y-coordinate in noise space.</param>
        /// <param name="size">Frequency of the turbulence.</param>
        /// <returns>Turbulence value at the given coordinates.</returns>
        public double Turbulence(int x, int y, double size)
        {
            double value = 0.0;
            double initialSize = size;

            while (size >= 1)
            {
                value += SmoothNoise(x / size, y / size) * size;
                size /= 2;
            }

            return (value / initialSize) * 255.0;
        }

        /// <summary>
        /// Computes interpolated noise at non-integer coordinates.
        /// </summary>
        /// <param name="x">X-coordinate in noise space.</param>
        /// <param name="y">Y-coordinate in noise space.</param>
        /// <returns>Smoothed noise value.</returns>
        public double SmoothNoise(double x, double y)
        {
            int intX = (int)Math.Floor(x);
            double fracX = x - intX;
            int intY = (int)Math.Floor(y);
            double fracY = y - intY;

            double v1 = Noise[(intY + NoiseHeight) % NoiseHeight, (intX + NoiseWidth) % NoiseWidth];
            double v2 = Noise[(intY + NoiseHeight) % NoiseHeight, (intX + 1 + NoiseWidth) % NoiseWidth];
            double v3 = Noise[(intY + 1 + NoiseHeight) % NoiseHeight, (intX + NoiseWidth) % NoiseWidth];
            double v4 = Noise[(intY + 1 + NoiseHeight) % NoiseHeight, (intX + 1 + NoiseWidth) % NoiseWidth];

            double i1 = ImageHelper.Interpolate(v1, v2, fracX);
            double i2 = ImageHelper.Interpolate(v3, v4, fracX);

            return ImageHelper.Interpolate(i1, i2, fracY);
        }
    }
}
