/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/CompareView.cs
 * PURPOSE:     View Model for the Comparer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ExtendedSystemObjects;
using ImageCompare;
using Imaging;
using ViewModel;

namespace SlimViews
{
    /// <inheritdoc />
    /// <summary>
    ///     Compare the Images, the View Model
    /// </summary>
    /// <seealso cref="ViewModel.ViewModelBase" />
    internal sealed class CompareView : ViewModelBase
    {
        /// <summary>
        ///     The analysis
        /// </summary>
        private readonly ImageAnalysis _analysis = new();

        /// <summary>
        ///     The compare
        /// </summary>
        private readonly ImageComparer _compare = new();

        /// <summary>
        ///     The image view, for callbacks
        /// </summary>
        private ImageView _imageView;

        /// <summary>
        ///     The index
        /// </summary>
        private int _index;

        /// <summary>
        ///     The modulo
        /// </summary>
        private int _modulo;

        /// <summary>
        ///     The next command
        /// </summary>
        private ICommand _nextCommand;

        /// <summary>
        ///     The observer eight
        /// </summary>
        private Dictionary<int, string> _observerEight;

        /// <summary>
        ///     The observer fifth
        /// </summary>
        private Dictionary<int, string> _observerFifth;

        /// <summary>
        ///     The observer first
        /// </summary>
        private Dictionary<int, string> _observerFirst;

        /// <summary>
        ///     The observer fourth
        /// </summary>
        private Dictionary<int, string> _observerFourth;

        /// <summary>
        ///     The observer ninth
        /// </summary>
        private Dictionary<int, string> _observerNinth;

        /// <summary>
        ///     The observer second
        /// </summary>
        private Dictionary<int, string> _observerSecond;

        /// <summary>
        ///     The observer seventh
        /// </summary>
        private Dictionary<int, string> _observerSeventh;

        /// <summary>
        ///     The observer sixth
        /// </summary>
        private Dictionary<int, string> _observerSixth;

        /// <summary>
        ///     The observer tenth
        /// </summary>
        private Dictionary<int, string> _observerTenth;

        /// <summary>
        ///     The observer third
        /// </summary>
        private Dictionary<int, string> _observerThird;

        /// <summary>
        ///     The previous command
        /// </summary>
        private ICommand _previousCommand;

        /// <summary>
        ///     The rows
        /// </summary>
        private int _rows;

        /// <summary>
        ///     The status
        /// </summary>
        private string _status;

        /// <summary>
        ///     Gets or sets the duplicates.
        /// </summary>
        /// <value>
        ///     The duplicates.
        /// </value>
        private List<List<string>> Duplicates { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        /// <value>
        ///     The status.
        /// </value>
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value, nameof(Status));
        }

        /// <summary>
        ///     Gets the previous Image.
        /// </summary>
        /// <value>
        ///     The previous Image.
        /// </value>
        public ICommand PreviousCommand => _previousCommand ??= new DelegateCommand<object>(PreviousAction, CanExecute);

        /// <summary>
        ///     Gets the next Image.
        /// </summary>
        /// <value>
        ///     The next Image.
        /// </value>
        public ICommand NextCommand =>
            _nextCommand ??= new DelegateCommand<object>(NextAction, CanExecute);

        /// <summary>
        ///     Gets or sets the observer first.
        /// </summary>
        /// <value>
        ///     The observer first.
        /// </value>
        public Dictionary<int, string> ObserverFirst
        {
            get => _observerFirst;
            set => SetProperty(ref _observerFirst, value, nameof(ObserverFirst));
        }

        /// <summary>
        ///     Gets or sets the observer second.
        /// </summary>
        /// <value>
        ///     The observer second.
        /// </value>
        public Dictionary<int, string> ObserverSecond
        {
            get => _observerSecond;
            set => SetProperty(ref _observerSecond, value, nameof(ObserverSecond));
        }

        /// <summary>
        ///     Gets or sets the observer third.
        /// </summary>
        /// <value>
        ///     The observer third.
        /// </value>
        public Dictionary<int, string> ObserverThird
        {
            get => _observerThird;
            set => SetProperty(ref _observerThird, value, nameof(ObserverThird));
        }

        /// <summary>
        ///     Gets or sets the observer fourth.
        /// </summary>
        /// <value>
        ///     The observer fourth.
        /// </value>
        public Dictionary<int, string> ObserverFourth
        {
            get => _observerFourth;
            set => SetProperty(ref _observerFourth, value, nameof(ObserverFourth));
        }

        /// <summary>
        ///     Gets or sets the observer fifth.
        /// </summary>
        /// <value>
        ///     The observer fifth.
        /// </value>
        public Dictionary<int, string> ObserverFifth
        {
            get => _observerFifth;
            set => SetProperty(ref _observerFifth, value, nameof(ObserverFifth));
        }

