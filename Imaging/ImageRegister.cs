﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        Imaging/ImageRegister.cs
 * PURPOSE:     Register for Image Operations, and some helpful extensions
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://docs.rainmeter.net/tips/colormatrix-guide/
 *              https://archive.ph/hzR2W
 *              https://www.codeproject.com/Articles/3772/ColorMatrix-Basics-Simple-Image-Color-Adjustment
 */

// ReSharper disable MemberCanBeInternal
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global, should be viewed external, since this is the config

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Imaging
{
    /// <summary>
    ///     The image register class.
    /// </summary>
    public static class ImageRegister
    {
        /// <summary>
        ///     The filter property map
        ///     Mapping of filters to their used properties
        /// </summary>
        private static readonly Dictionary<ImageFilters, HashSet<string>> FilterPropertyMap = new()
        {
            {
                ImageFilters.GaussianBlur,
                new HashSet<string> { nameof(ImageFilterConfig.Factor), nameof(ImageFilterConfig.Bias) }
            },
            {
                ImageFilters.BoxBlur,
                new HashSet<string> { nameof(ImageFilterConfig.Factor), nameof(ImageFilterConfig.Bias) }
            },
            {
                ImageFilters.MotionBlur,
                new HashSet<string> { nameof(ImageFilterConfig.Factor), nameof(ImageFilterConfig.Bias) }
            },
            {
                ImageFilters.Sharpen,
                new HashSet<string> { nameof(ImageFilterConfig.Factor), nameof(ImageFilterConfig.Bias) }
            },
            {
                ImageFilters.Emboss,
                new HashSet<string> { nameof(ImageFilterConfig.Factor), nameof(ImageFilterConfig.Bias) }
            },
            {
                ImageFilters.Laplacian,
                new HashSet<string> { nameof(ImageFilterConfig.Factor), nameof(ImageFilterConfig.Bias) }
            },
            {
                ImageFilters.EdgeEnhance,
                new HashSet<string> { nameof(ImageFilterConfig.Factor), nameof(ImageFilterConfig.Bias) }
            },
            {
                ImageFilters.UnsharpMask,
                new HashSet<string> { nameof(ImageFilterConfig.Factor), nameof(ImageFilterConfig.Bias) }
            },
            { ImageFilters.AnisotropicKuwahara, new HashSet<string> { nameof(ImageFilterConfig.BaseWindowSize) } },
            { ImageFilters.SupersamplingAntialiasing, new HashSet<string> { nameof(ImageFilterConfig.Scale) } },
            { ImageFilters.PostProcessingAntialiasing, new HashSet<string> { nameof(ImageFilterConfig.Sigma) } },
            { ImageFilters.PencilSketchEffect, new HashSet<string> { nameof(ImageFilterConfig.Sigma) } }
            // Add other filters as necessary
        };

        /// <summary>
        ///     The texture property map
        ///     Mapping of textures to their used properties
        /// </summary>
        private static readonly Dictionary<TextureType, HashSet<string>> TexturePropertyMap = new()
        {
            {
                TextureType.Noise,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.MinValue),
                    nameof(TextureConfiguration.MaxValue),
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.UseSmoothNoise),
                    nameof(TextureConfiguration.UseTurbulence),
                    nameof(TextureConfiguration.TurbulenceSize)
                }
            },
            {
                TextureType.Clouds,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.MinValue),
                    nameof(TextureConfiguration.MaxValue),
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.TurbulenceSize)
                }
            },
            {
                TextureType.Marble,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.XPeriod),
                    nameof(TextureConfiguration.YPeriod),
                    nameof(TextureConfiguration.TurbulencePower),
                    nameof(TextureConfiguration.TurbulenceSize),
                    nameof(TextureConfiguration.BaseColor)
                }
            },
            {
                TextureType.Wave,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.XyPeriod),
                    nameof(TextureConfiguration.TurbulencePower),
                    nameof(TextureConfiguration.TurbulenceSize)
                }
            },
            {
                TextureType.Wood,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.Alpha),
                    nameof(TextureConfiguration.XyPeriod),
                    nameof(TextureConfiguration.TurbulencePower),
                    nameof(TextureConfiguration.TurbulenceSize),
                    nameof(TextureConfiguration.BaseColor)
                }
            },
            {
                TextureType.Crosshatch,
                new HashSet<string>
                {
                    nameof(TextureConfiguration.LineSpacing),
                    nameof(TextureConfiguration.LineColor),
                    nameof(TextureConfiguration.LineThickness),
                    nameof(TextureConfiguration.Angle1),
                    nameof(TextureConfiguration.Angle2)
                }
            }
            // Add other textures as necessary
        };

        /// <summary>
        ///     The sharpen filter
        /// </summary>
        internal static readonly double[,] SharpenFilter = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };

        /// <summary>
        ///     The gaussian blur
        /// </summary>
        internal static readonly double[,] GaussianBlur = { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };

        /// <summary>
        ///     The emboss filter
        /// </summary>
        internal static readonly double[,] EmbossFilter = { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };

        /// <summary>
        ///     The box blur
        /// </summary>
        internal static readonly double[,] BoxBlur = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };

        /// <summary>
        ///     The laplacian filter
        /// </summary>
        internal static readonly double[,] LaplacianFilter = { { 0, -1, 0 }, { -1, 4, -1 }, { 0, -1, 0 } };

        /// <summary>
        ///     The edge enhance
        /// </summary>
        internal static readonly double[,] EdgeEnhance = { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };

        /// <summary>
        ///     The unsharp mask
        /// </summary>
        internal static readonly double[,] UnsharpMask = { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };

        /// <summary>
        ///     The motion blur
        /// </summary>
        internal static readonly double[,] MotionBlur =
        {
            { 1, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 0, 0, 1 }
        };

        /// <summary>
        ///     The kernel 45 degrees
        ///     Defines directional edge detection kernel for crosshatching
        /// </summary>
        internal static readonly double[,] Kernel45Degrees = { { -1, -1, 2 }, { -1, 2, -1 }, { 2, -1, -1 } };

        /// <summary>
        ///     The kernel 135 degrees
        ///     Defines directional edge detection kernel for crosshatching
        /// </summary>
        internal static readonly double[,] Kernel135Degrees = { { 2, -1, -1 }, { -1, 2, -1 }, { -1, -1, 2 } };


        /// <summary>
        ///     The sobel x kernel
        /// </summary>
        internal static readonly int[,] SobelX = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };

        /// <summary>
        ///     The sobel y kernel
        /// </summary>
        internal static readonly int[,] SobelY = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

        /// <summary>
        ///     the color matrix needed to GrayScale an image
        ///     Source:
        ///     https://archive.ph/hzR2W
        ///     ColorMatrix:
        ///     | m11 m12 m13 m14 m15 |
        ///     | m21 m22 m23 m24 m25 |
        ///     | m31 m32 m33 m34 m35 |
        ///     | m41 m42 m43 m44 m45 |
        ///     | m51 m52 m53 m54 m55 |
        ///     translates to:
        ///     NewR = (m11 * R + m12 * G + m13 * B + m14 * A + m15)
        ///     NewG = (m21* R + m22* G + m23* B + m24* A + m25)
        ///     NewB = (m31* R + m32* G + m33* B + m34* A + m35)
        ///     NewA = (m41* R + m42* G + m43* B + m44* A + m45)
        /// </summary>
        internal static readonly ColorMatrix GrayScale = new(new[]
        {
            new[] { .3f, .3f, .3f, 0, 0 }, new[] { .59f, .59f, .59f, 0, 0 }, new[] { .11f, .11f, .11f, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     the color matrix needed to invert an image
        ///     Source:
        ///     https://archive.ph/hzR2W
        /// </summary>
        internal static readonly ColorMatrix Invert = new(new[]
        {
            new float[] { -1, 0, 0, 0, 0 }, new float[] { 0, -1, 0, 0, 0 }, new float[] { 0, 0, -1, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 }, new float[] { 1, 1, 1, 0, 1 }
        });

        /// <summary>
        ///     the color matrix needed to Sepia an image
        ///     Source:
        ///     https://archive.ph/hzR2W
        /// </summary>
        internal static readonly ColorMatrix Sepia = new(new[]
        {
            new[] { .393f, .349f, .272f, 0, 0 }, new[] { .769f, .686f, .534f, 0, 0 },
            new[] { 0.189f, 0.168f, 0.131f, 0, 0 }, new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     the color matrix needed to Color Swap an image to Polaroid
        ///     Source:
        ///     https://docs.rainmeter.net/tips/colormatrix-guide/
        /// </summary>
        internal static readonly ColorMatrix Polaroid = new(new[]
        {
            new[] { 1.438f, -0.062f, -0.062f, 0, 0 }, new[] { -0.122f, 1.378f, -0.122f, 0, 0 },
            new[] { 0.016f, -0.016f, 1.483f, 0, 0 }, new float[] { 0, 0, 0, 1, 0 },
            new[] { 0.03f, 0.05f, -0.02f, 0, 1 }
        });

        /// <summary>
        ///     the color matrix needed to Color Swap an image to BlackAndWhite
        ///     Source:
        ///     https://docs.rainmeter.net/tips/colormatrix-guide/
        /// </summary>
        internal static readonly ColorMatrix BlackAndWhite = new(new[]
        {
            new[] { 1.5f, 1.5f, 1.5f, 0, 0 }, new[] { 1.5f, 1.5f, 1.5f, 0, 0 }, new[] { 1.5f, 1.5f, 1.5f, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 }, new float[] { -1, -1, -1, 0, 1 }
        });

        /// <summary>
        ///     The brightness Filter
        ///     Adjusts the brightness of the image by scaling the color values.
        /// </summary>
        internal static readonly ColorMatrix Brightness = new(new[]
        {
            new[] { 1.2f, 0, 0, 0, 0 }, new[] { 0, 1.2f, 0, 0, 0 }, new[] { 0, 0, 1.2f, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     The contrast Filter
        ///     Adjusts the contrast of the image by scaling the differences between pixel values.
        /// </summary>
        internal static readonly ColorMatrix Contrast = new(new[]
        {
            new[] { 1.5f, 0, 0, 0, -0.2f }, new[] { 0, 1.5f, 0, 0, -0.2f }, new[] { 0, 0, 1.5f, 0, -0.2f },
            new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     The hue shift Filter
        ///     Shifts the hue of the image, effectively rotating the color wheel.
        /// </summary>
        internal static readonly ColorMatrix HueShift = new(new[]
        {
            new[] { 0.213f, 0.715f, 0.072f, 0, 0 }, new[] { 0.213f, 0.715f, 0.072f, 0, 0 },
            new[] { 0.213f, 0.715f, 0.072f, 0, 0 }, new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     The color balance Filter
        ///     Adjusts the balance of colors to emphasize or de-emphasize specific color channels.
        /// </summary>
        internal static readonly ColorMatrix ColorBalance = new(new[]
        {
            new[] { 1f, 0.2f, -0.2f, 0, 0 }, new[] { -0.2f, 1f, 0.2f, 0, 0 }, new[] { 0.2f, -0.2f, 1f, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     The vintage Filter
        ///     Applies a vintage effect by modifying the color matrix to mimic old photo tones.
        /// </summary>
        internal static readonly ColorMatrix Vintage = new(new[]
        {
            new[] { 0.393f, 0.349f, 0.272f, 0, 0 }, new[] { 0.769f, 0.686f, 0.534f, 0, 0 },
            new[] { 0.189f, 0.168f, 0.131f, 0, 0 }, new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
        });

        /// <summary>
        ///     Initializes the <see cref="ImageRegister" /> class.
        /// </summary>
        static ImageRegister()
        {
            // Initialize default Filter settings
            FilterSettings[ImageFilters.GaussianBlur] = new ImageFilterConfig { Factor = 1.0 / 16.0, Bias = 0.0 };
            FilterSettings[ImageFilters.BoxBlur] = new ImageFilterConfig { Factor = 1.0 / 9.0, Bias = 0.0 };
            FilterSettings[ImageFilters.MotionBlur] = new ImageFilterConfig { Factor = 1.0 / 5.0, Bias = 0.0 };
            FilterSettings[ImageFilters.Sharpen] =
                new ImageFilterConfig { Factor = 1.0, Bias = 0.0 }; // Assuming default values
            FilterSettings[ImageFilters.Emboss] =
                new ImageFilterConfig { Factor = 1.0, Bias = 0.0 }; // Assuming default values
            FilterSettings[ImageFilters.Laplacian] =
                new ImageFilterConfig { Factor = 1.0, Bias = 0.0 }; // Assuming default values
            FilterSettings[ImageFilters.EdgeEnhance] =
                new ImageFilterConfig { Factor = 1.0, Bias = 0.0 }; // Assuming default values
            FilterSettings[ImageFilters.UnsharpMask] =
                new ImageFilterConfig { Factor = 1.0, Bias = 0.0 }; // Assuming default values
            FilterSettings[ImageFilters.AnisotropicKuwahara] = new ImageFilterConfig { BaseWindowSize = 5 };
            FilterSettings[ImageFilters.SupersamplingAntialiasing] = new ImageFilterConfig { Scale = 1 };
            FilterSettings[ImageFilters.PostProcessingAntialiasing] = new ImageFilterConfig { Sigma = 1.0 };
            FilterSettings[ImageFilters.PencilSketchEffect] = new ImageFilterConfig { Sigma = 1.0 };
            // Add more default settings as needed

            // Initialize default Texture settings
            TextureSetting[TextureType.Noise] = new TextureConfiguration
            {
                MinValue = 0,
                MaxValue = 255,
                Alpha = 255,
                TurbulenceSize = 64,
                UseSmoothNoise = false,
                UseTurbulence = false
            };

            TextureSetting[TextureType.Clouds] = new TextureConfiguration
            {
                MinValue = 0, MaxValue = 255, Alpha = 255, TurbulenceSize = 64
            };

            TextureSetting[TextureType.Marble] = new TextureConfiguration
            {
                Alpha = 255,
                XPeriod = 5.0,
                YPeriod = 10.0,
                TurbulencePower = 5.0,
                TurbulenceSize = 32.0,
                BaseColor = Color.FromArgb(30, 10, 0)
            };

            TextureSetting[TextureType.Wave] = new TextureConfiguration
            {
                Alpha = 255, XyPeriod = 12.0, TurbulencePower = 0.1, TurbulenceSize = 32.0
            };

            TextureSetting[TextureType.Wood] = new TextureConfiguration
            {
                Alpha = 255,
                XyPeriod = 12.0,
                TurbulencePower = 0.1,
                TurbulenceSize = 32.0,
                BaseColor = Color.FromArgb(80, 30, 30)
            };

            TextureSetting[TextureType.Crosshatch] = new TextureConfiguration
            {
                LineSpacing = 2,
                LineColor = Color.Black,
                LineThickness = 1,
                Angle1 = 45.0f,
                Angle2 = 135.0f
            };

            // Add more default settings as needed
        }

        /// <summary>
        ///     The settings for our Filter
        /// </summary>
        public static ConcurrentDictionary<ImageFilters, ImageFilterConfig> FilterSettings { get; set; } = new();

        /// <summary>
        ///     The texture setting
        /// </summary>
        public static ConcurrentDictionary<TextureType, TextureConfiguration> TextureSetting { get; set; } = new();

        /// <summary>
        ///     Gets or sets the count of retries.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        internal static int Count { get; set; }

        /// <summary>
        ///     Gets the settings.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Return the current config</returns>
        public static ImageFilterConfig GetSettings(ImageFilters filter)
        {
            return FilterSettings.TryGetValue(filter, out var config) ? config : new ImageFilterConfig();
        }

        /// <summary>
        ///     Sets the settings.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="config">The configuration.</param>
        public static void SetSettings(ImageFilters filter, ImageFilterConfig config)
        {
            FilterSettings[filter] = config;
        }

        /// <summary>
        ///     Gets the available filters.
        /// </summary>
        /// <returns>List of available Filters</returns>
        public static IEnumerable<ImageFilters> GetAvailableFilters()
        {
            return FilterSettings.Keys;
        }

        /// <summary>
        ///     Gets the used properties.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>List of properties needed for our Filters</returns>
        public static HashSet<string> GetUsedProperties(ImageFilters filter)
        {
            return FilterPropertyMap.TryGetValue(filter, out var properties) ? properties : new HashSet<string>();
        }

        /// <summary>
        ///     Gets the settings.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Get the Setting based on Filter</returns>
        public static TextureConfiguration GetSettings(TextureType filter)
        {
            return TextureSetting.TryGetValue(filter, out var config) ? config : new TextureConfiguration();
        }

        /// <summary>
        ///     Sets the settings.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="config">The configuration.</param>
        public static void SetSettings(TextureType filter, TextureConfiguration config)
        {
            TextureSetting[filter] = config;
        }

        /// <summary>
        ///     Gets the used properties.
        ///     Method to get the used properties for a specific texture type
        /// </summary>
        /// <param name="textureType">Type of the texture.</param>
        /// <returns>List of properties needed for our Textures</returns>
        public static HashSet<string> GetUsedProperties(TextureType textureType)
        {
            return TexturePropertyMap.TryGetValue(textureType, out var properties)
                ? properties
                : new HashSet<string>();
        }
    }
}
