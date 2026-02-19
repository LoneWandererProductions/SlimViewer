/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        ViewGuiResources.cs
 * PURPOSE:     String constants for UI commands and binding parameters
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace SlimControls
{
    /// <summary>
    /// Contains string constants used across the UI for CommandParameters to avoid magic strings.
    /// Used directly in XAML via the {x:Static} markup extension.
    /// </summary>
    public static class ViewGuiResources
    {
        // --- Selection Tool Strings ---
        public const string RectangleText = "RectangleText";
        public const string EllipseText = "EllipseText";
        public const string FreeFormText = "FreeFormText";

        // --- Texture Strings ---
        public const string TextureNoise = "TextureNoise";
        public const string TextureClouds = "TextureClouds";
        public const string TextureMarble = "TextureMarble";
        public const string TextureWood = "TextureWood";
        public const string TextureWave = "TextureWave";
        public const string TextureCrosshatch = "TextureCrosshatch";
        public const string TextureConcrete = "TextureConcrete";
        public const string TextureCanvas = "TextureCanvas";

        // --- Filter Strings ---
        public const string FilterNone = "FilterNone";
        public const string FilterGrayScale = "FilterGrayScale";
        public const string FilterInvert = "FilterInvert";
        public const string FilterSepia = "FilterSepia";
        public const string FilterBlackAndWhite = "FilterBlackAndWhite";
        public const string FilterPolaroid = "FilterPolaroid";
        public const string FilterContour = "FilterContour";
        public const string FilterBrightness = "FilterBrightness";
        public const string FilterContrast = "FilterContrast";
        public const string FilterHueShift = "FilterHueShift";
        public const string FilterColorBalance = "FilterColorBalance";
        public const string FilterVintage = "FilterVintage";
        public const string FilterSharpen = "FilterSharpen";
        public const string FilterGaussianBlur = "FilterGaussianBlur";
        public const string FilterEmboss = "FilterEmboss";
        public const string FilterBoxBlur = "FilterBoxBlur";
        public const string FilterLaplacian = "FilterLaplacian";
        public const string FilterEdgeEnhance = "FilterEdgeEnhance";
        public const string FilterMotionBlur = "FilterMotionBlur";
        public const string FilterUnsharpMask = "FilterUnsharpMask";
        public const string FilterDifferenceOfGaussians = "FilterDifferenceOfGaussians";
        public const string FilterCrosshatch = "FilterCrosshatch";
        public const string FilterFloydSteinbergDithering = "FilterFloydSteinbergDithering";
        public const string FilterAnisotropicKuwahara = "FilterAnisotropicKuwahara";
        public const string FilterSupersamplingAntialiasing = "FilterSupersamplingAntialiasing";
        public const string FilterPostProcessingAntialiasing = "FilterPostProcessingAntialiasing";
        public const string FilterPencilSketchEffect = "FilterPencilSketchEffect";
    }
}