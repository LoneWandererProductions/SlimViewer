/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        CompareView.cs
 * PURPOSE:     View Model for the Comparer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global

using ImageCompare;
using Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewModel;

namespace SlimViews
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
        ///     Reference to the parent ImageView for callbacks.
        /// </summary>
        private ImageView _imageView;

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
        private string _status;

        /// <summary>
        ///     Holds the raw duplicate or similar image paths.
        ///     Each inner list represents a single observer group.
        /// </summary>
        private List<List<string>> _duplicates;

        /// <summary>
        ///     Observable collection of observer dictionaries for UI binding.
        ///     Each dictionary maps an item index to its file path.
        /// </summary>
        public ObservableCollection<Dictionary<int, string>> Observers { get; } =
            new ObservableCollection<Dictionary<int, string>>(
                Enumerable.Repeat<Dictionary<int, string>>(null, 10).ToList()
            );

        /// <summary>
        ///     Command to navigate to the previous page of images.
        /// </summary>
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
        private ICommand _previousCommand;

        /// <summary>
        /// The next command
        /// </summary>
        private ICommand _nextCommand;

        /// <summary>
        ///     Gets or sets the status text.
        /// </summary>
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value, nameof(Status));
        }

        /// <summary>
        ///     Initiates the comparison asynchronously.
        ///     Retrieves duplicates or similar images and populates observer groups.
        /// </summary>
        /// <param name="subFolders">Include subfolders if true.</param>
        /// <param name="currentFolder">The folder to scan.</param>
        /// <param name="imageView">Parent ImageView for callbacks.</param>
        /// <param name="similarity">Similarity threshold in percent. 0 = exact duplicates.</param>
        internal async Task AsyncInitiate(bool subFolders, string currentFolder,
            ImageView imageView, int similarity = 0)
        {
            _imageView = imageView;
            Status = ViewResources.StatusCompareStart;

            _duplicates = await Task.Run(() =>
            {
                return similarity == 0
                    ? _compare.GetDuplicateImages(currentFolder, subFolders, ImagingResources.Appendix)
                    : _compare.GetSimilarImages(currentFolder, subFolders, ImagingResources.Appendix, similarity);
            }).ConfigureAwait(false);

            if (_duplicates == null)
            {
                Status = ViewResources.StatusCompareFinished;
                return;
            }

            _rows = (_duplicates.Count + 9) / 10; // Ceiling division
            _index = 0;

            GenerateView();
            Status = ViewResources.StatusCompareFinished;
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
        ///     Generates the observer groups for the current page.
        ///     Updates each dictionary in-place to preserve WPF bindings.
        /// </summary>
        private void GenerateView()
        {
            if (_duplicates == null || _duplicates.Count == 0)
                return;

            int baseIndex = _index * 10;

            // WPF binding note: ObservableCollection tracks Add/Remove but not indexer replacement.
            // Dictionaries inside collection do not implement INotifyPropertyChanged.
            // Therefore we update each dictionary in-place to preserve bindings rather than replace them.
            for (int i = 0; i < 10; i++)
            {
                var group = _duplicates.ElementAtOrDefault(baseIndex + i);

                if (group != null)
                {
                    if (Observers[i] == null)
                        Observers[i] = new Dictionary<int, string>();
                    else
                        Observers[i].Clear();

                    int idx = 0;
                    foreach (var path in group)
                        Observers[i][idx++] = path;

                    // Optional: could raise OnPropertyChanged(nameof(Observers)) here if replacing dictionary entirely
                }
                else
                {
                    if (Observers[i] != null)
                        Observers[i].Clear(); // clearing preserves binding while showing no items
                }
            }

            // Paging logic summary:
            // _index * 10 calculates the starting observer group for the current page.
            // Each page can have up to 10 observer groups (0..9).
            // _rows is calculated as ceiling(_duplicates.Count / 10) to cover partial pages.
            // Observers[0..9] corresponds to UI slots for the current page.
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
            _imageView.ChangeImage(files, path, details.GetDetails());
        }
    }
}