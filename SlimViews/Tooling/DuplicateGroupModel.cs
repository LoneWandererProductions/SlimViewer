/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews.Tooling
 * FILE:        DuplicateGroupModel.cs
 * PURPOSE:     View over each group of duplicate images, containing the image paths and commands for actions on the group (delete, rename, etc.)
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Input;
using ViewModel;

namespace SlimViews.Tooling
{
    /// <summary>
    /// View model representing a group of duplicate images, containing the image paths and commands for actions on the group (delete, rename, etc.)
    /// </summary>
    /// <seealso cref="ViewModel.ViewModelBase" />
    public class DuplicateGroupModel : ViewModelBase
    {
        /// <summary>
        /// The images
        /// </summary>
        private Dictionary<int, string> _images = new();

        /// <summary>
        /// The similarity scores
        /// </summary>
        private Dictionary<int, int> _similarityScores = new();

        /// <summary>
        /// The new name
        /// </summary>
        private string _newName;

        /// <summary>
        /// The group identifier
        /// </summary>
        private string _groupId;

        /// <summary>
        /// Gets the group identifier.
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        public string GroupId
        {
            get => _groupId;
            set => SetProperty(ref _groupId, value, nameof(GroupId));
        }

        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        /// <value>
        /// The images.
        /// </value>
        public Dictionary<int, string> Images
        {
            get => _images;
            set => SetProperty(ref _images, value, nameof(Images));
        }

        /// <summary>
        /// Gets or sets the similarity scores.
        /// </summary>
        /// <value>
        /// The similarity scores.
        /// </value>
        public Dictionary<int, int> SimilarityScores
        {
            get => _similarityScores;
            set => SetProperty(ref _similarityScores, value, nameof(SimilarityScores));
        }

        /// <summary>
        /// Creates new name.
        /// </summary>
        /// <value>
        /// The new name.
        /// </value>
        public string NewName
        {
            get => _newName;
            set => SetProperty(ref _newName, value, nameof(NewName));
        }

        /// <summary>
        /// Gets the selection.
        /// </summary>
        /// <value>
        /// The selection.
        /// </value>
        public ConcurrentDictionary<int, bool> Selection { get; } = new();

        /// <summary>
        /// Gets or sets the delete selected command.
        /// </summary>
        /// <value>
        /// The delete selected command.
        /// </value>
        public ICommand DeleteSelectedCommand { get; set; }

        /// <summary>
        /// Gets or sets the rename selected command.
        /// </summary>
        /// <value>
        /// The rename selected command.
        /// </value>
        public ICommand RenameSelectedCommand { get; set; }


        /// <summary>
        /// Gets or sets the delete all command.
        /// </summary>
        /// <value>
        /// The delete all command.
        /// </value>
        public ICommand DeleteAllCommand { get; set; }
    }
}