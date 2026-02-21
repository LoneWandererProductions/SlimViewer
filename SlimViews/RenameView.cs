/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        RenameView.cs
 * PURPOSE:     View Model for Rename
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable EventNeverSubscribedTo.Global

using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileHandler;
using SlimViews.DataObjects;
using ViewModel;

namespace SlimViews
{
    /// <inheritdoc />
    /// <summary>
    ///     The View for Rename
    /// </summary>
    /// <seealso cref="T:ViewModel.ViewModelBase" />
    public sealed class RenameView : ViewModelBase
    {
        /// <summary>
        ///     The add command
        /// </summary>
        private ICommand _addCommand;

        /// <summary>
        ///     The appendages at command
        /// </summary>
        private ICommand _appendagesAtCommand;

        /// <summary>
        ///     The numbers
        /// </summary>
        private int _numbers;

        /// <summary>
        ///     The remove appendage command
        /// </summary>
        private ICommand _removeAppendageCommand;

        /// <summary>
        ///     The remove command
        /// </summary>
        private ICommand _removeCommand;

        /// <summary>
        ///     The reorder command
        /// </summary>
        private ICommand _reorderCommand;

        /// <summary>
        ///     The replace command
        /// </summary>
        private ICommand _replaceCommand;

        /// <summary>
        ///     The apply changes command (commits to disk)
        /// </summary>
        private ICommand _applyChangesCommand;

        /// <summary>
        ///     The replacement
        /// </summary>
        private string _replacement;

        /// <summary>
        ///     The replacer
        /// </summary>
        private string _replacer;

        /// <summary>
        ///     Indicates whether the view model is currently processing files.
        /// </summary>
        private bool _isWorking;

        /// <summary>
        ///     The internal dictionary keeping track of all files.
        /// </summary>
        private ConcurrentDictionary<int, string> _observer;

        /// <summary>
        ///     Gets the collection of items bound to the Preview DataGrid in the UI.
        /// </summary>
        /// <value>
        ///     The observable collection of PreviewItems.
        /// </value>
        public ObservableCollection<PreviewItem> PreviewItems { get; } = new ObservableCollection<PreviewItem>();

        /// <summary>
        ///     Gets or sets the replacement string.
        /// </summary>
        /// <value>
        ///     The replacement string.
        /// </value>
        public string Replacement
        {
            get => _replacement;
            set => SetProperty(ref _replacement, value, nameof(Replacement));
        }

        /// <summary>
        ///     Gets or sets the numbers.
        /// </summary>
        /// <value>
        ///     The numbers.
        /// </value>
        public int Numbers
        {
            get => _numbers;
            set => SetProperty(ref _numbers, value, nameof(Numbers));
        }

