/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews.Contexts
 * FILE:        UiState.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using CommonControls;
using System.IO;
using System.Reflection;
using System.Windows;

namespace SlimViews.Contexts
{
    public record UiState
    {
        /// <summary>
        ///     Gets or sets the root.
        /// </summary>
        /// <value>
        ///     The root.
        /// </value>
        internal readonly string Root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Gets or sets the status image.
        /// </summary>
        /// <value>
        /// The status image.
        /// </value>
        internal string StatusImage { get; set; }

        /// <summary>
        /// Gets the green icon path.
        /// </summary>
        /// <value>
        /// The green icon path.
        /// </value>
        internal string GreenIconPath => Path.Combine(Root, ViewResources.IconPathGreen);

        /// <summary>
        /// Gets the red icon path.
        /// </summary>
        /// <value>
        /// The red icon path.
        /// </value>
        internal string RedIconPath => Path.Combine(Root, ViewResources.IconPathRed);

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
        internal Window Main { get; set; }

        /// <summary>
        /// Gets or sets the reference to Image Zoom custom Control itself.
        /// No data Binding only reference to control.
        /// </summary>
        /// <value>
        /// The zoom.
        /// </value>
        internal ImageZoom ImageZoomControl { get; set; }

        /// <summary>
        /// Gets or sets the thumb Control.
        /// Internal use only no data Binding.
        /// </summary>
        /// <value>
        /// The thumb.
        /// </value>
        internal Thumbnails Thumb { get; set; }

        /// <summary>
        /// Gets or sets the picker.
        /// </summary>
        /// <value>
        /// The picker.
        /// </value>
        internal ColorPickerMenu Picker { get; set; }

        //Internal control settings

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
        internal bool IsSelectionEmpty => Thumb.Selection == null || Thumb.Selection.Count == 0;

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