        /// <summary>
        ///     Gets or sets the observer sixth.
        /// </summary>
        /// <value>
        ///     The observer sixth.
        /// </value>
        public Dictionary<int, string> ObserverSixth
        {
            get => _observerSixth;
            set => SetProperty(ref _observerSixth, value, nameof(ObserverSixth));
        }

        /// <summary>
        ///     Gets or sets the observer seventh.
        /// </summary>
        /// <value>
        ///     The observer seventh.
        /// </value>
        public Dictionary<int, string> ObserverSeventh
        {
            get => _observerSeventh;
            set => SetProperty(ref _observerSeventh, value, nameof(ObserverSeventh));
        }

        /// <summary>
        ///     Gets or sets the observer eight.
        /// </summary>
        /// <value>
        ///     The observer eight.
        /// </value>
        public Dictionary<int, string> ObserverEight
        {
            get => _observerEight;
            set => SetProperty(ref _observerEight, value, nameof(ObserverEight));
        }

        /// <summary>
        ///     Gets or sets the observer ninth.
        /// </summary>
        /// <value>
        ///     The observer ninth.
        /// </value>
        public Dictionary<int, string> ObserverNinth
        {
            get => _observerNinth;
            set => SetProperty(ref _observerNinth, value, nameof(ObserverNinth));
        }

        /// <summary>
        ///     Gets or sets the observer tenth.
        /// </summary>
        /// <value>
        ///     The observer tenth.
        /// </value>
        public Dictionary<int, string> ObserverTenth
        {
            get => _observerTenth;
            set => SetProperty(ref _observerTenth, value, nameof(ObserverTenth));
        }

        /// <summary>
        ///     Gets a value indicating whether this instance can execute.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     <c>true</c> if this instance can execute the specified object; otherwise, <c>false</c>.
        /// </returns>
        /// <value>
        ///     <c>true</c> if this instance can execute; otherwise, <c>false</c>.
        /// </value>
        private bool CanExecute(object obj)
        {
            // check if executing is allowed, i.e., validate, check if a process is running, etc.
            return true;
        }

        /// <summary>
        ///     Initiates the specified sub folders.
        /// </summary>
        /// <param name="subFolders">if set to <c>true</c> [sub folders].</param>
        /// <param name="currentFolder">The current folder.</param>
        /// <param name="imageView">The image view.</param>
        /// <param name="similarity">The similarity, in Percentages</param>
        internal async Task AsyncInitiate(bool subFolders, string currentFolder, ImageView imageView,
            int similarity = 0)
        {
            _imageView = imageView;

            Status = ViewResources.StatusCompareStart;

            //no specified difference lvl, so Duplicates
            if (similarity == 0)
                _ = await Task.Run(() =>
                    Duplicates = _compare.GetDuplicateImages(currentFolder, subFolders, ImagingResources.Appendix)
                ).ConfigureAwait(false);
            //with difference lvl
            else
                _ = await Task.Run(() =>
                    Duplicates = _compare.GetSimilarImages(currentFolder, subFolders, ImagingResources.Appendix,
                        similarity)
                ).ConfigureAwait(false);

            if (Duplicates == null)
            {
                Status = ViewResources.StatusCompareFinished;
                return;
            }

            _rows = Duplicates.Count / 10;
            _modulo = Duplicates.Count % 10;
            if (_modulo > 0) _rows++;
            _index = 0;

            GenerateView();

            Status = ViewResources.StatusCompareFinished;
        }

        /// <summary>
        ///     Next Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void NextAction(object obj)
        {
            if (_index < _rows - 1)
                _index++;
            else
                return;

            GenerateView();
        }

        /// <summary>
        ///     Previous Image.
        /// </summary>
        private void PreviousAction(object obj)
        {
            if (_index > 0)
                _index--;
            else
                return;

            GenerateView();
        }

