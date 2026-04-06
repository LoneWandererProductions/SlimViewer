/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews.Tooling
 * FILE:        GifView.cs
 * PURPOSE:     View Model for the Gif Window (Refactored for Async/Thread Safety)
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using Common.Dialogs;
using Common.Images;
using ExtendedSystemObjects;
using FileHandler;
using Imaging;
using Imaging.Gifs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ViewModel;

namespace SlimViews.Tooling
{
    /// <inheritdoc cref="ViewModelBase" />
    /// <summary>
    ///     The ViewModel for the GIF Editor window.
    ///     Handles splitting GIFs into frames, combining images into GIFs, and file exports.
    /// </summary>
    public sealed class GifView : ViewModelBase, IDisposable
    {
        // --- Fields ---

        /// <summary>
        ///     If true, temporary files are deleted when the window closes.
        /// </summary>
        private bool _autoClear;

        /// <summary>
        ///     The currently displayed image frame.
        /// </summary>
        private BitmapImage? _bmp;

        /// <summary>
        ///     Full path to the currently loaded GIF file.
        /// </summary>
        private string _gifPath = string.Empty;

        /// <summary>
        ///     Full path to the currently selected image frame.
        /// </summary>
        private string _filePath = string.Empty;

        /// <summary>
        ///     Status or error message displayed in the UI.
        /// </summary>
        private string _information = string.Empty;

        /// <summary>
        ///     Base directory for temporary file generation.
        ///     Uses a subfolder to avoid cluttering the app root.
        /// </summary>
        private string _outputPath = Path.Combine(Directory.GetCurrentDirectory(), "TempGif");

        /// <summary>
        ///     Indicates if a file is loaded and controls are enabled.
        /// </summary>
        private bool _isActive;

        /// <summary>
        ///     The dictionary of thumbnail images (Index -> FilePath).
        /// </summary>
        private Dictionary<int, string> _observer = new();

        /// <summary>
        ///     Token source for cancelling long-running background tasks (like GIF encoding).
        /// </summary>
        private CancellationTokenSource? _cts;

        /// <summary>
        /// The delays
        /// </summary>
        private Dictionary<int, int> _delays = new();

        /// <summary>
        /// The current delay
        /// Default 100ms
        /// </summary>
        private int _currentDelay = 100;

        /// <summary>
        /// The selected identifier
        /// </summary>
        private int _selectedId = -1;

        /// <summary>
        /// The set delay command
        /// </summary>
        private ICommand? _setDelayCommand;

        /// <summary>
        /// The set all delay command
        /// </summary>
        private ICommand? _setAllDelayCommand;

        /// <summary>
        /// The open command
        /// </summary>
        private ICommand? _openCommand;

        /// <summary>
        /// The open folder command
        /// </summary>
        private ICommand? _openFolderCommand;

        /// <summary>
        /// The clear command
        /// </summary>
        private ICommand? _clearCommand;

        /// <summary>
        /// The save GIF command
        /// </summary>
        private ICommand? _saveGifCommand;

        /// <summary>
        /// The save images command
        /// </summary>
        private ICommand? _saveImagesCommand;

        /// <summary>
        /// The save current frame command
        /// </summary>
        private ICommand? _saveCurrentFrameCommand;

        /// <summary>
        ///     Path where individual frames are extracted to.
        /// </summary>
        private string ImageExportPath => Path.Combine(OutputPath, ViewResources.ImagesPath);

        /// <summary>
        ///     Path where new GIF previews are generated.
        /// </summary>
        private string GifExportPath => Path.Combine(OutputPath, ViewResources.NewGifPath);

