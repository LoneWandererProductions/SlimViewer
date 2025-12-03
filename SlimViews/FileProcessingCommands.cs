/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        FileProcessingCommands.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using CommonDialogs;
using FileHandler;
using Imaging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace SlimViews
{
    /// <summary>
    ///     Provides command-based operations for managing and manipulating image files,
    ///     including delete, move, rename, conversion, and save operations.
    /// </summary>
    internal class FileProcessingCommands
    {
        /// <summary>
        ///     Cleans the temporary folder used by the application.
        /// </summary>
        /// <param name="obj">If <c>true</c>, suppresses user confirmation messages.</param>
        internal void CleanTempFolder(bool? obj)
        {
            var silent = obj == true;
            var root = Path.Combine(Directory.GetCurrentDirectory(), ViewResources.TempFolder);

            try
            {
                _ = FileHandleDelete.DeleteAllContents(root);
            }
            catch (FileHandlerException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    $"{ViewResources.ErrorMessage}{nameof(CleanTempFolder)}");
                return;
            }

            if (!silent)
            {
                _ = MessageBox.Show(ViewResources.StatusDone, ViewResources.CaptionDone,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        ///     Converts the currently loaded image to a CIF file or from CIF to bitmap,
        ///     depending on the owner's configuration.
        /// </summary>
        /// <param name="owner">The image view that owns the image.</param>
        /// <param name="obj">Unused parameter (reserved for future use).</param>
        internal void ConvertCif(ImageView owner, object obj)
        {
            if (owner?.Image?.Bitmap == null)
                return;

            var pathObj = DialogHandler.HandleFileOpen(ViewResources.FileOpen, owner.FileContext.CurrentPath);
            if (pathObj == null || !File.Exists(pathObj.FilePath))
                return;

            try
            {
                if (owner.CompressCif)
                    owner.Image.CustomImageFormat.GenerateCifCompressedFromBitmap(owner.Image.Bitmap, pathObj.FilePath);
                else
                    owner.Image.CustomImageFormat.GenerateBitmapToCifFile(owner.Image.Bitmap, pathObj.FilePath);
            }
            catch (FileHandlerException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    $"{ViewResources.ErrorMessage}{nameof(ConvertCif)}");
            }
        }

        /// <summary>
        ///     Deletes one or more images depending on the current selection.
        /// </summary>
        /// <param name="owner">The image view owner.</param>
        /// <param name="obj">Unused parameter (reserved for interface compatibility).</param>
        internal async Task DeleteAsync(ImageView owner, object obj)
        {
            if (owner == null || owner.UiState == null)
                return;

            // nothing to do if no current item and no selection
            if (!owner.Observer.ContainsKey(owner.FileContext.CurrentId) && owner.UiState.IsSelectionEmpty)
                return;

            var idsToDelete = owner.UiState.IsSelectionEmpty
                ? new[] { owner.FileContext.CurrentId }
                : owner.UiState.Thumb.Selection.Keys.ToArray();

            var deletedCount = 0;

            foreach (var id in idsToDelete)
            {
                try
                {
                    if (!owner.Observer.TryGetValue(id, out var filePath))
                        continue;

                    var deleted = await FileHandleSafeDelete.DeleteFile(filePath);
                    if (deleted)
                    {
                        deletedCount++;
                        if (owner.Count > 0)
                            owner.Count--;
                    }
                }
                catch (FileHandlerException ex)
                {
                    Trace.WriteLine(ex);
                    _ = MessageBox.Show(ex.ToString(),
                        $"{ViewResources.ErrorMessage}{nameof(DeleteAsync)}");
                }
            }

            // Refresh or clear view depending on context
            if (!owner.UiState.IsSelectionEmpty)
            {
                owner.LoadThumbs(owner.FileContext.CurrentPath);
                _ = MessageBox.Show($"{ViewResources.MessageCount}{deletedCount}",
                    ViewResources.MessageSuccess, MessageBoxButton.OK);
            }
            else
            {
                owner.Bmp = null;
                owner.Image.Bitmap = null;
                owner.GifPath = null;
                owner.FileContext.GifPath = null;

                owner.UiState.Thumb.RemoveSingleItem(owner.FileContext.CurrentId);
                owner.NextAction(this);
            }
        }

        /// <summary>
        ///     Moves selected or current images to a new folder.
        /// </summary>
        /// <param name="owner">The image view owner.</param>
        /// <param name="obj">Unused parameter (reserved for interface compatibility).</param>
        internal void Move(ImageView owner, object obj)
        {
            if (owner == null || (!File.Exists(owner.FileName) && owner.UiState.IsSelectionEmpty))
                return;

            if (string.IsNullOrEmpty(owner.FileContext.CurrentPath))
                owner.FileContext.CurrentPath = owner.UiState.Root;

            var targetDir = DialogHandler.ShowFolder(
                owner.FileContext.CurrentPath ?? Directory.GetCurrentDirectory());
            if (string.IsNullOrEmpty(targetDir) || !Directory.Exists(targetDir))
                return;

            var fileIds = owner.UiState.IsSelectionEmpty
                ? new[] { owner.FileContext.CurrentId }
                : owner.UiState.Thumb.Selection.Keys.ToArray();

            var movedCount = 0;

            foreach (var id in fileIds)
            {
                if (!owner.Observer.TryGetValue(id, out var sourcePath) || !File.Exists(sourcePath))
                    continue;

                var fileName = Path.GetFileName(sourcePath);
                var destPath = Path.Combine(targetDir, fileName);

                if (File.Exists(destPath))
                {
                    var dialogResult = MessageBox.Show(
                        ViewResources.MessageFileAlreadyExists,
                        ViewResources.CaptionFileAlreadyExists,
                        MessageBoxButton.YesNo);

                    if (dialogResult == MessageBoxResult.No)
                        continue;
                }

                try
                {
                    new FileInfo(sourcePath).MoveTo(destPath, true);
                    movedCount++;
                }
                catch (IOException ioEx)
                {
                    Trace.WriteLine(ioEx);
                    _ = MessageBox.Show(ioEx.ToString(),
                        $"{ViewResources.ErrorMessage}{nameof(Move)}");
                }
            }

            if (!owner.UiState.IsSelectionEmpty)
            {
                _ = MessageBox.Show($"{ViewResources.MessageMoved}{movedCount}",
                    ViewResources.MessageSuccess, MessageBoxButton.OK);
            }
        }

        /// <summary>
        ///     Moves all files from the current folder to a selected target folder.
        /// </summary>
        /// <param name="owner">The image view owner.</param>
        /// <param name="obj">Unused parameter (reserved for interface compatibility).</param>
        internal void MoveAll(ImageView owner, object obj)
        {
            if (owner == null)
                return;

            if (string.IsNullOrEmpty(owner.FileContext.CurrentPath))
                owner.FileContext.CurrentPath = Path.GetDirectoryName(owner.UiState.Root);

            if (owner.FileContext.IsFilesEmpty)
                return;

            var targetDir = DialogHandler.ShowFolder(
                owner.FileContext.CurrentPath ?? Directory.GetCurrentDirectory());
            if (string.IsNullOrEmpty(targetDir) || !Directory.Exists(targetDir))
                return;

            var targetFiles = FileHandleSearch.GetFilesByExtensionFullPath(
                targetDir, ImagingResources.Appendix, owner.UiState.UseSubFolders
            ) ?? Enumerable.Empty<string>();

            var sourceFiles = owner.FileContext.Files;

            if (sourceFiles.Intersect(targetFiles).Any())
            {
                var dialogResult = MessageBox.Show(
                    ViewResources.MessageFileAlreadyExists,
                    ViewResources.CaptionFileAlreadyExists,
                    MessageBoxButton.YesNo);

                if (dialogResult == MessageBoxResult.No)
                    return;
            }

            _ = FileHandleCut.CutFiles(sourceFiles, targetDir, false);
        }

        /// <summary>
        ///     Renames the current image file.
        /// </summary>
        /// <param name="owner">The image view owner.</param>
        /// <param name="obj">Unused parameter (reserved for interface compatibility).</param>
        internal async Task Rename(ImageView owner, object obj)
        {
            if (owner == null || !owner.IsImageActive)
                return;

            if (!owner.Observer.TryGetValue(owner.FileContext.CurrentId, out string? file) || !File.Exists(file))
                return;

            var folder = Path.GetDirectoryName(file);
            if (string.IsNullOrEmpty(folder))
                return;

            var newFilePath = Path.Combine(folder, owner.FileName);

            if (File.Exists(newFilePath))
            {
                var dialogResult = MessageBox.Show(
                    ViewResources.MessageFileAlreadyExists,
                    ViewResources.CaptionFileAlreadyExists,
                    MessageBoxButton.YesNo);

                if (dialogResult == MessageBoxResult.No)
                    return;
            }

            try
            {
                var check = await FileHandleRename.RenameFile(file, newFilePath);
                if (!check)
                    return;
            }
            catch (FileHandlerException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    $"{ViewResources.ErrorMessage}{nameof(Rename)}");
                return;
            }

            owner.Observer[owner.FileContext.CurrentId] = newFilePath;
            owner.GenerateView(newFilePath);
        }

        /// <summary>
        ///     Saves the currently loaded image to a user-selected file path.
        /// </summary>
        /// <param name="owner">The image view owner.</param>
        /// <param name="obj">Unused parameter (reserved for interface compatibility).</param>
        internal void Save(ImageView owner, object obj)
        {
            if (owner?.Bmp == null)
                return;

            var bitmap = owner.Bmp.ToBitmap();
            var pathObj = DialogHandler.HandleFileSave(ViewResources.FileOpen, owner.FileContext.CurrentPath);
            if (pathObj == null)
                return;

            if (string.Equals(pathObj.FilePath, owner.FileContext.FilePath, StringComparison.OrdinalIgnoreCase))
                _ = FileHandleDelete.DeleteFile(owner.FileContext.FilePath);

            try
            {
                var success = owner.SaveImage(pathObj.FilePath, pathObj.Extension, bitmap);
                if (!success)
                    _ = MessageBox.Show(ViewResources.ErrorCouldNotSaveFile);
            }
            catch (Exception ex) when (ex is ArgumentException or IOException or ExternalException)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    $"{ViewResources.ErrorMessage}{nameof(Save)}");
            }
        }
    }
}