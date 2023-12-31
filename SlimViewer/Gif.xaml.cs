using System.ComponentModel;
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
        }

        /// <summary>
        ///     Thumbs the image clicked.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        private void Thumb_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id);
        }

        /// <summary>
        ///     Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //TODO add Check
            View.ClearCommand.Execute(null);
        }
    }
}