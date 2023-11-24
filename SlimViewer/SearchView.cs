/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViewer/SearchView.cs
 * PURPOSE:     View Model for Search
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ExtendedSystemObjects;
using FileHandler;
using ImageCompare;
using Imaging;
using ViewModel;

namespace SlimViewer
{
    /// <inheritdoc />
    /// <summary>
    ///     Search View Model
    /// </summary>
    internal sealed class SearchView : INotifyPropertyChanged
    {
        /// <summary>
        ///     The analysis
        /// </summary>
        private readonly ImageAnalysis _analysis = new();

        /// <summary>
        ///     The b
        /// </summary>
        private int _b;

        /// <summary>
        ///     The current folder
        /// </summary>
        private string _currentFolder;

        /// <summary>
        ///     The g
        /// </summary>
        private int _g;

        /// <summary>
        ///     The r
        /// </summary>
        private int _r;

        /// <summary>
        ///     The range
        /// </summary>
        private int _range;

        /// <summary>
        ///     The search color command
        /// </summary>
        private ICommand _searchColorCommand;

        /// <summary>
        ///     The search string
        /// </summary>
        private string _searchString;

        /// <summary>
        ///     The search string command
        /// </summary>
        private ICommand _searchStringCommand;

        /// <summary>
        ///     The sub folders
        /// </summary>
        private bool _subFolders;

        /// <summary>
        ///     The view
        /// </summary>
        private ImageView _view;

        /// <summary>
        ///     Gets or sets the range.
        /// </summary>
        /// <value>
        ///     The range.
        /// </value>
        public int Range
        {
            get => _range;
            set
            {
                if (_range == value) return;

                _range = value;
                OnPropertyChanged(nameof(Range));
            }
        }

        /// <summary>
        ///     Gets or sets the r.
        /// </summary>
        /// <value>
        ///     The r.
        /// </value>
        public int R
        {
            get => _r;
            set
            {
                if (_r == value) return;

                _r = value;
                OnPropertyChanged(nameof(R));
            }
        }

        /// <summary>
        ///     Gets or sets the g.
        /// </summary>
        /// <value>
        ///     The g.
        /// </value>
        public int G
        {
            get => _g;
            set
            {
                if (_g == value) return;

                _g = value;
                OnPropertyChanged(nameof(G));
            }
        }

        /// <summary>
        ///     Gets or sets the b.
        /// </summary>
        /// <value>
        ///     The b.
        /// </value>
        public int B
        {
            get => _b;
            set
            {
                if (_b == value) return;

                _b = value;
                OnPropertyChanged(nameof(B));
            }
        }

        /// <summary>
        ///     Gets or sets the search string.
        /// </summary>
        /// <value>
        ///     The search string.
        /// </value>
        public string SearchString
        {
            get => _searchString;
            set
            {
                if (_searchString == value) return;

                _searchString = value;
                OnPropertyChanged(nameof(SearchString));
            }
        }


        /// <summary>
        ///     Gets the search string command.
        /// </summary>
        /// <value>
        ///     The search string command.
        /// </value>
        public ICommand SearchStringCommand =>
            _searchStringCommand ??= new DelegateCommand<object>(StringAction, CanExecute);

        /// <summary>
        ///     Gets the search color command.
        /// </summary>
        /// <value>
        ///     The search color command.
        /// </value>
        public ICommand SearchColorCommand =>
            _searchColorCommand ??= new DelegateCommand<object>(ColorAction, CanExecute);

        /// <inheritdoc />
        /// <summary>
        ///     Triggers if an Attribute gets changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            // check if executing is allowed, not used right now
            return true;
        }

        /// <summary>
        ///     Initiates the specified sub folders.
        /// </summary>
        /// <param name="subFolders">if set to <c>true</c> [sub folders].</param>
        /// <param name="currentFolder">The current folder.</param>
        /// <param name="imageView">The image view.</param>
        /// <param name="colorHsv">The color HSV.</param>
        internal void Initiate(bool subFolders, string currentFolder, ImageView imageView, ColorHsv colorHsv)
        {
            _subFolders = subFolders;
            _currentFolder = currentFolder;
            _view = imageView;
            //just a start value
            Range = 3;

            if (colorHsv is null) return;

            R = colorHsv.R;
            G = colorHsv.G;
            B = colorHsv.B;
        }

        /// <summary>
        ///     String Search action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void StringAction(object obj)
        {
            if (string.IsNullOrEmpty(SearchString)) return;

            var lst = FileHandleSearch.GetFilesWithSubString(_currentFolder, ImagingResources.Appendix, _subFolders,
                SearchString, true);

            if (lst.IsNullOrEmpty()) return;

            _view.ChangeImage(lst);
        }

        /// <summary>
        ///     Colors Search action.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <exception cref="ArgumentException">Argument Exception</exception>
        /// <exception cref="InvalidOperationException">Invalid Operation</exception>
        private void ColorAction(object obj)
        {
            try
            {
                var lst = _analysis.FindImagesInColorRange(R, G, B, Range, _currentFolder, _subFolders,
                    ImagingResources.Appendix);

                if (lst.IsNullOrEmpty()) return;

                _view.ChangeImage(lst);
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}