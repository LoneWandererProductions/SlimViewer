/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews.Tooling
 * FILE:        DuplicateSearchConfig.xaml.cs
 * PURPOSE:     Lets the user pick which folder(s) to search for duplicate/similar images and
 *              set an exact similarity percentage, instead of the previous fixed set of
 *              70/80/90/95% menu presets scoped to only the currently-open folder.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Common.Dialogs;

namespace SlimViews.Tooling
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Configuration dialog for a duplicate/similar-image search: which folder(s) to search
    ///     (searched together as one combined pool - see <see cref="Imaging.Compare.ImageComparer" />'s
    ///     multi-folder overloads), whether to include subfolders, and either "exact duplicates
    ///     only" or an exact, freely-chosen similarity percentage.
    /// </summary>
    public sealed partial class DuplicateSearchConfig
    {
        /// <summary>
        ///     Gets the folders the user selected to search.
        /// </summary>
        public ObservableCollection<string> Folders { get; } = new();

        /// <summary>
        ///     Gets a value indicating whether subfolders should be included, after the dialog
        ///     closes with a positive result.
        /// </summary>
        public bool IncludeSubfolders { get; private set; }

        /// <summary>
        ///     Gets the chosen similarity percentage after the dialog closes with a positive
        ///     result. 0 means "exact duplicates only".
        /// </summary>
        public int Similarity { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DuplicateSearchConfig" /> class,
        ///     pre-populating the folder list with the folder currently open in the main viewer
        ///     (if any) so the common case - "just search what I'm already looking at" - doesn't
        ///     require an extra click.
        /// </summary>
        /// <param name="initialFolder">The folder to pre-select, if any.</param>
        public DuplicateSearchConfig(string? initialFolder)
        {
            InitializeComponent();
            DataContext = this;

            if (!string.IsNullOrWhiteSpace(initialFolder))
            {
                Folders.Add(initialFolder);
            }
        }

        /// <summary>
        ///     Handles the Click event of the "Add Folder" button - opens the app's own themed
        ///     folder picker (the same one used everywhere else) rather than a raw native dialog.
        /// </summary>
        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            var startFrom = Folders.Count > 0 ? Folders[^1] : string.Empty;
            var picked = DialogHandler.ShowFolder(startFrom);

            if (string.IsNullOrWhiteSpace(picked)) return;

            if (!Folders.Contains(picked, StringComparer.OrdinalIgnoreCase))
            {
                Folders.Add(picked);
            }
        }

        /// <summary>
        ///     Handles the Click event of the "Remove Selected" button.
        /// </summary>
        private void RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (FolderListBox.SelectedItem is string selected)
            {
                Folders.Remove(selected);
            }
        }

        /// <summary>
        ///     Enables/disables the similarity slider based on which mode is selected. Also fires
        ///     during InitializeComponent before ExactRadio/SimilarRadio/SimilarityPanel are fully
        ///     wired up, so it guards against that with a null check.
        /// </summary>
        private void ModeChanged(object sender, RoutedEventArgs e)
        {
            if (SimilarityPanel == null || SimilarRadio == null) return;

            SimilarityPanel.IsEnabled = SimilarRadio.IsChecked == true;
        }

        /// <summary>
        ///     Keeps the percentage label in sync with the slider as the user drags it.
        /// </summary>
        private void SimilaritySlider_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (SimilarityLabel == null) return;

            SimilarityLabel.Text = $"{(int)Math.Round(e.NewValue)}%";
        }

        /// <summary>
        ///     Handles the Click event of the "Search" button: validates at least one folder is
        ///     selected, captures the final settings, and closes the dialog with a positive result.
        /// </summary>
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (Folders.Count == 0)
            {
                MessageBox.Show(this, "Select at least one folder to search.", "No Folder Selected",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            IncludeSubfolders = SubfoldersCheckBox.IsChecked == true;
            Similarity = ExactRadio.IsChecked == true ? 0 : (int)Math.Round(SimilaritySlider.Value);

            DialogResult = true;
            Close();
        }
    }
}