/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        FileProcessingCommands.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using Common.Dialogs;
using FileHandler;
using Imaging;
using System;
using System.Collections.Generic;
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
                if (owner.Image.CompressCif)
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
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="paths">The paths.</param>
        /// <param name="isSilent">if set to <c>true</c> [is silent].</param>
        internal async Task DeleteAsync(ImageView owner, List<string> paths, bool isSilent)
        {
            if (owner == null || paths.Count == 0) return;

            // 1. SCHRITT: Dem Mother Window befehlen, das Bild loszulassen.
            // Wir leeren die Anzeige, damit WPF den File-Handle freigibt.
            owner.Image?.Clear();

            // Kleiner Trick: Wir geben WPF einen Moment Zeit, das UI-Binding zu lösen
            await Task.Yield();

            int deletedCount = 0;

            foreach (var path in paths)
            {
                try
                {
                    // 2. SCHRITT: Jetzt ist die Datei (hoffentlich) frei zum Löschen
                    if (await FileHandleSafeDelete.DeleteFile(path))
                    {
                        deletedCount++;
                        if (owner.Count > 0) owner.Count--;
                    }
                }
                catch (Exception ex)
                {
                    // Falls es immer noch lockt, sehen wir es hier im Trace
                    Trace.WriteLine($"CRITICAL: Lock still active on {path}: {ex.Message}");
                }
            }

            // 3. SCHRITT: UI aufräumen
            if (deletedCount > 0)
            {
                // Die Liste im Hauptfenster aktualisieren (das entfernt die Thumbnails)
                owner.LoadThumbs(owner.FileContext.CurrentPath);
                owner.RefreshActionAsync(nameof(FileProcessingCommands));

                if (!isSilent)
                {
                    MessageBox.Show($"{ViewResources.MessageCount}{deletedCount}",
                        ViewResources.MessageSuccess);
                }
            }
        }

        /// <summary>
        /// The "Selection" method: Gathers paths from the Mother Window state 
        /// and delegates to the path-based DeleteAsync.
        /// </summary>
        internal async Task DeleteAsync(ImageView owner)
        {
            if (owner?.UiState == null || owner.FileContext?.Observer == null)
                return;

            var pathsToDelete = new List<string>();

            // Gather paths based on current selection or current item
            if (owner.UiState.IsSelectionEmpty)
            {
                if (owner.FileContext.Observer.TryGetValue(owner.FileContext.CurrentId, out var path))
                {
                    pathsToDelete.Add(path);
                }
            }
            else
            {
                // Snapshot the keys to avoid "Collection Modified" errors
                var selectedIds = owner.UiState.Thumb.Selection.Keys.ToArray();
                foreach (var id in selectedIds)
                {
                    if (owner.FileContext.Observer.TryGetValue(id, out var path))
                    {
                        pathsToDelete.Add(path);
                    }
                }
            }

            // Delegate the actual work to the robust path-based method
            await DeleteAsync(owner, pathsToDelete, isSilent: false);
        }

        /// <summary>
        ///     Moves selected or current images to a new folder.
        /// </summary>
        /// <param name="owner">The image view owner.</param>
        /// <param name="obj">Unused parameter (reserved for interface compatibility).</param>
        internal void Move(ImageView owner, object obj)
        {
            if (owner == null || (!File.Exists(owner.FileContext.FileName) && owner.UiState.IsSelectionEmpty))
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
                if (!owner.FileContext.Observer.TryGetValue(id, out var sourcePath) || !File.Exists(sourcePath))
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

            owner.RefreshActionAsync(nameof(FileProcessingCommands));
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

            owner.RefreshActionAsync(nameof(FileProcessingCommands));
        }

        /// <summary>
        ///     Renames the current image file.
        /// </summary>
        /// <param name="owner">The image view owner.</param>
        internal async Task RenameCurrentAsync(ImageView owner)
        {
            if (!owner.FileContext.Observer.TryGetValue(owner.FileContext.CurrentId, out string? oldPath))
                return;

            var folder = Path.GetDirectoryName(oldPath);
            if (string.IsNullOrEmpty(folder)) return;

            var newPath = Path.Combine(folder, owner.FileContext.FileName);

            if (File.Exists(newPath) && oldPath != newPath)
            {
                var result = MessageBox.Show(ViewResources.MessageFileAlreadyExists,
                    ViewResources.CaptionFileAlreadyExists,
                    MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) return;
            }

            string? resultPath = await RenameAsync(owner, oldPath, newPath, isSilent: false);

            if (resultPath != null)
            {
                // Reload the view with the new name
                owner.GenerateView(resultPath);
            }
        }

        /// <summary>
        /// Core rename logic: Handles the Lock by clearing the owner view first.
        /// </summary>
        internal async Task<string?> RenameAsync(ImageView owner, string sourcePath, string targetPath, bool isSilent)
        {
            if (owner == null || string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(targetPath))
                return null;

            if (sourcePath == targetPath) return sourcePath;

            // 1. LOCK PREVENTION: Clear the image viewer
            // If the owner is currently displaying the file we are about to rename, WPF will lock it.
            owner.Image?.Clear();
            await Task.Yield();

            try
            {
                bool success = await FileHandleRename.RenameFile(sourcePath, targetPath);

                if (success)
                {
                    // Update the Observer in the Mother Window so the ID now points to the new path
                    var match = owner.FileContext.Observer.FirstOrDefault(x => x.Value == sourcePath);
                    if (match.Value != null)
                    {
                        owner.FileContext.Observer[match.Key] = targetPath;
                    }

                    // 2. REFRESH UI
                    owner.LoadThumbs(owner.FileContext.CurrentPath);
                    owner.RefreshActionAsync(nameof(FileProcessingCommands));

                    return targetPath;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Rename failed: {ex.Message}");
                if (!isSilent)
                {
                    MessageBox.Show($"{ViewResources.ErrorMessage}: {ex.Message}");
                }
            }

            return null;
        }

        /// <summary>
        ///     Saves the currently loaded image to a user-selected file path.
        /// </summary>
        /// <param name="owner">The image view owner.</param>
        /// <param name="obj">Unused parameter (reserved for interface compatibility).</param>
        internal void Save(ImageView owner, object obj)
        {
            if (owner?.Image.BitmapImage == null)
                return;

            var bitmap = owner.Image.BitmapImage.ToBitmap();
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