        /// <summary>
        ///     Generates the view.
        /// </summary>
        private void GenerateView()
        {
            if (Duplicates.IsNullOrEmpty()) return;

            var index = _index * 10;
            int i;

            if (Duplicates.ElementAtOrDefault(index) != null)
            {
                var lst = Duplicates[index];
                i = 0;
                ObserverFirst = lst.ToDictionary(_ => i++);
            }
            else
            {
                ObserverFirst = null;
            }

            if (Duplicates.ElementAtOrDefault(index + 1) != null)
            {
                var lst = Duplicates[index + 1];
                i = 0;
                ObserverSecond = lst.ToDictionary(_ => i++);
            }
            else
            {
                ObserverSecond = null;
            }

            if (Duplicates.ElementAtOrDefault(index + 2) != null)
            {
                var lst = Duplicates[index + 2];
                i = 0;
                ObserverThird = lst.ToDictionary(_ => i++);
            }
            else
            {
                ObserverThird = null;
            }

            if (Duplicates.ElementAtOrDefault(index + 3) != null)
            {
                var lst = Duplicates[index + 3];
                i = 0;
                ObserverFourth = lst.ToDictionary(_ => i++);
            }
            else
            {
                ObserverFourth = null;
            }

            if (Duplicates.ElementAtOrDefault(index + 4) != null)
            {
                var lst = Duplicates[index + 4];
                i = 0;
                ObserverFifth = lst.ToDictionary(_ => i++);
            }
            else
            {
                ObserverFifth = null;
            }

            if (Duplicates.ElementAtOrDefault(index + 5) != null)
            {
                var lst = Duplicates[index + 5];
                i = 0;
                ObserverSixth = lst.ToDictionary(_ => i++);
            }
            else
            {
                ObserverSixth = null;
            }

            if (Duplicates.ElementAtOrDefault(index + 6) != null)
            {
                var lst = Duplicates[index + 6];
                i = 0;
                ObserverSeventh = lst.ToDictionary(_ => i++);
            }
            else
            {
                ObserverSeventh = null;
            }

            if (Duplicates.ElementAtOrDefault(index + 7) != null)
            {
                var lst = Duplicates[index + 7];
                i = 0;
                ObserverEight = lst.ToDictionary(_ => i++);
            }
            else
            {
                ObserverEight = null;
            }

            if (Duplicates.ElementAtOrDefault(index + 8) != null)
            {
                var lst = Duplicates[index + 8];
                i = 0;
                ObserverNinth = lst.ToDictionary(_ => i++);
            }
            else
            {
                ObserverNinth = null;
            }

            if (Duplicates.ElementAtOrDefault(index + 9) != null)
            {
                var lst = Duplicates[index + 9];
                i = 0;
                ObserverTenth = lst.ToDictionary(_ => i++);
            }
            else
            {
                ObserverTenth = null;
            }
        }

        /// <summary>
        ///     Changes the image.
        ///     Check if File exists will be done in the <seealso cref="ImageView" />
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="id">The image identifier.</param>
        public void ChangeImage(int itemId, int id)
        {
            switch (id)
            {
                case 0:
                {
                    var files = _observerFirst.Values.ToList();
                    if (!_observerFirst.ContainsKey(itemId)) return;

                    var path = _observerFirst[itemId];

                    LoadImages(path, itemId, files);

                    break;
                }
                case 1:
                {
                    var files = _observerSecond.Values.ToList();
                    if (!_observerSecond.ContainsKey(itemId)) return;

                    var path = _observerSecond[itemId];

                    LoadImages(path, itemId, files);

                    break;
                }
                case 2:
                {
                    var files = _observerThird.Values.ToList();
                    if (!_observerThird.ContainsKey(itemId)) return;

                    var path = _observerThird[itemId];

                    LoadImages(path, itemId, files);

                    break;
                }
                case 3:
                {
                    var files = _observerFourth.Values.ToList();
                    if (!_observerFourth.ContainsKey(itemId)) return;

                    var path = _observerFourth[itemId];

                    LoadImages(path, itemId, files);

                    break;
                }
                case 4:
                {
                    var files = _observerFifth.Values.ToList();
                    if (!_observerFifth.ContainsKey(itemId)) return;

                    var path = _observerFifth[itemId];

                    LoadImages(path, itemId, files);

                    break;
                }
                case 5:
                {
                    var files = _observerSixth.Values.ToList();
                    if (!_observerSixth.ContainsKey(itemId)) return;

                    var path = _observerSixth[itemId];

                    LoadImages(path, itemId, files);

                    break;
                }
                case 6:
                {
                    var files = _observerSeventh.Values.ToList();
                    if (!_observerSeventh.ContainsKey(itemId)) return;

                    var path = ObserverSeventh[itemId];

                    LoadImages(path, itemId, files);

                    break;
                }
                case 7:
                {
                    var files = _observerEight.Values.ToList();
                    if (!_observerEight.ContainsKey(itemId)) return;

                    var path = _observerEight[itemId];

                    LoadImages(path, itemId, files);

                    break;
                }
                case 8:
                {
                    var files = _observerNinth.Values.ToList();
                    if (!_observerNinth.ContainsKey(itemId)) return;

                    var path = _observerNinth[itemId];

                    LoadImages(path, itemId, files);

                    break;
                }
                case 9:
                {
                    var files = _observerTenth.Values.ToList();
                    if (!_observerTenth.ContainsKey(itemId)) return;

                    var path = _observerTenth[itemId];

                    LoadImages(path, itemId, files);

                    break;
                }
            }
        }

        /// <summary>
        ///     Loads the images.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="files">The files.</param>
        private void LoadImages(string path, int itemId, List<string> files)
        {
            var cache = _analysis.GetImageDetails(files);

            if (cache == null) return;

            var details = cache[itemId];

            _imageView.ChangeImage(files, path, details.GetDetails());
        }
    }
}