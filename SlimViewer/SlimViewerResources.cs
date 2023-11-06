/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViewer/SlimViewerResources.cs
 * PURPOSE:     Resources for the SlimViewer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;

namespace SlimViewer
{
    /// <summary>
    ///     SlimViewer Resources
    /// </summary>
    internal static class SlimViewerResources
    {
        /// <summary>
        ///     The file open Options (const). Value: "Image Files(*.png)|*.png|Image Files(*.jpg)|*.jpg|Image
        ///     Files(*.Bmp)|*.Bmp|Image Files(*.gif)|*.gif|Image Files(*.tif)|*.tif|All files (*.*)|*.*".
        /// </summary>
        internal const string FileOpen =
            "Image Files(*.png)|*.png|Image Files(*.jpg)|*.jpg|Image Files(*.Bmp)|*.Bmp|Image Files(*.gif)|*.gif|Image Files(*.tif)|*.tif|All files (*.*)|*.*";

        /// <summary>
        ///     The file open CBR (const). Value: "Comic Book File (*.cbz)|*.cbz".
        /// </summary>
        internal const string FileOpenCbz =
            "Comic Book File (*.cbz)|*.cbz";

        /// <summary>
        ///     The file open cif (const). Value: "Custom image File (*.cif)|*.cif".
        /// </summary>
        internal const string FileOpenCif =
            "Custom image File (*.cif)|*.cif";

        /// <summary>
        ///     The Temp Folder (const). Value: "Temp".
        /// </summary>
        internal const string TempFolder = "Temp";

        /// <summary>
        ///     The Explorer (const). Value: "Explorer.exe".
        /// </summary>
        internal const string Explorer = "Explorer.exe";

        /// <summary>
        ///     The Select (const). Value: "/select, \"".
        /// </summary>
        internal const string Select = "/select, \"";

        /// <summary>
        ///     The Close (const). Value: "\"".
        /// </summary>
        internal const string Close = "\"";

        /// <summary>
        ///     The Status Done (const). Value: "Done"
        /// </summary>
        internal const string StatusDone = "Done";

        /// <summary>
        ///     The Caption Done (const). Value: "Cleaning done"
        /// </summary>
        internal const string CaptionDone = "Cleaning done";

        /// <summary>
        ///     The Information Converted (const). Value: "Converted: "
        /// </summary>
        internal const string InformationConverted = "Converted: ";

        /// <summary>
        ///     The Display Images (const). Value: "New Images loaded: "
        /// </summary>
        internal const string DisplayImages = "New Images loaded: ";

        /// <summary>
        ///     The Information Converted (const). Value: "Errors: "
        /// </summary>
        internal const string InformationErrors = "Errors: ";

        /// <summary>
        ///     The error could not save file (const). Value: "Could not Save the File."
        /// </summary>
        internal const string ErrorCouldNotSaveFile = "Could not Save the File";

        /// <summary>
        ///     The Message Box Message (const). Value: "Shall we overwrite the file?."
        /// </summary>
        internal const string MessageFileAlreadyExists = "Shall we overwrite the file?";

        /// <summary>
        ///     The Message Box Header (const). Value: "File already exists"
        /// </summary>
        internal const string CaptionFileAlreadyExists = "File already exists";

        /// <summary>
        ///     The Image Name (const). Value: "Name: "
        /// </summary>
        internal const string ImageName = ", Name: ";

        /// <summary>
        ///     The Image Size (const). Value: " , Size: "
        /// </summary>
        internal const string ImageSize = " , Size: ";

        /// <summary>
        ///     The gif Frames (const). Value: " , Frames: "
        /// </summary>
        internal const string Frames = " , Frames: ";

        /// <summary>
        ///     The Image Height (const). Value: "Bytes, Height: "
        /// </summary>
        internal const string ImageHeight = " , Height: ";

        /// <summary>
        ///     The Image Width (const). Value: " , Width: "
        /// </summary>
        internal const string ImageWidth = " , Width: ";

        /// <summary>
        ///     The Image Path (const). Value: " Path: "
        /// </summary>
        internal const string ImagePath = " Path: ";

        /// <summary>
        ///     The Icon Path Green (const). Value: @"System\green.png"
        /// </summary>
        internal const string IconPathGreen = @"System\green.png";

        /// <summary>
        ///     The Icon Path Red (const). Value:  @"System\red.png"
        /// </summary>
        internal const string IconPathRed = @"System\red.png";

        /// <summary>
        ///     The jpg Extension (const). Value: ".jpg"
        /// </summary>
        internal const string JpgExt = ".jpg";

        /// <summary>
        ///     The jpg Extension Alt (const). Value:".jpeg"
        /// </summary>
        internal const string JpgExtAlt = ".jpeg";

        /// <summary>
        ///     The png Extension (const). Value: ".png"
        /// </summary>
        internal const string PngExt = ".png";

        /// <summary>
        ///     The Bmp Extension (const). Value: ".Bmp"
        /// </summary>
        internal const string BmpExt = ".Bmp";

        /// <summary>
        ///     The Gif Extension (const). Value: ".gif"
        /// </summary>
        internal const string GifExt = ".gif";

        /// <summary>
        ///     The Tif Extension (const). Value: ".tif"
        /// </summary>
        internal const string TifExt = ".tif";

        /// <summary>
        ///     The Cbz Extension (const). Value: ".cbz"
        /// </summary>
        internal const string CbzExt = ".cbz";

        /// <summary>
        ///     The MessageBox caption Error (const). Value: "Error"
        /// </summary>
        internal const string MessageError = "Error";

        /// <summary>
        ///     The MessageBox caption Success (const). Value: "Success"
        /// </summary>
        internal const string MessageSuccess = "Success";

        /// <summary>
        ///     The MessageBox Count Text (const). Value: "Files deleted: "
        /// </summary>
        internal const string MessageCount = "Files deleted: ";

        /// <summary>
        ///     The MessageBox Moved Text (const). Value: "Files moved: "
        /// </summary>
        internal const string MessageMoved = "Files moved: ";

        /// <summary>
        ///     The Status Compare started (const). Value: "Compare started."
        /// </summary>
        internal const string StatusCompareStart = "Compare started.";

        /// <summary>
        ///     The Status Compare finished (const). Value: "Compare finished."
        /// </summary>
        internal const string StatusCompareFinished = "Compare finished.";

        /// <summary>
        ///     The File Appendix
        /// </summary>
        internal static readonly List<string> Appendix = new()
        {
            JpgExt,
            PngExt,
            BmpExt,
            GifExt,
            TifExt
        };
    }
}