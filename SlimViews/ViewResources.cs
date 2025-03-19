/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/ViewResources.cs
 * PURPOSE:     Resources for the SlimViewer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Windows.Media.Imaging;
using Imaging;

namespace SlimViews
{
    /// <summary>
    ///     SlimViewer Resources
    /// </summary>
    internal static class ViewResources
    {
        /// <summary>
        ///     The file open Options (const). Value: "Image Files(*.png)|*.png|Image Files(*.jpg)|*.jpg|Image
        ///     Files(*.Bmp)|*.Bmp|Image Files(*.gif)|*.gif|Image Files(*.tif)|*.tif|All files (*.*)|*.*".
        /// </summary>
        internal const string FileOpen =
            "Image Files(*.jpg)|*.jpg|Image Files(*.png)|*.png|Image Files(*.Bmp)|*.Bmp|Image Files(*.gif)|*.gif|Image Files(*.tif)|*.tif|All files (*.*)|*.*";

        /// <summary>
        ///     The file open CBR (const). Value: "Comic Book File (*.cbz)|*.cbz".
        /// </summary>
        internal const string FileOpenCbz =
            "Comic Book File (*.cbz)|*.cbz";

        /// <summary>
        ///     The file open txt (const). Value: "Text File (*.txt)|*.txt".
        /// </summary>
        internal const string FileOpenTxt =
            "Text File (*.txt)|*.txt";

        /// <summary>
        ///     The file open gif (const). Value: "Gif File (*.cbz)|*.cbz".
        /// </summary>
        internal const string FileOpenGif =
            "Gif File (*.gif)|*.gif";

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
        ///     The Display Images (const). Value: "New Images loaded: "
        /// </summary>
        internal const string DisplayImages = "New Images loaded: ";


        /// <summary>
        ///     The Message Box Message (const). Value: "Shall we overwrite the file?."
        /// </summary>
        internal const string MessageFileAlreadyExists = "Shall we overwrite the file?";

        /// <summary>
        ///     The Message Box Header (const). Value: "File already exists"
        /// </summary>
        internal const string CaptionFileAlreadyExists = "File already exists";

        /// <summary>
        ///     The Icon Path Green (const). Value: @"System\green.png"
        /// </summary>
        internal const string IconPathGreen = @"System\green.png";

        /// <summary>
        ///     The Icon Path Red (const). Value:  @"System\red.png"
        /// </summary>
        internal const string IconPathRed = @"System\red.png";

        /// <summary>
        ///     The Cbz Extension (const). Value: ".cbz"
        /// </summary>
        internal const string CbzExt = ".cbz";

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
        ///     The Information Converted (const). Value: "Converted: "
        /// </summary>
        internal const string InformationConverted = "Converted: ";

        /// <summary>
        ///     The Information Converted (const). Value: "Errors: "
        /// </summary>
        internal const string InformationErrors = "Errors: ";

        /// <summary>
        ///     The Information Color (const). Value: "Color: "
        /// </summary>
        internal const string InformationColor = "Color: ";

        /// <summary>
        ///     The Information Count fo Colors (const). Value: " count: "
        /// </summary>
        internal const string InformationCount = " count: ";

        /// <summary>
        ///     The error could not save file (const). Value: "Could not Save the File."
        /// </summary>
        internal const string ErrorCouldNotSaveFile = "Could not Save the File";

        /// <summary>
        ///     The error with the Serializer (const). Value: "Could not Serialize: ".
        /// </summary>
        internal const string ErrorSerializer = "Could not Serialize: ";

        /// <summary>
        ///     The error with Values that are not positive (const). Value: "Value must be positive.".
        /// </summary>
        internal const string ErrorMeasures = "Value must be positive: ";

        /// <summary>
        ///     The Error Image not supported (const). Value: " "Unsupported image format."
        /// </summary>
        internal const string ErrorNotSupported = "Unsupported image format.";

        /// <summary>
        ///     The MessageBox caption Error (const). Value: "Error"
        /// </summary>
        internal const string ErrorMessage = "Error: ";

        /// <summary>
        ///     The error File not Found Text (const). Value: "File not found: "
        /// </summary>
        internal const string ErrorFileNotFoundMessage = "File not found: ";

        /// <summary>
        ///     The error source (const). Value:  "Error Source: "
        /// </summary>
        internal const string ErrorSourceMessage = "Error Source: ";

