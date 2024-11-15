/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/RenameView.cs
 * PURPOSE:     View Model for Rename
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable EventNeverSubscribedTo.Global

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileHandler;
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
        ///     The replacement
        /// </summary>
        private string _replacement;

        /// <summary>
        ///     The replacer
        /// </summary>
        private string _replacer;

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
        ///     Gets the explorer command.
        /// </summary>
        /// <value>
        ///     The explorer command.
        /// </value>
        public ICommand RemoveAppendageCommand => _removeAppendageCommand ??=
            new AsyncDelegateCommand<object>(RemoveAppendageActionAsync, CanExecute);

        /// <summary>
        ///     Gets the add command.
        /// </summary>
        /// <value>
        ///     The add command.
        /// </value>
        public ICommand AddCommand => _addCommand ??= new AsyncDelegateCommand<object>(AddActionAsync, CanExecute);

        /// <summary>
        ///     Gets the remove command.
        /// </summary>
        /// <value>
        ///     The remove command.
        /// </value>
        public ICommand RemoveCommand =>
            _removeCommand ??= new AsyncDelegateCommand<object>(RemoveActionAsync, CanExecute);

        /// <summary>
        ///     Gets the reorder command.
        /// </summary>
        /// <value>
        ///     The reorder command.
        /// </value>
        public ICommand ReorderCommand =>
            _reorderCommand ??= new AsyncDelegateCommand<object>(ReorderCommandActionAsync, CanExecute);

        /// <summary>
        ///     Gets the reorder command.
        /// </summary>
        /// <value>
        ///     The reorder command.
        /// </value>
        public ICommand ReplaceCommand =>
            _replaceCommand ??= new AsyncDelegateCommand<object>(ReplaceCommandActionAsync, CanExecute);

        /// <summary>
        ///     Gets the appendages at command.
        /// </summary>
        /// <value>
        ///     The appendages at command.
        /// </value>
        public ICommand AppendagesAtCommand =>
            _appendagesAtCommand ??= new AsyncDelegateCommand<object>(AppendagesAtActionAsync, CanExecute);

        /// <summary>
        ///     Gets or sets the observer.
        /// </summary>
        /// <value>
        ///     The observer.
        /// </value>
        public ConcurrentDictionary<int, string> Observer { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance can execute.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     <c>true</c> if this instance can execute the specified object; otherwise, <c>false</c>.
        /// </returns>
        /// <value>
        ///     <c>true</c> if this instance can execute; otherwise, <c>false</c>.
        /// </value>
        public bool CanExecute(object obj)
        {
            // check if executing is allowed, not used right now
            return true;
        }

        /// <summary>
        ///     Removes Appendage in File Name
        /// </summary>
        /// <param name="obj">The object.</param>
        private async Task RemoveAppendageActionAsync(object obj)
        {
            var observer = new ConcurrentDictionary<int, string>(Observer);
            if (Replacement == null) return;

            try
            {
                foreach (var (key, value) in Observer)
                {
                    var str = Path.GetFileName(value);

                    var file = str.RemoveAppendage(Replacement);

                    if (string.IsNullOrEmpty(file) ||
                        string.Equals(str, file, StringComparison.OrdinalIgnoreCase)) continue;

                    var directory = Path.GetDirectoryName(value);
                    if (string.IsNullOrEmpty(directory)) continue;

                    var target = Path.Combine(directory, file);

                    var check = await FileHandleRename.RenameFile(value, target);
                    if (check) observer[key] = target;
                }

                SlimViewerRegister.Changed = true;
                Observer = observer;
            }
            catch (FileHandlerException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        ///     Adds Elements in File Name
        /// </summary>
        private async Task AddActionAsync(object obj)
        {
            // Make a copy of the Observer dictionary to modify it
            var observer = new Dictionary<int, string>(Observer);

            // Ensure that the Replacement is not null
            if (Replacement == null) return;

            foreach (var (key, value) in Observer)
            {
                // Get the filename without the directory path
                var str = Path.GetFileName(value);

                // Use the AddAppendage method to create a new filename
                var file = str.AddAppendage(Replacement);

                // If the new filename is the same as the original, skip it
                if (string.IsNullOrEmpty(file) || string.Equals(str, file, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Get the directory path
                var directory = Path.GetDirectoryName(value);
                if (string.IsNullOrEmpty(directory))
                    continue;

                // Combine the directory with the new filename
                var target = Path.Combine(directory, file);

                // Attempt to rename the file asynchronously
                var check = await FileHandleRename.RenameFile(value, target);
                if (check)
                    // Update the observer dictionary with the new filename
                    observer[key] = target;
            }

            // Update the Observer property with the modified dictionary
            Observer = new ConcurrentDictionary<int, string>(observer);
        }


        /// <summary>
        ///     Remove Elements in File Name
        /// </summary>
        private async Task RemoveActionAsync(object obj)
        {
            var observer = new ConcurrentDictionary<int, string>(Observer);
            if (Replacement == null) return;

            try
            {
                foreach (var (key, value) in Observer)
                {
                    var str = Path.GetFileName(value);

                    var file = str.ReplacePart(Replacement, string.Empty);

                    if (string.IsNullOrEmpty(file) ||
                        string.Equals(str, file, StringComparison.OrdinalIgnoreCase)) continue;

                    var directory = Path.GetDirectoryName(value);
                    if (string.IsNullOrEmpty(directory)) continue;

                    var target = Path.Combine(directory, file);

                    var check = await FileHandleRename.RenameFile(value, target);
                    if (check) observer[key] = value;
                }

                SlimViewerRegister.Changed = true;
                Observer = new ConcurrentDictionary<int, string>(observer);
            }
            catch (FileHandlerException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        ///     Reorder Elements in File Name
        /// </summary>
        private async Task ReorderCommandActionAsync(object obj)
        {
            var observer = new ConcurrentDictionary<int, string>(Observer);
            try
            {
                foreach (var (key, value) in Observer)
                {
                    var str = Path.GetFileNameWithoutExtension(value);
                    var ext = Path.GetExtension(value);

                    var file = str.ReOrderNumbers();
                    if (string.IsNullOrEmpty(file) ||
                        string.Equals(str, file, StringComparison.OrdinalIgnoreCase)) continue;

                    file = string.Concat(file, ext);

                    var directory = Path.GetDirectoryName(value);
                    if (string.IsNullOrEmpty(directory)) continue;

                    file = Path.Combine(directory, file);

                    var check = await FileHandleRename.RenameFile(value, file);
                    if (check) observer[key] = file;
                }

                SlimViewerRegister.Changed = true;
                Observer = observer;
            }
            catch (FileHandlerException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        ///     Replaces part or the string command.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async Task ReplaceCommandActionAsync(object obj)
        {
            var observer = new ConcurrentDictionary<int, string>(Observer);
            if (Replacer == null) return;

            try
            {
                foreach (var (key, value) in Observer)
                {
                    var str = Path.GetFileNameWithoutExtension(value);
                    var ext = Path.GetExtension(value);

                    var file = str.ReplacePart(Replacement, Replacer);
                    if (string.IsNullOrEmpty(file) ||
                        string.Equals(str, file, StringComparison.OrdinalIgnoreCase)) continue;

                    file = string.Concat(file, ext);

                    var directory = Path.GetDirectoryName(value);
                    if (string.IsNullOrEmpty(directory)) continue;

                    file = Path.Combine(directory, file);

                    var check = await FileHandleRename.RenameFile(value, file);
                    if (check) observer[key] = file;
                }

                SlimViewerRegister.Changed = true;
                Observer = observer;
            }
            catch (FileHandlerException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        ///     Remove Appendage at number count action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async Task AppendagesAtActionAsync(object obj)
        {
            var observer = new ConcurrentDictionary<int, string>(Observer);
            if (Numbers <= 0) return;

            try
            {
                foreach (var (key, value) in Observer)
                {
                    var str = Path.GetFileNameWithoutExtension(value);
                    var ext = Path.GetExtension(value);

                    if (str.Length <= Numbers) continue;

                    var file = str.Remove(0, Numbers);

                    if (string.IsNullOrEmpty(file)) continue;

                    file = string.Concat(file, ext);

                    var directory = Path.GetDirectoryName(value);
                    if (string.IsNullOrEmpty(directory)) continue;

                    file = Path.Combine(directory, file);

                    var check = await FileHandleRename.RenameFile(value, file);
                    if (check) observer[key] = file;
                }

                SlimViewerRegister.Changed = true;
                Observer = observer;
            }
            catch (FileHandlerException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}