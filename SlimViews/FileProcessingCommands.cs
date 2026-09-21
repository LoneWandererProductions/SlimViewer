/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        FileProcessingCommands.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Common.Dialogs;
using FileHandler;
using Imaging;

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
        internal void ConvertCif(ImageView? owner, object obj)
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
        internal async Task DeleteAsync(ImageView? owner, List<string?> paths, bool isSilent)
        {
            if (owner == null || paths.Count == 0) return;

            // Only release the viewer if the image currently on screen is actually one of the
            // files we're about to delete - otherwise there's no lock to release and no reason
            // to blank the preview out from under the user.
            var isCurrentImageAffected = owner.FileContext.FilePath != null &&
                                         paths.Any(p => string.Equals(p, owner.FileContext.FilePath,
                                             StringComparison.OrdinalIgnoreCase));

            // Captured before anything is actually removed from Observer - needed afterward to
            // find whichever surviving image now sits closest to where this one was.
            var currentIdBeforeDelete = owner.FileContext.CurrentId;

            if (isCurrentImageAffected)
            {
                owner.Image.Clear();
                await Task.Yield();
            }

            // This didn't set IsBusy at all before, unlike the Compare window's equivalent -
            // combined with DeleteFile's Recycle-Bin call previously blocking the UI thread with
            // no real await, a multi-file delete looked completely frozen (no busy indicator, and
            // the file-count label jumping straight from start to finish instead of counting down).
            owner.UiState.IsBusy = true;

            var deletedCount = 0;

            try
            {
                foreach (var path in paths)
                {
                    try
                    {
                        // step: delete file
                        if (await FileHandleSafeDelete.DeleteFile(path))
                        {
                            deletedCount++;

                            // Remove just this one thumbnail cell in place (leaving a blank gap
                            // where it was) instead of forcing a full folder rescan + thumb view
                            // rebuild. Much faster, and the rest of the grid - including scroll
                            // position and any other selections - stays untouched.
                            var match = owner.FileContext.Observer?.FirstOrDefault(x =>
                                string.Equals(x.Value, path, StringComparison.OrdinalIgnoreCase));

                            if (match?.Value != null)
                            {
                                owner.UiState.Thumb?.RemoveSingleItem(match.Value.Key);
                                if (owner.Count > 0) owner.Count--;
                            }

                            owner.FileContext.Files?.RemoveAll(f =>
                                string.Equals(f, path, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"CRITICAL: Lock still active on {path}: {ex.Message}");
                    }
                }
            }
            finally
            {
                owner.UiState.IsBusy = false;
            }

            if (deletedCount > 0)
            {
                if (isCurrentImageAffected)
                {
                    // The image we were viewing is gone - move to whichever surviving image now
                    // sits closest to where it was, instead of owner.NextAction(owner). NextAction
                    // uses Utility.GetNextElement, whose "id not found in the list" fallback wraps
                    // around to the very first id - the exact same fallback it uses for "we're at
                    // the last item, wrap to the first". Since the just-deleted id is *always*
                    // "not found" immediately after RemoveSingleItem, every single-image delete
                    // was unconditionally teleporting the view (and the thumbnail scrollbar with
                    // it) back to the first image in the whole folder, no matter where the deleted
                    // image actually was. Finding the closest surviving neighbor keeps the user in
                    // roughly the same place instead.
                    var survivors = owner.FileContext.Observer?.Keys;
                    var nextId = FindClosestSurvivingId(currentIdBeforeDelete, survivors);

                    if (nextId >= 0)
                    {
                        owner.ChangeImage(nextId);
                        owner.UiState.Thumb?.SelectAndCenter(nextId);
                        owner.NavigationLogic();
                    }
                }

                if (!isSilent)
                {
                    MessageBox.Show($"{ViewResources.MessageCount}{deletedCount}",
                        ViewResources.MessageSuccess);
                }
            }
        }

        /// <summary>
        /// Finds whichever surviving id now sits closest to where <paramref name="removedId" />
        /// used to be: the smallest surviving id that is still &gt;= <paramref name="removedId" />
        /// (i.e. "the next image in the folder, now shifted down one"), falling back to the
        /// largest surviving id if the removed one was at (or past) the end.
        /// </summary>
        /// <param name="removedId">The id that was just removed.</param>
        /// <param name="survivingIds">The ids still present after removal.</param>
        /// <returns>The closest surviving id, or -1 if none remain.</returns>
        private static int FindClosestSurvivingId(int removedId, IEnumerable<int>? survivingIds)
        {
            if (survivingIds == null) return -1;

            var sorted = survivingIds.OrderBy(x => x).ToList();
            if (sorted.Count == 0) return -1;

            foreach (var id in sorted)
            {
                if (id >= removedId) return id;
            }

            return sorted[^1];
        }

        /// <summary>
        /// The "Selection" method: Gathers paths from the Mother Window state
        /// and delegates to the path-based DeleteAsync.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal async Task DeleteAsync(ImageView? owner)
        {
            if (owner?.UiState == null || owner.FileContext?.Observer == null)
                return;

            var pathsToDelete = new List<string?>();

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
        internal void Move(ImageView? owner, object obj)
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

            _ = owner.RefreshActionAsync(nameof(FileProcessingCommands));
        }

        /// <summary>
        ///     Moves all files from the current folder to a selected target folder.
        /// </summary>
        /// <param name="owner">The image view owner.</param>
        /// <param name="obj">Unused parameter (reserved for interface compatibility).</param>
        internal void MoveAll(ImageView? owner, object obj)
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

            IEnumerable<string?> targetFiles = FileHandleSearch.GetFilesByExtensionFullPath(
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

            _ = owner.RefreshActionAsync(nameof(FileProcessingCommands));
        }

        /// <summary>
        ///     Renames the current image file.
        /// </summary>
        /// <param name="owner">The image view owner.</param>
        internal async Task RenameCurrentAsync(ImageView owner)
        {
            if (!owner.FileContext.Observer.TryGetValue(owner.FileContext.CurrentId, out var oldPath))
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

            var resultPath = await RenameAsync(owner, oldPath, newPath, isSilent: false);

            if (resultPath != null)
            {
                // Reload the view with the new name
                owner.GenerateView(resultPath);
            }
        }

        /// <summary>
        /// Core rename logic: Handles the Lock by clearing the owner view first.
        /// </summary>
        internal async Task<string?> RenameAsync(ImageView? owner, string? sourcePath, string? targetPath,
            bool isSilent)
        {
            if (owner == null || string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(targetPath))
                return null;

            if (sourcePath == targetPath) return sourcePath;

            var isCurrentImage = string.Equals(owner.FileContext.FilePath, sourcePath,
                StringComparison.OrdinalIgnoreCase);

            // 1. LOCK PREVENTION: only clear the viewer if we're renaming the file it currently
            // has open - that's the only case WPF could be holding a lock on the file. Renaming
            // some other file (e.g. from the duplicate/similar view) shouldn't blank the preview.
            if (isCurrentImage)
            {
                owner.Image?.Clear();
                await Task.Yield();
            }

            try
            {
                var success = await FileHandleRename.RenameFile(sourcePath, targetPath);

                if (success)
                {
                    // Update the Observer entry IN PLACE - same dictionary instance the Thumbnails
                    // control is bound to, so this alone keeps things in sync. Renaming doesn't
                    // change any pixel data, so the existing thumbnail cell needs no visual refresh,
                    // and there's no need for a full folder rescan / thumb view rebuild.
                    var match = owner.FileContext.Observer?.FirstOrDefault(x => x.Value == sourcePath);
                    if (match?.Value != null)
                    {
                        owner.FileContext.Observer[match.Value.Key] = targetPath;
                    }

                    // Keep the raw file list consistent too (used for sorting/navigation elsewhere).
                    var idx = owner.FileContext.Files?.FindIndex(f =>
                        string.Equals(f, sourcePath, StringComparison.OrdinalIgnoreCase)) ?? -1;
                    if (idx >= 0)
                    {
                        owner.FileContext.Files![idx] = targetPath;
                    }

                    if (isCurrentImage)
                    {
                        owner.FileContext.FilePath = targetPath;
                        owner.FileContext.FileName = Path.GetFileName(targetPath);
                    }

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
        internal void Save(ImageView? owner, object obj)
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