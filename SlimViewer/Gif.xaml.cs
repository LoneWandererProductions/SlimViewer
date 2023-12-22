using System.Windows;
using CommonControls;

namespace SlimViewer
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Gif creator window
    /// </summary>
    internal partial class Gif
    {
        public Gif()
        {
            InitializeComponent();
            View.Image = ImageGif;
        }

        /// <summary>
        ///     Thumbs the image clicked.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        private void Thumb_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id);
        }
    }
}