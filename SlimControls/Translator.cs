/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimControls/Translator.cs
 * PURPOSE:     Converts our static strings to the enum we need in the Imaging Library
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */


using System;
using CommonControls;
using Imaging;

namespace SlimControls
{
    /// <summary>
    ///     Converts string to enums
    /// </summary>
    public static class Translator
    {
        /// <summary>
        ///     Gets the tools from string.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>SelectionTools enum</returns>
        public static ImageZoomTools GetToolsFromString(string command)
        {
            return command switch
            {
                ViewGuiResources.MoveText => ImageZoomTools.Move,
                ViewGuiResources.RectangleText => ImageZoomTools.Rectangle,
                ViewGuiResources.EllipseText => ImageZoomTools.Ellipse,
                ViewGuiResources.FreeFormText => ImageZoomTools.FreeForm,
                _ => ImageZoomTools.Move
            };
        }

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
        /// Converts to image zoom tools.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns>Mostly move</returns>
        public static ImageZoomTools ConvertToImageZoomTools(ImageTools tool)
        {
            return tool switch
            {
                ImageTools.Move => ImageZoomTools.Move,
                ImageTools.Paint => ImageZoomTools.Trace,
                ImageTools.Erase => ImageZoomTools.Trace,
                ImageTools.ColorSelect => ImageZoomTools.Trace,
                _ => ImageZoomTools.Move
            };
        }

        /// <summary>
        ///     Gets the filter from string.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>ImageFilters enum</returns>
        public static ImageFilters GetFilterFromString(string command)
        {
            return command switch
            {
                ViewGuiResources.FilterNone => ImageFilters.None,
                ViewGuiResources.FilterGrayScale => ImageFilters.GrayScale,
                ViewGuiResources.FilterInvert => ImageFilters.Invert,
                ViewGuiResources.FilterSepia => ImageFilters.Sepia,
                ViewGuiResources.FilterBlackAndWhite => ImageFilters.BlackAndWhite,
                ViewGuiResources.FilterPolaroid => ImageFilters.Polaroid,
                ViewGuiResources.FilterContour => ImageFilters.Contour,
                ViewGuiResources.FilterBrightness => ImageFilters.Brightness,
                ViewGuiResources.FilterContrast => ImageFilters.Contrast,
                ViewGuiResources.FilterHueShift => ImageFilters.HueShift,
                ViewGuiResources.FilterColorBalance => ImageFilters.ColorBalance,
                ViewGuiResources.FilterVintage => ImageFilters.Vintage,
                ViewGuiResources.FilterSharpen => ImageFilters.Sharpen,
                ViewGuiResources.FilterGaussianBlur => ImageFilters.GaussianBlur,
                ViewGuiResources.FilterEmboss => ImageFilters.Emboss,
                ViewGuiResources.FilterBoxBlur => ImageFilters.BoxBlur,
                ViewGuiResources.FilterLaplacian => ImageFilters.Laplacian,
                ViewGuiResources.FilterEdgeEnhance => ImageFilters.EdgeEnhance,
                ViewGuiResources.FilterMotionBlur => ImageFilters.MotionBlur,
                ViewGuiResources.FilterUnsharpMask => ImageFilters.UnsharpMask,
                ViewGuiResources.FilterDifferenceOfGaussians => ImageFilters.DifferenceOfGaussians,
                ViewGuiResources.FilterCrosshatch => ImageFilters.Crosshatch,
                ViewGuiResources.FilterFloydSteinbergDithering => ImageFilters.FloydSteinbergDithering,
                ViewGuiResources.FilterAnisotropicKuwahara => ImageFilters.AnisotropicKuwahara,
                ViewGuiResources.FilterSupersamplingAntialiasing => ImageFilters.SupersamplingAntialiasing,
                ViewGuiResources.FilterPostProcessingAntialiasing => ImageFilters.PostProcessingAntialiasing,
                ViewGuiResources.FilterPencilSketchEffect => ImageFilters.PencilSketchEffect,
                _ => ImageFilters.None
            };
        }
    }
}