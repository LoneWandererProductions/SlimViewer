/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews.Tooling
 * FILE:        CompareView.cs
 * PURPOSE:     View Model for the Comparer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global


using ImageCompare;
using Imaging;
using Imaging.Gifs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ViewModel;

namespace SlimViews.Tooling
{
    /// <inheritdoc />
    /// <summary>
    ///     ViewModel for comparing images. Handles paging through duplicates or similar images
    ///     and provides observer groups for data binding in the Compare Window.
    /// </summary>
    internal sealed class CompareView : ViewModelBase
    {
        /// <summary>
        ///     Image analysis helper for retrieving image details.
        /// </summary>
        private readonly ImageAnalysis _analysis = new();

        /// <summary>
        ///     Image comparer utility for finding duplicates or similar images.
        /// </summary>
        private readonly ImageComparer _compare = new();

        /// <summary>
        ///     Current page index (0-based).
        /// </summary>
        private int _index;

        /// <summary>
        ///     Total number of pages.
        /// </summary>
        private int _rows;

        /// <summary>
        ///     Status text displayed in the UI.
        /// </summary>
        private string? _status;

        /// <summary>
        /// The selected image path
        /// </summary>
        private string? _selectedImagePath;

        /// <summary>
        ///     Holds the raw duplicate or similar image paths.
        ///     Each inner list represents a single observer group.
        /// </summary>
        private List<List<string>>? _duplicates;

        /// <summary>
        /// The current selected image information
        /// </summary>
        private string? _currentSelectedImageInfo;

        /// <summary>
        /// The thumb image clicked
        /// </summary>
        private DelegateCommand<object>? _thumbImageClicked;

        /// <summary>
        /// The page identifier to path
        /// </summary>
        private readonly Dictionary<int, string> _pageIdToPath = new();

        /// <summary>
        /// Observable collection of observer dictionaries for UI binding.
        /// Each dictionary maps an item index to its file path.
        /// </summary>
        /// <value>
        /// The observers.
        /// </value>
        public ObservableCollection<Dictionary<int, string>> Observers { get; } =
            new ObservableCollection<Dictionary<int, string>>(
                Enumerable.Repeat<Dictionary<int, string>>(null, 10).ToList()
            );

        /// <summary>
        /// Gets or sets the current selected image information.
        /// </summary>
        /// <value>
        /// The current selected image information.
        /// </value>
        public string CurrentSelectedImageInfo
        {
            get => _currentSelectedImageInfo;
            set => SetProperty(ref _currentSelectedImageInfo, value, nameof(CurrentSelectedImageInfo));
        }

        /// <summary>
        /// Gets or sets the path of the currently selected image for the preview panel.
        /// </summary>
        /// <value>
        /// The selected image path.
        /// </value>
        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set => SetProperty(ref _selectedImagePath, value, nameof(SelectedImagePath));
        }

        private ImageView _imageView;

