using CommonControls;
using System.Windows;

namespace SlimViews.Contexts
{
    public record UiState
    {
        public Visibility LeftButtonVisibility { get; set; }
        public Visibility RightButtonVisibility { get; set; }
        public Visibility ThumbnailVisibility { get; set; }
        public bool Thumbs { get; set; }

        public bool SubFolders { get; internal set; }

        /// <summary>
        /// Gets or sets the reference to the main Window.
        /// </summary>
        /// <value>
        /// The main window.
        /// </value>
        public Window Main { get; set; }

        public Thumbnails Thumb { get; set; }
    }
}