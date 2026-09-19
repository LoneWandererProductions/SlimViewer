/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews.Contexts
 * FILE:        UiState.cs
 * PURPOSE:     Ui state management for ImageView, including visibility, paths, and references to controls. Pure UI state with no image data.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.IO;
using Common.Images;
using System.Reflection;
using System.Windows;
using ViewModel;

namespace SlimViews.Contexts
{
    /// <summary>
    /// ImageView UI State, holds all UI-related state and references to controls used by <see cref="ImageView"/>.
    /// </summary>
    /// <seealso cref="System.IEquatable&lt;SlimViews.Contexts.UiState&gt;" />
    public class UiState : ViewModelBase
    {
        /// <summary>
        /// Whether background work is in progress
        /// </summary>
        private bool _isBusy;

        /// <summary>
        ///     Gets or sets the root.
        /// </summary>
        /// <value>
        ///     The root.
        /// </value>
        internal readonly string? Root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// The cif editor visibility
        /// </summary>
        private Visibility _cifEditorVisibility = Visibility.Collapsed;

        /// <summary>
        /// Gets or sets the visibility of the CifChannelEditor panel.
        /// </summary>
        /// <value>
        /// The cif editor visibility.
        /// </value>
        public Visibility CifEditorVisibility
        {
            get => _cifEditorVisibility;
            set
            {
                _cifEditorVisibility = value;
                OnPropertyChanged(nameof(CifEditorVisibility));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether background work (loading an image, scanning
        /// a folder, deleting/renaming files, etc.) is currently in progress. Drives the pulsing
        /// <see cref="Common.Images.BusyIndicator" /> dot shown in the UI - a steady dot when
        /// <c>false</c>, a pulsing one when <c>true</c>. This is the thing to set/clear around any
        /// background operation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        /// <summary>
        /// Gets or sets the left button visibility.
        /// </summary>
        /// <value>
        /// The left button visibility.
        /// </value>
        public Visibility LeftButtonVisibility { get; set; }

        /// <summary>
        /// Gets or sets the right button visibility.
        /// </summary>
        /// <value>
        /// The right button visibility.
        /// </value>
        public Visibility RightButtonVisibility { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail visibility.
        /// </summary>
        /// <value>
        /// The thumbnail visibility.
        /// </value>
        public Visibility ThumbnailVisibility { get; set; }

        /// <summary>
        /// Gets or sets the reference to the main Window.
        /// Internal use only no data Binding.
        /// </summary>
        /// <value>
        /// The main window.
        /// </value>
        internal Window? Main { get; set; }

        /// <summary>
        /// Gets or sets the reference to Image Zoom custom Control itself.
        /// No data Binding only reference to control.
        /// </summary>
        /// <value>
        /// The zoom.
        /// </value>
        internal ImageZoom? ImageZoomControl { get; set; }

        /// <summary>
        /// Gets or sets the thumb Control.
        /// Internal use only no data Binding.
        /// </summary>
        /// <value>
        /// The thumb.
        /// </value>
        internal Thumbnails? Thumb { get; set; }

        /// <summary>
        /// Gets or sets the picker.
        /// </summary>
        /// <value>
        /// The picker.
        /// </value>
        internal ColorPickerMenu? Picker { get; set; }

        //--- Internal control settings ---

        /// <summary>
        /// Gets or sets a value indicating whether [automatic clean].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic clean]; otherwise, <c>false</c>.
        /// </value>
        internal bool AutoClean { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is thumbs visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is thumbs visible; otherwise, <c>false</c>.
        /// </value>
        internal bool IsThumbsVisible { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether this instance is selection empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selection empty; otherwise, <c>false</c>.
        /// </value>
        internal bool IsSelectionEmpty => Thumb?.Selection == null || Thumb.Selection.Count == 0;

        /// <summary>
        /// Gets or sets a value indicating whether [use sub folders].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use sub folders]; otherwise, <c>false</c>.
        /// </value>
        internal bool UseSubFolders { get; set; }

        //Internal methods

        /// <summary>
        /// Hides the buttons.
        /// </summary>
        internal void HideButtons() => LeftButtonVisibility = RightButtonVisibility = Visibility.Hidden;

        /// <summary>
        /// Thumbnails the state.
        /// </summary>
        /// <returns>State of Thumbnail Visibility</returns>
        internal Visibility ThumbnailState() => IsThumbsVisible ? Visibility.Visible : Visibility.Hidden;
    }
}