/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     PreviewItem
 * FILE:        SlimViews.DataObjects.cs
 * PURPOSE:     Preview Object for the Rename Window
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using ViewModel;

namespace SlimViews.DataObjects
{
    /// <summary>
    ///     Represents a single file in the rename preview grid.
    ///     Allows the user to see the "Before" and "After" state without touching the hard drive.
    /// </summary>
    public class PreviewItem : ViewModelBase
    {
        private string _newName;
        private string _status;

        /// <summary>
        ///     Gets or sets the internal ID from the observer dictionary.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the original full file path.
        /// </summary>
        public string OriginalPath { get; set; }

        /// <summary>
        ///     Gets or sets the original file name (without directory).
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        ///     Gets or sets the predicted new file name.
        ///     Updates automatically in the UI when changed.
        /// </summary>
        public string NewName
        {
            get => _newName;
            set => SetProperty(ref _newName, value, nameof(NewName));
        }

        /// <summary>
        ///     Gets or sets the commit status (e.g., "Pending", "Success", "Failed").
        /// </summary>
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value, nameof(Status));
        }
    }
}
