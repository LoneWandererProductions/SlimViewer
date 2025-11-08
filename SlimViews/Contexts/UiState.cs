using CommonControls;
using System.Windows;

namespace SlimViews.Contexts
{
    public record UiState
    {
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

        //Internal control settings

        public bool AutoClean { get; internal set; }

        public bool IsThumbsVisible { get; set; }

        public bool UseSubFolders { get; internal set; }
    }
}