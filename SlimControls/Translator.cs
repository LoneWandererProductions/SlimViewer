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
        internal static ImageZoomTools GetToolsFromString(string command)
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
        ///     Maps an ImageTools enum value to an integer code.
        /// </summary>
        internal static EnumTools MapToolToEnumTools(ImageTools tool)
        {
            return tool switch
            {
                ImageTools.Paint => EnumTools.Paint,
                ImageTools.Erase => EnumTools.Erase,
                ImageTools.Area => EnumTools.Area,
                _ => EnumTools.Move // Default value for unknown tools
            };
        }

        /// <summary>
        ///     Maps the code to tool.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public static MaskShape MapCodeToTool(ImageZoomTools code)
        {
            return code switch
            {
                ImageZoomTools.Rectangle => MaskShape.Rectangle,
                ImageZoomTools.Ellipse => MaskShape.Circle,
                ImageZoomTools.FreeForm => MaskShape.Polygon,
                _ => MaskShape.Rectangle
            };
        }

        /// <summary>
        ///     Gets the fill tool.
        /// </summary>
        /// <param name="enumString">The enum string.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Invalid enum string: {enumString} - enumString</exception>
        public static EnumTools GetFillTool(string enumString)
        {
            // Attempt to parse the string to an EnumTools value
            if (Enum.TryParse(enumString, out EnumTools result)) return result;

            // If parsing fails, you could either throw an exception, return a default value, or handle it however you'd like.
            return EnumTools.SolidColor;
        }

        /// <summary>
        ///     Maps the enum tools to tool.
        /// </summary>
        /// <param name="toolCode">The tool code.</param>
        /// <returns></returns>
        public static ImageTools MapEnumToolsToTool(EnumTools toolCode)
        {
            return toolCode switch
            {
                EnumTools.Area => ImageTools.Area,
                EnumTools.ColorSelect => ImageTools.ColorSelect,
                EnumTools.Paint => ImageTools.Paint,
                EnumTools.Erase => ImageTools.Erase,
                EnumTools.Move => ImageTools.Move,
                EnumTools.SolidColor => ImageTools.Area,
                EnumTools.Texture => ImageTools.Area,
                EnumTools.Filter => ImageTools.Area
            };
        }

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