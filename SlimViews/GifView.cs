/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/GifView.cs
 * PURPOSE:     View Model for the Gif Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable MemberCanBeInternal

using CommonControls;
using CommonDialogs;
using ExtendedSystemObjects;
using FileHandler;
using Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ViewModel;

namespace SlimViews
{
    /// <inheritdoc />
    /// <summary>
    ///     Gif Viewer
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public sealed class GifView : ViewModelBase, IDisposable
    {
        /// <summary>
        /// The automatic clear
        /// </summary>
        private bool _autoClear;

        /// <summary>
        /// The BMP (currently shown frame)
        /// </summary>
        private BitmapImage? _bmp;

        // Commands

        /// <summary>
        /// The clear command
        /// </summary>
        private ICommand? _clearCommand;

        /// <summary>
        /// The close command
        /// </summary>
        private ICommand? _closeCommand;

        /// <summary>
        /// The open command
        /// </summary>
        private ICommand? _openCommand;

        /// <summary>
        /// The open folder command
        /// </summary>
        private ICommand? _openFolderCommand;

        /// <summary>
        /// The save GIF command
        /// </summary>
        private ICommand? _saveGifCommand;

        /// <summary>
        /// The save images command
        /// </summary>
        private ICommand? _saveImagesCommand;

        // Paths & Info

        /// <summary>
        /// The file path
        /// </summary>
        private string _filePath = string.Empty;

        /// <summary>
        /// The GIF export
        /// </summary>
        private string _gifExport = string.Empty;

        /// <summary>
        /// The GIF path
        /// </summary>
        private string _gifPath = string.Empty;

        /// <summary>
        /// The image export
        /// </summary>
        private string _imageExport = string.Empty;

        /// <summary>
        /// The information
        /// </summary>
        private string _information = string.Empty;

        /// <summary>
        /// The output path
        /// </summary>
        private string _outputPath = Directory.GetCurrentDirectory();

        // Status

        /// <summary>
        /// The is active
        /// </summary>
        private bool _isActive;

        /// <summary>
        /// The observer
        /// </summary>
        private Dictionary<int, string> _observer = new();

        /// <summary>
        /// Cancellation token source for background operations
        /// </summary>
        private CancellationTokenSource? _cts;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GifView"/> class.
        ///     Commands are created here to avoid lazy-field boilerplate elsewhere.
        /// </summary>
        public GifView(Window window, Thumbnails thumb)
        {
            _window = window;
            Thumbnail = thumb;
            // initialize commands (keeps using DelegateCommand<object> to match existing pattern)
            _openCommand = new DelegateCommand<object>(async _ => await OpenActionAsync(), CanExecute);
            _openFolderCommand = new DelegateCommand<object>(async _ => await OpenFolderActionAsync(), CanExecute);
            _clearCommand = new DelegateCommand<object>(_ => ClearAction(), CanExecute);
            _saveGifCommand = new DelegateCommand<object>(_ => SaveGifAction(), CanExecute);
            _saveImagesCommand = new DelegateCommand<object>(_ => SaveImagesAction(), CanExecute);
            _closeCommand = new DelegateCommand<object>(_ => CloseAction(), CanExecute);
        }

        /// <summary>
        /// Releases resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        /// <summary>
        ///     Gets the open command.
        /// </summary>
        public ICommand OpenCommand => _openCommand!;

        /// <summary>
        ///     Gets the open folder command.
        /// </summary>
        public ICommand OpenFolderCommand => _openFolderCommand!;

        /// <summary>
        ///     Gets the clear command.
        /// </summary>
        public ICommand ClearCommand => _clearCommand!;

        /// <summary>
        ///     Gets the save GIF command.
        /// </summary>
        public ICommand SaveGifCommand => _saveGifCommand!;

        /// <summary>
        ///     Gets the save images command.
        /// </summary>
        public ICommand SaveImagesCommand => _saveImagesCommand!;

        /// <summary>
        ///     Gets the close command.
        /// </summary>
        public ICommand CloseCommand => _closeCommand!;

        /// <summary>
        ///     Gets or sets the basic File information.
        /// </summary>
        public string Information
        {
            get => _information;
            set => SetProperty(ref _information, value);
        }

        /// <summary>
        ///     Gets or sets the file path.
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        /// <summary>
        ///     Gets or sets the output path.
        /// </summary>
        public string OutputPath
        {
            get => _outputPath;
            set => SetProperty(ref _outputPath, value);
        }

        /// <summary>
        ///     Gets or sets the BitmapImage (currently displayed frame).
        /// </summary>
        public BitmapImage? Bmp
        {
            get => _bmp;
            set => SetProperty(ref _bmp, value);
        }

        /// <summary>
        ///     Gets or sets the GIF path.
        /// </summary>
        public string GifPath
        {
            get => _gifPath;
            set => SetProperty(ref _gifPath, value);
        }

        /// <summary>
        /// Gets or sets the observer dictionary (index -&gt; file path).
        /// </summary>
        /// <value>
        /// The observer.
        /// </value>
        public Dictionary<int, string> Observer
        {
            get => _observer;
            set => SetProperty(ref _observer, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [automatic clear].
        ///     When true the registered <see cref="SlimViewerRegister.GifCleanUp"/> flag is kept in sync.
        /// </summary>
        public bool AutoClear
        {
            get => _autoClear;
            set => SetPropertyAndCallback(ref _autoClear, value, v => SlimViewerRegister.GifCleanUp = v);
        }

        /// <summary>
        /// The window
        /// </summary>
        private Window _window;

        /// <summary>
        ///     Gets or sets the thumbnail container (internal).
        /// </summary>
        internal Thumbnails Thumbnail { private get; set; } = null!;

        /// <summary>
        ///     Computed image export path under OutputPath.
        /// </summary>
        private string ImageExportPath => Path.Combine(OutputPath, ViewResources.ImagesPath);

        /// <summary>
        ///     Computed gif export path under OutputPath.
        /// </summary>
        private string GifExportPath => Path.Combine(OutputPath, ViewResources.NewGifPath);

        /// <summary>
        /// Changes the displayed image to the file at the given observer id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void ChangeImage(int id)
        {
            if (!_observer.ContainsKey(id)) return;

            var filePath = _observer[id];

            // Clear any previous GIF indicator
            GifPath = string.Empty;

            // Load image synchronously because helper likely caches — move to async if heavy
            var bmpWrapper = ImageProcessor.LoadImage(filePath);
            Bmp = bmpWrapper.ToBitmapImage();

            var fileName = Path.GetFileName(filePath);

            var info = ImageGifHandler.GetImageInfo(filePath);
            if (info == null)
            {
                Information = string.Concat(ViewResources.ErrorFileNotFoundMessage, filePath);
                return;
            }

            // set information text (keeps existing formatting helper)
            Information = ViewResources.BuildImageInformation(filePath, fileName, Bmp);
        }

        /// <summary>
        ///     Generates the thumb view from a list of image file paths.
        /// </summary>
        /// <param name="files">The file list.</param>
        private async Task GenerateThumbViewAsync(IReadOnlyCollection<string> files)
        {
            if (files == null) return;

            var token = GetCancellationToken();

            // Convert collection to dictionary on background thread (IO/cpu work)
            var dict = await Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();
                return files.ToDictionary();
            }, token).ConfigureAwait(false);

            // UI-thread update
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Observer = dict;
            });
        }

        /// <summary>
        ///     Opens a GIF file (command handler). Async wrapper for UI command.
        /// </summary>
        private async Task OpenActionAsync()
        {
            var pathObj = DialogHandler.HandleFileOpen(ViewResources.FileOpenGif, null);
            if (pathObj == null || string.IsNullOrEmpty(pathObj.FilePath)) return;

            // validate extension before heavy work
            if (!string.Equals(pathObj.Extension, ViewResources.GifExt, StringComparison.OrdinalIgnoreCase))
                return;

            // re-create output folders, cancel prior ops
            CancelBackgroundWork();
            _cts = new CancellationTokenSource();

            // ensure directories and export paths exist
            await InitiateAsync(OutputPath).ConfigureAwait(false);

            GifPath = pathObj.FilePath;
            FilePath = GifPath;

            // Get gif info
            var info = ImageGifHandler.GetImageInfo(GifPath);
            if (info == null)
            {
                Information = string.Concat(ViewResources.ErrorFileNotFoundMessage, FilePath);
                return;
            }

            Information = ViewResources.BuildGifInformation(GifPath, info);

            // Convert GIF into frames (async). Use ImageExportPath
            var convertTask = ImageProcessor.ConvertGifActionAsync(GifPath, ImageExportPath);

            try
            {
                await convertTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // conversion cancelled - swallow or set Information
                return;
            }
            catch (Exception ex)
            {
                Information = string.Concat(ViewResources.ErrorFileNotFoundMessage, FilePath, " - ", ex.Message);
                return;
            }

            // Get generated frames
            var currentFolder = ImageExportPath;
            var fileList = FileHandleSearch.GetFilesByExtensionFullPath(currentFolder, ImagingResources.JpgExt, false);

            if (fileList.IsNullOrEmpty()) return;

            // spawn thumbnail generation on UI flow
            await GenerateThumbViewAsync(fileList).ConfigureAwait(false);
        }

        /// <summary>
        /// Opens a folder and converts its images into a GIF (command handler).
        /// </summary>
        private async Task OpenFolderActionAsync()
        {
            var path = DialogHandler.ShowFolder(null);
            if (string.IsNullOrEmpty(path)) return;

            CancelBackgroundWork();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            await InitiateAsync(OutputPath).ConfigureAwait(false);

            // scan folder for supported image appendix (background thread is used by GetFiles... if heavy)
            var fileList = FileHandleSearch.GetFilesByExtensionFullPath(path, ImagingResources.Appendix, false);

            if (fileList == null)
            {
                Information = ViewResources.MessageFiles;
                return;
            }

            // Guard: if too many files, show message and abort
            if (fileList.Count >= 200)
            {
                // Keep UI message as before (breaking MVVM but preserving behavior)
                _ = MessageBox.Show(ViewResources.MessageFiles, ViewResources.MessageInformation);
                return;
            }

            // generate thumbnails for folder
            await GenerateThumbViewAsync(fileList).ConfigureAwait(false);

            // Convert folder to GIF (synchronous call per original, keep result)
            try
            {
                _gifPath = ImageProcessor.ConvertToGifAction(path, _gifPath);
                FilePath = _gifPath;
            }
            catch (Exception ex)
            {
                Information = string.Concat(ViewResources.ErrorFileNotFoundMessage, path, " - ", ex.Message);
                return;
            }

            var info = ImageGifHandler.GetImageInfo(_gifPath);
            if (info == null)
            {
                Information = string.Concat(ViewResources.ErrorFileNotFoundMessage, FilePath);
                return;
            }

            Information = ViewResources.BuildGifInformation(_gifPath, info);
        }

        /// <summary>
        /// Clears temporary exports (images + gifs).
        /// </summary>
        private void ClearAction()
        {
            try
            {
                if (Directory.Exists(ImageExportPath)) Directory.Delete(ImageExportPath, true);
                if (Directory.Exists(GifExportPath)) Directory.Delete(GifExportPath, true);
            }
            catch (Exception ex)
            {
                // Preserve behavior: do not throw to UI; instead show info text
                Information = $"Clear failed: {ex.Message}";
            }
        }

        private bool _isClosing;

        /// <summary>
        ///     Closes the application (preserves original behavior).
        /// </summary>
        private void CloseAction()
        {
            if (_window == null || !_window.IsLoaded)
                return;

            if (SlimViewerRegister.GifCleanUp)
                ClearAction();


            //TODO ERROR HERE!

            _window.Close();
        }


        /// <summary>
        ///     Saves a new GIF from the currently selected thumbnails.
        /// </summary>
        private void SaveGifAction()
        {
            var pathObj = DialogHandler.HandleFileSave(ViewResources.FileOpenGif, OutputPath);
            if (pathObj == null) return;

            // Build list from thumbnail selection (preserve ordering by PathSort helper)
            var lst = Thumbnail.Selection.Keys
                              .Select(id => Observer[id])
                              .ToList();

            lst = lst.PathSort();

            try
            {
                ImageProcessor.ConvertGifAction(lst, pathObj.FilePath);
            }
            catch (Exception ex)
            {
                Information = $"Save GIF failed: {ex.Message}";
            }
        }

        /// <summary>
        ///     Saves the currently selected images to target folder.
        /// </summary>
        private void SaveImagesAction()
        {
            var path = DialogHandler.ShowFolder(OutputPath);
            if (string.IsNullOrEmpty(path)) return;

            var lst = Thumbnail.Selection.Keys
                              .Select(id => _observer[id])
                              .ToList();

            _ = FileHandleCopy.CopyFiles(lst, path, false);
        }

        /// <summary>
        /// Initiates this instance and prepares the output folders.
        /// </summary>
        /// <param name="path">Target Path</param>
        private async Task InitiateAsync(string path)
        {
            var token = GetCancellationToken();

            // compute exports
            _imageExport = Path.Combine(path, ViewResources.ImagesPath);
            _gifExport = Path.Combine(path, ViewResources.NewGifPath);

            await Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();

                // Recreate output path
                if (Directory.Exists(_gifExport)) Directory.Delete(_gifExport, true);
                Directory.CreateDirectory(_gifExport);

                OutputPath = path;



                // ensure directories exist
                if (!Directory.Exists(_imageExport)) Directory.CreateDirectory(_imageExport);
                if (!Directory.Exists(_gifExport)) Directory.CreateDirectory(_gifExport);
            }, token).ConfigureAwait(false);

            IsActive = true;
        }

        /// <summary>
        /// Cancel any background work in progress.
        /// </summary>
        private void CancelBackgroundWork()
        {
            try
            {
                _cts?.Cancel();
                _cts?.Dispose();
            }
            catch
            {
                // ignore
            }
            finally
            {
                _cts = null;
            }
        }

        /// <summary>
        /// Returns the cancellation token or a default none-cancelled token.
        /// </summary>
        /// <returns></returns>
        private CancellationToken GetCancellationToken() => _cts?.Token ?? CancellationToken.None;
    }
}
