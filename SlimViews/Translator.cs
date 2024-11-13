/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/Translator.cs
 * PURPOSE:     Converts our static strings to the enum we need in the Imaging Library
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */


using CommonControls;
using Imaging;

namespace SlimViews
{
    /// <summary>
    /// Converts string to enums
    /// </summary>
    internal static class Translator
    {
        /// <summary>
        /// Gets the tools from string.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>SelectionTools enum</returns>
        internal static SelectionTools GetToolsFromString(string command)
        {
            return command switch
            {
                SlimViewerGuiResources.MoveText => SelectionTools.Move,
                SlimViewerGuiResources.RectangleText => SelectionTools.Rectangle,
                SlimViewerGuiResources.EllipseText => SelectionTools.Ellipse,
                SlimViewerGuiResources.FreeFormText => SelectionTools.FreeForm,
                _ => SelectionTools.Move,
            };
        }

        /// <summary>
        /// Gets the texture from string.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>TextureType enum</returns>
        internal static TextureType GetTextureFromString(string command)
        {
            return command switch
            {
                SlimViewerGuiResources.TextureNoise => TextureType.Noise,
                SlimViewerGuiResources.TextureClouds => TextureType.Clouds,
                SlimViewerGuiResources.TextureMarble => TextureType.Marble,
                SlimViewerGuiResources.TextureWood => TextureType.Wood,
                SlimViewerGuiResources.TextureWave => TextureType.Wave,
                SlimViewerGuiResources.TextureCrosshatch => TextureType.Crosshatch,
                SlimViewerGuiResources.TextureConcrete => TextureType.Concrete,
                SlimViewerGuiResources.TextureCanvas => TextureType.Canvas,
                _ => TextureType.Noise,
            };
        }

        /// <summary>
        /// Gets the filter from string.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>ImageFilters enum</returns>
        internal static ImageFilters GetFilterFromString(string command)
        {
            return command switch
            {
                SlimViewerGuiResources.FilterNone => ImageFilters.None,
                SlimViewerGuiResources.FilterGrayScale => ImageFilters.GrayScale,
                SlimViewerGuiResources.FilterInvert => ImageFilters.Invert,
                SlimViewerGuiResources.FilterSepia => ImageFilters.Sepia,
                SlimViewerGuiResources.FilterBlackAndWhite => ImageFilters.BlackAndWhite,
                SlimViewerGuiResources.FilterPolaroid => ImageFilters.Polaroid,
                SlimViewerGuiResources.FilterContour => ImageFilters.Contour,
                SlimViewerGuiResources.FilterBrightness => ImageFilters.Brightness,
                SlimViewerGuiResources.FilterContrast => ImageFilters.Contrast,
                SlimViewerGuiResources.FilterHueShift => ImageFilters.HueShift,
                SlimViewerGuiResources.FilterColorBalance => ImageFilters.ColorBalance,
                SlimViewerGuiResources.FilterVintage => ImageFilters.Vintage,
                SlimViewerGuiResources.FilterSharpen => ImageFilters.Sharpen,
                SlimViewerGuiResources.FilterGaussianBlur => ImageFilters.GaussianBlur,
                SlimViewerGuiResources.FilterEmboss => ImageFilters.Emboss,
                SlimViewerGuiResources.FilterBoxBlur => ImageFilters.BoxBlur,
                SlimViewerGuiResources.FilterLaplacian => ImageFilters.Laplacian,
                SlimViewerGuiResources.FilterEdgeEnhance => ImageFilters.EdgeEnhance,
                SlimViewerGuiResources.FilterMotionBlur => ImageFilters.MotionBlur,
                SlimViewerGuiResources.FilterUnsharpMask => ImageFilters.UnsharpMask,
                SlimViewerGuiResources.FilterDifferenceOfGaussians => ImageFilters.DifferenceOfGaussians,
                SlimViewerGuiResources.FilterCrosshatch => ImageFilters.Crosshatch,
                SlimViewerGuiResources.FilterFloydSteinbergDithering => ImageFilters.FloydSteinbergDithering,
                SlimViewerGuiResources.FilterAnisotropicKuwahara => ImageFilters.AnisotropicKuwahara,
                SlimViewerGuiResources.FilterSupersamplingAntialiasing => ImageFilters.SupersamplingAntialiasing,
                SlimViewerGuiResources.FilterPostProcessingAntialiasing => ImageFilters.PostProcessingAntialiasing,
                SlimViewerGuiResources.FilterPencilSketchEffect => ImageFilters.PencilSketchEffect,
                _ => ImageFilters.None,
            };
        }
    }
}