        /// <summary>
        ///     The directory error message (const). Value: "Error Dictionary not found.".
        /// </summary>
        internal const string ErrorDirectoryMessage = "Error Dictionary not found.";

        /// <summary>
        ///     The object error message (const). Value:  "Error Source: "
        /// </summary>
        internal const string ErrorObjectMessage = "Error with the loaded object.";

        /// <summary>
        ///     The error title (const). Value:  "Error Source: "
        /// </summary>
        internal const string ErrorTitle = "An error occured.";

        /// <summary>
        ///     The error Message extension not supported (const). Value: "Extension not yet supported: "
        /// </summary>
        internal const string ErrorFileNotSupported = "Extension not yet supported: ";

        /// <summary>
        ///     The Status Compare started (const). Value: "Compare started."
        /// </summary>
        internal const string StatusCompareStart = "Compare started.";

        /// <summary>
        ///     The Status Compare finished (const). Value: "Compare finished."
        /// </summary>
        internal const string StatusCompareFinished = "Compare finished.";

        /// <summary>
        ///     The New Gif file Name. (const). Value: "NewGif.gif"
        /// </summary>
        internal const string NewGif = "NewGif.gif";

        /// <summary>
        ///     The Images Path (const). Value: "Images"
        /// </summary>
        internal const string ImagesPath = "Images";

        /// <summary>
        ///     The New Gif file Name.(const).  Value: "NewGif"
        /// </summary>
        internal const string NewGifPath = "NewGif";

        /// <summary>
        ///     The similarity.(const).  Value: "Similarity: "
        /// </summary>
        internal const string Similarity = "Similarity: ";

        /// <summary>
        ///     The Information Warning Message (const). Value: " Warning: "
        /// </summary>
        internal const string MessageInformation = " Warning";

        /// <summary>
        ///     The Message for the 200 Image limit (const). Value: " Right now we set a limit of 200 Images."
        /// </summary>
        internal const string MessageFiles = "Right now we set a limit of 200 Images.";

        /// <summary>
        ///     The Image Name (const). Value: "Name: "
        /// </summary>
        private const string ImageName = ", Name: ";

        /// <summary>
        ///     The Image Size (const). Value: " , Size: "
        /// </summary>
        private const string ImageSize = " , Size: ";

        /// <summary>
        ///     The gif Frames (const). Value: " , Frames: "
        /// </summary>
        private const string Frames = " , Frames: ";

        /// <summary>
        ///     The Image Height (const). Value: "Bytes, Height: "
        /// </summary>
        private const string ImageHeight = " , Height: ";

        /// <summary>
        ///     The Image Width (const). Value: " , Width: "
        /// </summary>
        private const string ImageWidth = " , Width: ";

        /// <summary>
        ///     The Image Path (const). Value: " Path: "
        /// </summary>
        private const string ImagePath = " Path: ";

        /// <summary>
        ///     Builds the gif Image information.
        /// </summary>
        /// <param name="gifPath">The GIF path.</param>
        /// <param name="info">The information.</param>
        /// <returns>String of Image Information</returns>
        internal static string BuildGifInformation(string gifPath, ImageGifInfo info)
        {
            return string.Concat(gifPath, ImageName,
                info.Name, ImageHeight, info.Height, ImageWidth,
                info.Width,
                ImageSize, info.Size, Frames.Length, info.Frames);
        }

        /// <summary>
        ///     Builds the image information.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="bmp">The BMP.</param>
        /// <returns>String of Image Information</returns>
        internal static string BuildImageInformation(string filePath, string fileName, BitmapImage bmp)
        {
            return string.Concat(ImagePath, filePath, ImageName,
                fileName, ImageHeight, bmp.Height, ImageWidth,
                bmp.Width,
                ImageSize, bmp.Height * bmp.Width);
        }

        /// <summary>
        ///     Builds the image information with line breaks.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="bmp">The BMP.</param>
        /// <returns>String of Image Information with line breaks.</returns>
        public static string BuildImageInformationLine(string filePath, string fileName, BitmapImage bmp)
        {
            return string.Concat(ImagePath, filePath, Environment.NewLine,
                ImageName, fileName, Environment.NewLine,
                ImageHeight, bmp.Height, Environment.NewLine,
                ImageWidth, bmp.Width, Environment.NewLine,
                ImageSize, bmp.Height * bmp.Width, Environment.NewLine);
        }
    }
}