        /// <summary>
        ///     Reference to the Thumbnail control (injected via Constructor).
        /// </summary>
        internal Thumbnails Thumbnail { private get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GifView"/> class.
        /// </summary>
        /// <param name="thumb">The thumbnail control reference.</param>
        public GifView(Thumbnails thumb, string? initialFilePath)
        {
            Thumbnail = thumb;
            // Sync local AutoClear with the global register immediately on load
            AutoClear = SlimViewerRegister.GifCleanUp;

            // Initialize the file path if provided (e.g., from command line or drag-and-drop)
            FilePath = initialFilePath ?? string.Empty;
        }

        /// <summary>
        ///     Cleanup resources when the ViewModel is destroyed.
        /// </summary>
        public void Dispose()
        {
            CancelBackgroundWork();
        }
        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        /// <value>
        /// The information.
        /// </value>
        public string Information
        {
            get => _information;
            set => SetProperty(ref _information, value);
        }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        /// <summary>
        /// Gets or sets the output path.
        /// </summary>
        /// <value>
        /// The output path.
        /// </value>
        public string OutputPath
        {
            get => _outputPath;
            set => SetProperty(ref _outputPath, value);
        }

        /// <summary>
        /// Gets or sets the BMP.
        /// </summary>
        /// <value>
        /// The BMP.
        /// </value>
        public BitmapImage? Bmp
        {
            get => _bmp;
            set => SetProperty(ref _bmp, value);
        }
        /// <summary>
        /// Gets a value indicating whether this instance is working.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is working; otherwise, <c>false</c>.
        /// </value>
        public bool IsWorking { get; private set; }

        /// <summary>
        /// Gets or sets the GIF path.
        /// </summary>
        /// <value>
        /// The GIF path.
        /// </value>
        public string GifPath
        {
            get => _gifPath;
            set => SetProperty(ref _gifPath, value);
        }

        /// <summary>
        /// Gets or sets the observer.
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
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        /// <summary>
        ///     Gets or sets AutoClear. 
        ///     Updates the Global Register immediately when changed in UI.
        /// </summary>
        public bool AutoClear
        {
            get => _autoClear;
            set => SetPropertyAndCallback(ref _autoClear, value, v => SlimViewerRegister.GifCleanUp = v);
        }

        /// <summary>
        /// Gets or sets the current delay.
        /// </summary>
        /// <value>
        /// The current delay.
        /// </value>
        public int CurrentDelay
        {
            get => _currentDelay;
            set => SetProperty(ref _currentDelay, value);
        }


        /// <summary>
        /// Gets the open command.
        /// </summary>
        /// <value>
        /// The open command.
        /// </value>
        public ICommand OpenCommand => GetCommand(ref _openCommand, async _ => await OpenActionAsync());

        /// <summary>
        /// Gets the open folder command.
        /// </summary>
        /// <value>
        /// The open folder command.
        /// </value>
        public ICommand OpenFolderCommand => GetCommand(ref _openFolderCommand, async _ => await OpenFolderActionAsync());

        /// <summary>
        /// Gets the clear command.
        /// </summary>
        /// <value>
        /// The clear command.
        /// </value>
        public ICommand ClearCommand => GetCommand(ref _clearCommand, _ => ClearAction());

        /// <summary>
        /// Gets the save GIF command.
        /// </summary>
        /// <value>
        /// The save GIF command.
        /// </value>
        public ICommand SaveGifCommand => GetCommand(ref _saveGifCommand, async _ => await SaveGifActionAsync());

        /// <summary>
        /// Gets the save images command.
        /// </summary>
        /// <value>
        /// The save images command.
        /// </value>
        public ICommand SaveImagesCommand => GetCommand(ref _saveImagesCommand, async _ => await SaveImagesActionAsync());

        /// <summary>
        /// Gets the set delay command.
        /// </summary>
        /// <value>
        /// The set delay command.
        /// </value>
        public ICommand SetDelayCommand => GetCommand(ref _setDelayCommand, _ => SetDelayAction());

        /// <summary>
        /// Gets the set all delay command.
        /// </summary>
        /// <value>
        /// The set all delay command.
        /// </value>
        public ICommand SetAllDelayCommand => GetCommand(ref _setAllDelayCommand, _ => SetAllDelayAction());

        /// <summary>
        /// Gets the save current frame command.
        /// </summary>
        /// <value>
        /// The save current frame command.
        /// </value>
        public ICommand SaveCurrentFrameCommand => GetCommand(ref _saveCurrentFrameCommand, async _ => await SaveCurrentFrameActionAsync());

        /// <summary>
        ///     Updates the main preview image when a thumbnail is clicked.
        /// </summary>
        /// <param name="id">The ID of the selected thumbnail.</param>
        public void ChangeImage(int id)
        {
            if (!_observer.TryGetValue(id, out var filePath)) return;

            _selectedId = id; // Track selection

            // Sync UI with stored delay (or default if missing)
            if (_delays.TryGetValue(id, out var delay))
            {
                CurrentDelay = delay;
            }
            else
            {
                // Should rarely happen if we init correctly, but safe fallback
                CurrentDelay = 100;
                _delays[id] = 100;
            }

            var bmpWrapper = ImageProcessor.LoadImage(filePath);
            Bmp = bmpWrapper.ToBitmapImage();
            FilePath = filePath;
            Information = ViewResources.BuildImageInformation(filePath, Path.GetFileName(filePath), Bmp);
        }

        /// <summary>
        ///     Opens a GIF file, splits it into frames using the Facade, and saves them to the export folder.
        /// </summary>
        private async Task OpenActionAsync()
        {
            var pathObj = DialogHandler.HandleFileOpen(ViewResources.FileOpenGif, null);
            if (pathObj == null || string.IsNullOrEmpty(pathObj.FilePath)) return;

            // Validate extension
            if (!pathObj.Extension.Equals(ViewResources.GifExt, StringComparison.OrdinalIgnoreCase)) return;

            // Reset cancel token for new work
            CancelBackgroundWork();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                IsWorking = true; // Lock UI
                GifPath = pathObj.FilePath;
                FilePath = GifPath;

                // 1. Prepare Folders (IO Bound)
                await PrepareOutputFoldersAsync(token);

                // 2. Convert GIF to Frames (CPU/IO Bound)
                // Use the Facade to split the GIF into Bitmaps. Returns List<System.Drawing.Bitmap>
                var frames = await Task.Run(() => ImagingFacade.SplitGifAsync(GifPath), token);

                if (frames == null || frames.Count == 0)
                {
                    Information = "No frames extracted.";
                    return;
                }

                // 3. Save Frames to Disk (IO Bound)
                // We need to save these manually so the Thumbnail viewer can pick them up
                await Task.Run(() =>
                {
                    for (int i = 0; i < frames.Count; i++)
                    {
                        token.ThrowIfCancellationRequested();

                        using var frame = frames[i]; // Dispose bitmap after saving to free memory
                        var framePath = Path.Combine(ImageExportPath, $"frame_{i:000}.jpg");

                        // Save using System.Drawing.Imaging
                        frame.Save(framePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }, token);

                // 4. Scan for Resulting Frames (IO Bound)
                // We scan the folder to verify generation and populate the list
                var fileList = await Task.Run(() =>
                    FileHandleSearch.GetFilesByExtensionFullPath(ImageExportPath, ImagingResources.JpgExt, false), token);

                if (fileList == null || fileList.Count == 0)
                {
                    Information = "No frames extracted.";
                    return;
                }

                // 5. Update UI with Thumbnails
                await GenerateThumbViewAsync(fileList, token);
                IsActive = true;
            }
            catch (OperationCanceledException)
            {
                Information = "Operation Cancelled.";
            }
            catch (Exception ex)
            {
                Information = $"Error: {ex.Message}";
            }
            finally
            {
                IsWorking = false; // Unlock UI
            }
        }

        /// <summary>
        ///     Opens a folder of images to combine into a GIF.
        /// </summary>
        private async Task OpenFolderActionAsync()
        {
            var path = DialogHandler.ShowFolder(null);
            if (string.IsNullOrEmpty(path)) return;

            CancelBackgroundWork();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                IsWorking = true;

                // 1. Scan Folder (IO Bound)
                var fileList = await Task.Run(() =>
                    FileHandleSearch.GetFilesByExtensionFullPath(path, ImagingResources.Appendix, false), token);

                if (fileList == null || fileList.Count == 0)
                {
                    Information = ViewResources.MessageFiles;
                    return;
                }

                // Guard against massive folders freezing the logic
                if (fileList.Count >= 800)
                {
                    MessageBox.Show(ViewResources.MessageFiles, ViewResources.MessageInformation);
                    return;
                }

                // 2. Prepare Output
                await PrepareOutputFoldersAsync(token);

                // 3. Generate Thumbnails
                await GenerateThumbViewAsync(fileList, token);

                // 4. Create Preview GIF (CPU Bound)
                // Use Facade: CreateGif(sourceFolder, targetFile)
                // We pass the DIRECTORY (GifExportPath) because the backend appends the filename.
                string fullTargetFile = Path.Combine(GifExportPath, ViewResources.NewGif);

                // Note: If your backend strictly requires a FOLDER path and appends a default name, pass GifExportPath.
                // If it accepts a full file path (as implied by some overloads), pass fullTargetFile.
                // Based on your snippet 'ConvertToGifAction(string folder, string gifPath)' combining paths internally:
                await Task.Run(() => ImagingFacade.CreateGif(path, fullTargetFile), token);

                // The backend likely created "GifExportPath/NewGif.gif" (check ViewResources.NewGif value)
                // Adjust this line if your resource name differs.
                GifPath = fullTargetFile;
                FilePath = GifPath;
                IsActive = File.Exists(GifPath);
            }
            catch (Exception ex)
            {
                Information = $"Error: {ex.Message}";
            }
            finally
            {
                IsWorking = false;
            }
        }

        /// <summary>
        ///     Saves selected frames as a new GIF file.
        /// </summary>
        /// <summary>
        ///     Saves selected frames as a new GIF file.
        /// </summary>
        private async Task SaveGifActionAsync()
        {
            var pathObj = DialogHandler.HandleFileSave(ViewResources.FileOpenGif, OutputPath);
            if (pathObj == null) return;

            // Filter valid selections
            if (Thumbnail.Selection == null || Thumbnail.Selection.IsEmpty) return;

            // 1. Get Sorted Paths (Strings)
            var selectedFiles = Thumbnail.Selection.Keys
                .Where(k => _observer.ContainsKey(k))
                .Select(k => _observer[k])
                .ToList()
                .PathSort();

            if (selectedFiles == null || !selectedFiles.Any()) return;

            IsWorking = true;
            try
            {
                // 2. Perform all Heavy Loading & Processing on Background Thread
                await Task.Run(() =>
                {
                    var frameInfoList = new List<FrameInfo>();

                    try
                    {
                        foreach (var file in selectedFiles)
                        {
                            // A. Lookup Delay (same logic as before)
                            // We need the ID to find the delay in the dictionary
                            var kvp = _observer.FirstOrDefault(x => x.Value == file);
                            int delayMs = 100; // Default

                            if (!kvp.Equals(default(KeyValuePair<int, string>)))
                            {
                                if (_delays.ContainsKey(kvp.Key))
                                {
                                    delayMs = _delays[kvp.Key];
                                }
                            }

                            // B. Load the Bitmap (System.Drawing)
                            // We explicitly create a new Bitmap from the file
                            var bmp = new System.Drawing.Bitmap(file);

                            // C. Create FrameInfo object matching your definition
                            frameInfoList.Add(new FrameInfo
                            {
                                Image = bmp,
                                DelayTime = delayMs / 1000.0, // Convert ms (int) to seconds (double)
                                Description = Path.GetFileName(file)
                            });
                        }

                        // D. Pass the list of loaded images to the Facade
                        ImagingFacade.CreateGif(frameInfoList, pathObj.FilePath);
                    }
                    finally
                    {
                        // E. CRITICAL CLEANUP: Dispose all Bitmaps to prevent GDI+ memory leaks
                        foreach (var frame in frameInfoList)
                        {
                            frame.Image?.Dispose();
                        }
                    }
                });

                Information = "GIF Saved Successfully.";
            }
            catch (Exception ex)
            {
                Information = $"Save Failed: {ex.Message}";
            }
            finally
            {
                IsWorking = false;
            }
        }

        /// <summary>
        ///     Exports selected frames as individual images.
        /// </summary>
        private async Task SaveImagesActionAsync()
        {
            var targetFolder = DialogHandler.ShowFolder(OutputPath);
            if (string.IsNullOrEmpty(targetFolder)) return;

            var selectedFiles = Thumbnail.Selection.Keys
                .Where(k => _observer.ContainsKey(k))
                .Select(k => _observer[k])
                .ToList();

            if (selectedFiles.Count == 0)
            {
                selectedFiles = _observer.Values.ToList();
            }

            IsWorking = true;
            try
            {
                // File Copying is IO Bound -> Task.Run
                await Task.Run(() => FileHandleCopy.CopyFiles(selectedFiles, targetFolder, false));
                Information = "Images Exported Successfully.";
            }
            catch (Exception ex)
            {
                Information = $"Export Failed: {ex.Message}";
            }
            finally
            {
                IsWorking = false;
            }
        }

        /// <summary>
        ///     Manually clears temporary files.
        /// </summary>
        private void ClearAction()
        {
            try
            {
                // We use "Fire and Forget" here because waiting for delete isn't critical for UI
                if (Directory.Exists(ImageExportPath)) Directory.Delete(ImageExportPath, true);
                if (Directory.Exists(GifExportPath)) Directory.Delete(GifExportPath, true);

                // Clear UI State
                Observer = new Dictionary<int, string>();
                Bmp = null;
                GifPath = string.Empty;
                IsActive = false;
                Information = "Temporary files cleared.";
            }
            catch (Exception ex)
            {
                Information = $"Clear Failed: {ex.Message}";
            }
        }

        /// <summary>
        ///     Ensures output directories exist and are clean.
        ///     Run on background thread to avoid UI IO hitching.
        /// </summary>
        private Task PrepareOutputFoldersAsync(CancellationToken token)
        {
            return Task.Run(() =>
            {
                if (!Directory.Exists(OutputPath)) Directory.CreateDirectory(OutputPath);

                // Delete subfolders to ensure clean state
                if (Directory.Exists(ImageExportPath)) Directory.Delete(ImageExportPath, true);
                if (Directory.Exists(GifExportPath)) Directory.Delete(GifExportPath, true);

                // Recreate them
                Directory.CreateDirectory(ImageExportPath);
                Directory.CreateDirectory(GifExportPath);
            }, token);
        }

        /// <summary>
        ///     Converts a list of files to a Dictionary for the Thumbnails control.
        /// </summary>
        // Update GenerateThumbViewAsync
        private async Task GenerateThumbViewAsync(ICollection<string> files, CancellationToken token)
        {
            var dict = await Task.Run(() => files.ToDictionary(), token);

            // Initialize delays for new files (Default 100ms)
            // We do this on the UI thread or safely inside the ViewModel to ensure sync
            var newDelays = new Dictionary<int, int>();
            foreach (var key in dict.Keys)
            {
                newDelays[key] = 100; // Default delay
            }

            // Update UI
            Observer = dict;
            _delays = newDelays;
            _selectedId = -1; // Reset selection
        }

        /// <summary>
        ///     Cancels any pending background tasks.
        /// </summary>
        private void CancelBackgroundWork()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        /// <summary>
        /// Sets the delay action.
        /// </summary>
        private void SetDelayAction()
        {
            if (_selectedId == -1 || !_delays.ContainsKey(_selectedId)) return;

            // Update the dictionary
            _delays[_selectedId] = CurrentDelay;
            Information = $"Frame {_selectedId} delay set to {CurrentDelay}ms.";
        }

        /// <summary>
        /// Sets all delay action.
        /// </summary>
        private void SetAllDelayAction()
        {
            var keys = _delays.Keys.ToList();
            foreach (var key in keys)
            {
                _delays[key] = CurrentDelay;
            }
            Information = $"All frames set to {CurrentDelay}ms.";
        }

        /// <summary>
        /// Exports only the currently selected frame to a user-defined location.
        /// </summary>
        private async Task SaveCurrentFrameActionAsync()
        {
            if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
            {
                Information = "No frame selected to save.";
                return;
            }

            // Reuse your existing HandleFileSave logic
            // We suggest a filename based on the current frame's source name
            var defaultName = Path.GetFileName(FilePath);
            var pathObj = DialogHandler.HandleFileSave(ViewResources.FileGifPng, OutputPath);

            if (pathObj == null) return;

            IsWorking = true;
            try
            {
                await Task.Run(() =>
                {
                    // Simple File.Copy is the most efficient way since the frame already exists in TempGif
                    File.Copy(FilePath, pathObj.FilePath, true);
                });

                Information = $"Frame saved to: {Path.GetFileName(pathObj.FilePath)}";
            }
            catch (Exception ex)
            {
                Information = $"Failed to save frame: {ex.Message}";
            }
            finally
            {
                IsWorking = false;
            }
        }

        /// <summary>
        ///     Helper to create commands with an IsWorking guard.
        /// </summary>
        private ICommand GetCommand(ref ICommand? field, Action<object> execute)
        {
            // We verify !IsWorking to prevent double-clicks crashing file access
            return field ??= new DelegateCommand<object>(execute, _ => !IsWorking);
        }
    }
}