        /// <summary>
        /// Gets or sets the status text.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value, nameof(Status));
        }

        /// <summary>
        /// Gets the current image identifier.
        /// </summary>
        /// <value>
        /// The current image identifier.
        /// </value>
        public int CurrentImageId { get; private set; }

        /// <summary>
        /// Gets the duplicate groups.
        /// </summary>
        /// <value>
        /// The duplicate groups.
        /// </value>
        public ObservableCollection<DuplicateGroupModel> DuplicateGroups { get; } = new();

        /// <summary>
        /// Gets the thumb image clicked.
        /// This handles the metadata display when an image is clicked
        /// </summary>
        /// <value>
        /// The thumb image clicked.
        /// </value>
        public ICommand ThumbImageClicked => _thumbImageClicked ??= new DelegateCommand<object>(param =>
        {
            if (param is Common.Images.ImageEventArgs args)
            {
                var group = DuplicateGroups.FirstOrDefault(g => g.GroupId == args.SenderTag);

                if (group != null && group.Images.TryGetValue(args.Id, out string path))
                {
                    if (File.Exists(path))
                    {
                        group.NewName = Path.GetFileNameWithoutExtension(path);
                        UpdateImageMetadata(path);
                        CurrentImageId = args.Id;

                        SelectedImagePath = path;
                    }
                }
            }
        });

        /// <summary>
        /// Command to navigate to the previous page of images.
        /// </summary>
        /// <value>
        /// The previous command.
        /// </value>
        public ICommand PreviousCommand =>
            _previousCommand ??= new DelegateCommand<object>(PreviousAction, _ => _index > 0);

        /// <summary>
        ///     Command to navigate to the next page of images.
        /// </summary>
        public ICommand NextCommand =>
            _nextCommand ??= new DelegateCommand<object>(NextAction, _ => _index < _rows - 1);

        /// <summary>
        /// The previous command
        /// </summary>
        private ICommand? _previousCommand;

        /// <summary>
        /// The next command
        /// </summary>
        private ICommand? _nextCommand;

        /// <summary>
        /// Initiates the comparison asynchronously.
        /// Retrieves duplicates or similar images and populates observer groups.
        /// </summary>
        /// <param name="subFolders">Include subfolders if true.</param>
        /// <param name="currentFolder">The folder to scan.</param>
        /// <param name="similarity">Similarity threshold in percent. 0 = exact duplicates.</param>
        /// <param name="imageView">The image view.</param>
        internal async Task AsyncInitiate(bool subFolders, string currentFolder, int similarity = 0,
            ImageView imageView = null)
        {
            _imageView = imageView;

            // UI Feedback: Let the user know exactly what kind of search is running
            Status = similarity == 0
                ? "Scanning for exact duplicates..."
                : $"Scanning for images with {similarity}% similarity...";

            _duplicates = await Task.Run(() =>
            {
                // This is the "Tricky" part:
                // similarity == 0 means bit-for-bit check
                // similarity > 0 means visual histogram/perceptual check
                return similarity == 0
                    ? _compare.GetDuplicateImages(currentFolder, subFolders, ImagingResources.Appendix)
                    : _compare.GetSimilarImages(currentFolder, subFolders, ImagingResources.Appendix, similarity);
            }).ConfigureAwait(false);

            if (_duplicates == null || _duplicates.Count == 0)
            {
                Status = "No matching images found.";
                return;
            }

            // Ceiling division for pagination
            _rows = (_duplicates.Count + 9) / 10;
            _index = 0;

            // Trigger the UI thread update
            Application.Current.Dispatcher.Invoke(() =>
            {
                GenerateView();
                Status = $"Found {_duplicates.Count} groups of matches.";
            });
        }

        /// <summary>
        ///     Navigate to the next page of images.
        /// </summary>
        /// <param name="_">Unused command parameter.</param>
        private void NextAction(object _)
        {
            if (_index < _rows - 1)
            {
                _index++;
                GenerateView();
            }
        }

        /// <summary>
        ///     Navigate to the previous page of images.
        /// </summary>
        /// <param name="_">Unused command parameter.</param>
        private void PreviousAction(object _)
        {
            if (_index > 0)
            {
                _index--;
                GenerateView();
            }
        }

        /// <summary>
        /// Generates the observer groups for the current page.
        /// Updates each dictionary in-place to preserve WPF bindings.
        /// </summary>
        private void GenerateView()
        {
            if (_duplicates == null || _duplicates.Count == 0) return;
            DuplicateGroups.Clear();

            int baseIndex = _index * 10;

            for (int i = 0; i < 10; i++)
            {
                var groupPaths = _duplicates.ElementAtOrDefault(baseIndex + i);
                if (groupPaths == null || !groupPaths.Any()) continue;

                var groupModel = new DuplicateGroupModel();

                groupModel.GroupId = $"Group_{i}";
                groupModel.NewName = Path.GetFileNameWithoutExtension(groupPaths.First());

                // ---> Bind the UI buttons to the ViewModel logic <---
                groupModel.DeleteAllCommand = new DelegateCommand<object>(_ => DeleteGroupAsync(groupModel));
                groupModel.DeleteSelectedCommand =
                    new DelegateCommand<object>(async (param) => await DeleteSelectedAsync(groupModel, param));
                groupModel.RenameSelectedCommand =
                    new DelegateCommand<object>(async (param) => await RenameSelectedAsync(groupModel, param));

                var imageDict = new Dictionary<int, string>();
                int localId = 0;
                foreach (var path in groupPaths)
                {
                    imageDict.Add(localId++, path);
                }

                groupModel.Images = imageDict;
                DuplicateGroups.Add(groupModel);
            }
        }

        /// <summary>
        ///     Changes the currently displayed image for a given observer and item.
        /// </summary>
        /// <param name="itemId">Index of the item in the observer group.</param>
        /// <param name="observerIndex">Observer group index (0-9).</param>
        public void ChangeImage(int itemId, int observerIndex)
        {
            if (observerIndex < 0 || observerIndex >= Observers.Count)
                return;

            var dict = Observers[observerIndex];
            if (dict == null || !dict.ContainsKey(itemId))
                return;

            var files = dict.Values.ToList();
            var path = dict[itemId];

            LoadImages(path, itemId, files);
        }

        /// <summary>
        /// Deletes the group asynchronous.
        /// </summary>
        /// <param name="group">The group.</param>
        private async Task DeleteGroupAsync(DuplicateGroupModel group)
        {
            try
            {
                // kvp.Value is the file path string
                await _imageView.Commands.FileService.DeleteAsync(_imageView, group.Images.Values.ToList(), false);
            }
            catch (Exception ex)
            {
                // Log the error and add it to our failed list so it remains in the UI
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Deletes the image asynchronous.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="parameter">The parameter.</param>
        private async Task DeleteSelectedAsync(DuplicateGroupModel group, object parameter)
        {
            // 1. Validate and extract selection
            if (parameter is not ConcurrentDictionary<int, bool> selection)
            {
                Trace.WriteLine("DeleteSelectedAsync: Invalid parameter type.");
                return;
            }

            // 2. Handle empty selection (Default to current image)
            if (selection.IsEmpty)
            {
                selection.AddOrUpdate(CurrentImageId, true, (key, oldValue) => true);
            }

            // 3. Identify IDs to delete
            var selectedKeys = selection.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

            // Safety check: ensure we have something to delete
            if (selectedKeys.Count == 0)
            {
                selectedKeys.Add(CurrentImageId);
            }

            // 4. Perform Deletion
            var updatedImages = new Dictionary<int, string>(group.Images);

            foreach (var key in selectedKeys)
            {
                if (group.Images.TryGetValue(key, out string path))
                {
                    try
                    {
                        await _imageView.Commands.FileService.DeleteAsync(_imageView, new List<string> { path }, false);

                        updatedImages.Remove(key);
                        group.Images.Remove(key); // Remove from the group immediately to update the UI
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"Failed to delete {path}: {ex.Message}");
                    }
                }
            }

            // 5. Update UI State
            group.Images = updatedImages;
            selection.Clear();

            if (group.Images.Count <= 1)
            {
                DuplicateGroups.Remove(group);
            }
        }

        /// <summary>
        /// Renames the selected asynchronous.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="parameter">The parameter.</param>
        private async Task RenameSelectedAsync(DuplicateGroupModel group, object parameter)
        {
            if (parameter is not ConcurrentDictionary<int, bool> selection || string.IsNullOrWhiteSpace(group.NewName))
                return;

            var selectedKeys = selection.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
            if (selectedKeys.Count == 0) selectedKeys.Add(CurrentImageId);

            var updatedImages = new Dictionary<int, string>(group.Images);
            bool anySuccess = false;

            foreach (var key in selectedKeys)
            {
                if (group.Images.TryGetValue(key, out string sourcePath))
                {
                    var extension = Path.GetExtension(sourcePath);
                    var directory = Path.GetDirectoryName(sourcePath);
                    var targetPath = Path.Combine(directory, group.NewName + extension);

                    // Use the Owner's FileService to ensure the viewer is cleared
                    string? newPath =
                        await _imageView.Commands.FileService.RenameAsync(_imageView, sourcePath, targetPath,
                            isSilent: true);

                    if (newPath != null)
                    {
                        updatedImages[key] = newPath;
                        anySuccess = true;
                    }
                }
            }

            if (anySuccess)
            {
                group.Images = updatedImages;
                Status = "Rename complete.";
            }
        }

        /// <summary>
        ///     Loads image details and updates the ImageView.
        /// </summary>
        /// <param name="path">Path of the selected image.</param>
        /// <param name="itemId">Index of the item in the group.</param>
        /// <param name="files">List of file paths in the current observer group.</param>
        private void LoadImages(string path, int itemId, List<string> files)
        {
            var cache = _analysis.GetImageDetails(files);
            if (cache == null)
                return;

            var details = cache[itemId];
        }

        /// <summary>
        /// Updates the image metadata.
        /// </summary>
        /// <param name="path">The path.</param>
        private void UpdateImageMetadata(string path)
        {
            try
            {
                var fileName = Path.GetFileName(path);
                string infoBody;

                // Check if the file is a GIF
                if (Path.GetExtension(path).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                {
                    // Assuming you have a way to load or retrieve the ImageGifInfo for the path
                    var gifData = ImageGifHandler.GetImageInfo(path);
                    infoBody = ViewResources.BuildUnifiedImageInfo(path, fileName, gifData);
                }
                else
                {
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage(new Uri(path));
                    infoBody = ViewResources.BuildUnifiedImageInfo(path, fileName, bitmap);
                }

                int score = GetScoreForPath(path);

                // Combine the multiline body with the similarity score
                CurrentSelectedImageInfo = $"{infoBody}{Environment.NewLine}Similarity: {score}%";
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Metadata Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the score for path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Information about images.</returns>
        private int GetScoreForPath(string path)
        {
            // Look through all groups to find which one contains this image
            var group = DuplicateGroups.FirstOrDefault(g => g.Images.Values.Contains(path));
            if (group != null)
            {
                // Find the key (ID) for this path to get the matching score
                var key = group.Images.FirstOrDefault(x => x.Value == path).Key;
                if (group.SimilarityScores.TryGetValue(key, out int score))
                {
                    return score;
                }
            }

            return 100; // Default for the "original" or if not found
        }
    }
}