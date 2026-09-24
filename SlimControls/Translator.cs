/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimControls/Translator.cs
 * PURPOSE:     Converts our static strings to the enum we need in the Imaging Library
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using Common.Images;
using Imaging.Enums;

namespace SlimControls
{
    /// <summary>
    ///     Converts string to enums
    /// </summary>
    public static class Translator
    {
        /// <summary>
        ///     Gets the texture from string.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>TextureType enum</returns>
        public static TextureType GetTextureFromString(string command)
        {
            return command switch
            {
                ViewGuiResources.TextureNoise => TextureType.Noise,
                ViewGuiResources.TextureClouds => TextureType.Clouds,
                ViewGuiResources.TextureMarble => TextureType.Marble,
                ViewGuiResources.TextureWood => TextureType.Wood,
                ViewGuiResources.TextureWave => TextureType.Wave,
                ViewGuiResources.TextureCrosshatch => TextureType.Crosshatch,
                ViewGuiResources.TextureConcrete => TextureType.Concrete,
                ViewGuiResources.TextureCanvas => TextureType.Canvas,
                _ => TextureType.Noise
            };
        }

        /// <summary>
        ///     Maps the code to tool.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        /// <remarks>
        ///     Delegates to <see cref="GestureCatalog"/> rather than maintaining its
        ///     own switch: a gesture's mask shape used to be decided independently
        ///     here and in <see cref="Common.Images.ImageZoom"/>'s mouse handling,
        ///     and the two could disagree (as happened with the Polygon tool, which
        ///     fell back to <see cref="MaskShape.Rectangle"/> here even once it drew
        ///     correctly on screen). There is now exactly one place that decides it.
        /// </remarks>
        public static MaskShape MapCodeToTool(ImageZoomTools code) => GestureCatalog.MaskShapeFor(code);

        /// <summary>
        ///     Gets the filter from string.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>ImageFilters enum</returns>
        public static FiltersType GetFilterFromString(string command)
        {
            return command switch
            {
                ViewGuiResources.FilterNone => FiltersType.None,
                ViewGuiResources.FilterGrayScale => FiltersType.GrayScale,
                ViewGuiResources.FilterInvert => FiltersType.Invert,
                ViewGuiResources.FilterSepia => FiltersType.Sepia,
                ViewGuiResources.FilterBlackAndWhite => FiltersType.BlackAndWhite,
                ViewGuiResources.FilterPolaroid => FiltersType.Polaroid,
                ViewGuiResources.FilterContour => FiltersType.Contour,
                ViewGuiResources.FilterBrightness => FiltersType.Brightness,
                ViewGuiResources.FilterContrast => FiltersType.Contrast,
                ViewGuiResources.FilterHueShift => FiltersType.HueShift,
                ViewGuiResources.FilterColorBalance => FiltersType.ColorBalance,
                ViewGuiResources.FilterVintage => FiltersType.Vintage,
                ViewGuiResources.FilterSharpen => FiltersType.Sharpen,
                ViewGuiResources.FilterGaussianBlur => FiltersType.GaussianBlur,
                ViewGuiResources.FilterEmboss => FiltersType.Emboss,
                ViewGuiResources.FilterBoxBlur => FiltersType.BoxBlur,
                ViewGuiResources.FilterLaplacian => FiltersType.Laplacian,
                ViewGuiResources.FilterEdgeEnhance => FiltersType.EdgeEnhance,
                ViewGuiResources.FilterMotionBlur => FiltersType.MotionBlur,
                ViewGuiResources.FilterUnsharpMask => FiltersType.UnsharpMask,
                ViewGuiResources.FilterDifferenceOfGaussians => FiltersType.DifferenceOfGaussians,
                ViewGuiResources.FilterCrosshatch => FiltersType.Crosshatch,
                ViewGuiResources.FilterFloydSteinbergDithering => FiltersType.FloydSteinbergDithering,
                ViewGuiResources.FilterAnisotropicKuwahara => FiltersType.AnisotropicKuwahara,
                ViewGuiResources.FilterSupersamplingAntialiasing => FiltersType.SupersamplingAntialiasing,
                ViewGuiResources.FilterPostProcessingAntialiasing => FiltersType.PostProcessingAntialiasing,
                ViewGuiResources.FilterPencilSketchEffect => FiltersType.PencilSketchEffect,
                _ => FiltersType.None
            };
        }
    }
}