        /// <summary>
        ///     Gets or sets the replacer string.
        /// </summary>
        /// <value>
        ///     The replacer string.
        /// </value>
        public string Replacer
        {
            get => _replacer;
            set => SetProperty(ref _replacer, value, nameof(Replacer));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is actively processing files.
        ///     Used to lock the UI during heavy I/O operations.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is working; otherwise, <c>false</c>.
        /// </value>
        public bool IsWorking
        {
            get => _isWorking;
            set
            {
                SetProperty(ref _isWorking, value, nameof(IsWorking));
                OnPropertyChanged(nameof(IsNotWorking));
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the UI should be enabled (inverse of IsWorking).
        /// </summary>
        /// <value>
        ///     <c>true</c> if it is safe to interact with the UI; otherwise, <c>false</c>.
        /// </value>
        public bool IsNotWorking => !IsWorking;

        /// <summary>
        ///     Gets the remove appendage command.
        /// </summary>
        /// <value>
        ///     The remove appendage command.
        /// </value>
        public ICommand RemoveAppendageCommand => _removeAppendageCommand ??=
            new DelegateCommand<object>(RemoveAppendageAction, CanExecute);

        /// <summary>
        ///     Gets the add command.
        /// </summary>
        /// <value>
        ///     The add command.
        /// </value>
        public ICommand AddCommand => _addCommand ??= new DelegateCommand<object>(AddAction, CanExecute);

        /// <summary>
        ///     Gets the remove command.
        /// </summary>
        /// <value>
        ///     The remove command.
        /// </value>
        public ICommand RemoveCommand => _removeCommand ??= new DelegateCommand<object>(RemoveAction, CanExecute);

        /// <summary>
        ///     Gets the reorder command.
        /// </summary>
        /// <value>
        ///     The reorder command.
        /// </value>
        public ICommand ReorderCommand =>
            _reorderCommand ??= new DelegateCommand<object>(ReorderCommandAction, CanExecute);

        /// <summary>
        ///     Gets the replace command.
        /// </summary>
        /// <value>
        ///     The replace command.
        /// </value>
        public ICommand ReplaceCommand =>
            _replaceCommand ??= new DelegateCommand<object>(ReplaceCommandAction, CanExecute);

        /// <summary>
        ///     Gets the appendages at command.
        /// </summary>
        /// <value>
        ///     The appendages at command.
        /// </value>
        public ICommand AppendagesAtCommand =>
            _appendagesAtCommand ??= new DelegateCommand<object>(AppendagesAtAction, CanExecute);

        /// <summary>
        ///     Gets the apply changes command.
        ///     This is the only command that actually executes file operations on the disk.
        /// </summary>
        /// <value>
        ///     The apply changes command.
        /// </value>
        public ICommand ApplyChangesCommand =>
            _applyChangesCommand ??= new AsyncDelegateCommand<object>(ApplyChangesAsync, CanExecute);

        /// <summary>
        ///     Gets or sets the observer.
        /// </summary>
        /// <value>
        ///     The observer.
        /// </value>
        public ConcurrentDictionary<int, string> Observer
        {
            get => _observer;
            set
            {
                _observer = value;
                InitializePreview(); // Generate preview rows when data is injected
            }
        }

        /// <summary>
        ///     Populates the Preview Grid with the initial filenames.
        /// </summary>
        private void InitializePreview()
        {
            PreviewItems.Clear();
            if (_observer == null) return;

            foreach (var kvp in _observer)
            {
                PreviewItems.Add(new PreviewItem
                {
                    Id = kvp.Key,
                    OriginalPath = kvp.Value,
                    OriginalName = Path.GetFileName(kvp.Value),
                    NewName = Path.GetFileName(kvp.Value), // Defaults to original until a command is run
                    Status = "Pending"
                });
            }
        }

        // ------------------------------------------------------------------
        // PREVIEW GENERATION LOGIC (Synchronous, Memory Only)
        // ------------------------------------------------------------------

        /// <summary>
        ///     Removes Appendage in File Name (Preview Only)
        /// </summary>
        /// <param name="obj">The object.</param>
        private void RemoveAppendageAction(object obj)
        {
            if (string.IsNullOrWhiteSpace(Replacement)) return;

            foreach (var item in PreviewItems)
            {
                var file = item.OriginalName.RemoveAppendage(Replacement);
                if (!string.IsNullOrEmpty(file)) item.NewName = file;
            }
        }

        /// <summary>
        ///     Adds Elements in File Name (Preview Only)
        /// </summary>
        /// <param name="obj">The object.</param>
        private void AddAction(object obj)
        {
            if (string.IsNullOrWhiteSpace(Replacement)) return;

            foreach (var item in PreviewItems)
            {
                var file = item.OriginalName.AddAppendage(Replacement);
                if (!string.IsNullOrEmpty(file)) item.NewName = file;
            }
        }

        /// <summary>
        ///     Remove Elements in File Name (Preview Only)
        /// </summary>
        /// <param name="obj">The object.</param>
        private void RemoveAction(object obj)
        {
            if (string.IsNullOrWhiteSpace(Replacement)) return;

            foreach (var item in PreviewItems)
            {
                var file = item.OriginalName.ReplacePart(Replacement, string.Empty);
                if (!string.IsNullOrEmpty(file)) item.NewName = file;
            }
        }

        /// <summary>
        ///     Reorder Elements in File Name (Preview Only)
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ReorderCommandAction(object obj)
        {
            foreach (var item in PreviewItems)
            {
                var str = Path.GetFileNameWithoutExtension(item.OriginalName);
                var ext = Path.GetExtension(item.OriginalName);

                var file = str.ReOrderNumbers();
                if (!string.IsNullOrEmpty(file)) item.NewName = string.Concat(file, ext);
            }
        }

        /// <summary>
        ///     Replaces part of the string command (Preview Only)
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ReplaceCommandAction(object obj)
        {
            if (string.IsNullOrWhiteSpace(Replacement) || string.IsNullOrWhiteSpace(Replacer)) return;

            foreach (var item in PreviewItems)
            {
                var str = Path.GetFileNameWithoutExtension(item.OriginalName);
                var ext = Path.GetExtension(item.OriginalName);

                var file = str.ReplacePart(Replacement, Replacer);
                if (!string.IsNullOrEmpty(file)) item.NewName = string.Concat(file, ext);
            }
        }

        /// <summary>
        ///     Remove Appendage at number count action (Preview Only)
        /// </summary>
        /// <param name="obj">The object.</param>
        private void AppendagesAtAction(object obj)
        {
            if (Numbers <= 0) return;

            foreach (var item in PreviewItems)
            {
                var str = Path.GetFileNameWithoutExtension(item.OriginalName);
                var ext = Path.GetExtension(item.OriginalName);

                if (str.Length > Numbers)
                {
                    var file = str.Remove(0, Numbers);
                    if (!string.IsNullOrEmpty(file)) item.NewName = string.Concat(file, ext);
                }
            }
        }

        // ------------------------------------------------------------------
        // HARD DRIVE COMMIT LOGIC (Asynchronous)
        // ------------------------------------------------------------------

        /// <summary>
        ///     Reads the Preview Grid and executes the actual file renames on the disk.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async Task ApplyChangesAsync(object obj)
        {
            IsWorking = true;
            try
            {
                var updatedObserver = new ConcurrentDictionary<int, string>(Observer);
                bool changesMade = false;

                foreach (var item in PreviewItems)
                {
                    // Only process files where the name actually changed and hasn't already succeeded
                    if (!string.Equals(item.OriginalName, item.NewName, StringComparison.OrdinalIgnoreCase) &&
                        item.Status != "Success")
                    {
                        var directory = Path.GetDirectoryName(item.OriginalPath);
                        if (string.IsNullOrEmpty(directory)) continue;

                        var target = Path.Combine(directory, item.NewName);

                        var success = await FileHandleRename.RenameFile(item.OriginalPath, target);

                        if (success)
                        {
                            item.Status = "Success";
                            item.OriginalPath = target; // Update path so it can be renamed again if needed
                            item.OriginalName = item.NewName; // Sync original so we don't rename twice
                            updatedObserver[item.Id] = target;
                            changesMade = true;
                        }
                        else
                        {
                            item.Status = "Failed";
                        }
                    }
                }

                if (changesMade)
                {
                    SlimViewerRegister.Changed = true;
                    Observer = updatedObserver;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                MessageBox.Show("An error occurred during batch rename:\n" + ex.Message, "Rename Error");
            }
            finally
            {
                IsWorking = false;
            }
        }
    }
}