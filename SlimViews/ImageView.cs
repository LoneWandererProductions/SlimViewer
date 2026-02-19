/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/ImageView.cs
 * PURPOSE:     Main ViewModel. Acts as the "Traffic Controller" connecting:
 *              1. The View (UI Binding)
 *              2. The Data Contexts (File, Image, UI State)
 *              3. The Tool State (DrawingState)
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global

#nullable enable
using CommonControls.Images;
using CommonDialogs;
using Exp;
using ExtendedSystemObjects;
using FileHandler;
using Imaging;
using Imaging.Enums;
using SlimViews.Contexts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ViewModel;
using Point = System.Windows.Point;

namespace SlimViews
{
    /// <summary>
    /// Main View for SlimViewer. This class is the central hub that connects the UI, the data contexts, and the drawing state.
    /// </summary>
    /// <seealso cref="ViewModel.ViewModelBase" />
    public sealed class ImageView : ViewModelBase
    {
        // -------------------------------------------------------------------
        // 1. CONTEXTS & STATE (The Data Layer)
        // -------------------------------------------------------------------

        /// <summary>
        /// The Single Source of Truth for Drawing Tools, Colors, and Modes.
        /// </summary>
        public DrawingState MyDrawingState { get; set; } = new DrawingState();

        // Internal Contexts (Data Holders)

        /// <summary>
        /// The image Context, holding all image-related data and operations.
        /// </summary>
        internal readonly ImageContext Image = new();

        /// <summary>
        /// The UI state Context, holding all UI-related state (button visibility, status images, etc).
        /// </summary>
        internal readonly UiState UiState = new();

        /// <summary>
        /// The file context, holding all file-related data (current path, list of files, observer for navigation).
        /// </summary>
        internal readonly FileContext FileContext = new();

        /// <summary>
        /// Commands handler for this view.
        /// </summary>
        /// <value>
        /// The commands.
        /// </value>
        public ImageViewCommands Commands { get; }

        /// <summary>
        /// Global Key Bindings.
        /// </summary>
        /// <value>
        /// The command bindings.
        /// </value>
        public Dictionary<Tuple<ModifierKeys, Key>, ICommand> CommandBindings { get; set; }

        // -------------------------------------------------------------------
        // 2. UI BINDING PROPERTIES (Proxies to Contexts)
        // -------------------------------------------------------------------

        /// <summary>
        /// Controls the actual interaction mode of the ImageZoom control (Pan, Rect, FreeForm).
        /// This is updated automatically when MyDrawingState changes.
        /// </summary>
        private ImageZoomTools _imageZoomTool;

        /// <summary>
        /// Gets or sets the image zoom tool.
        /// </summary>
        /// <value>
        /// The image zoom tool.
        /// </value>
        public ImageZoomTools ImageZoomTool
        {
            get => _imageZoomTool;
            set => SetProperty(ref _imageZoomTool, value, nameof(ImageZoomTool));
        }

        // --- File Context Proxies ---

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get => FileContext.Count;
            set
            {
                if (FileContext.Count == value) return;
                FileContext.Count = value;
                OnPropertyChanged(nameof(Count));
                NavigationLogic();
            }
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get => FileContext.FileName;
            set
            {
                if (FileContext.FileName == value) return;
                FileContext.FileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        /// <summary>
        /// Gets or sets the observer.
        /// </summary>
        /// <value>
        /// The observer.
        /// </value>
        public Dictionary<int, string?> Observer
        {
            get => FileContext.Observer;
            set
            {
                if (FileContext.Observer == value) return;
                FileContext.Observer = value;
                OnPropertyChanged(nameof(Observer));
                NavigationLogic();
            }
        }

        /// <summary>
        /// Gets or sets the GIF path.
        /// </summary>
        /// <value>
        /// The GIF path.
        /// </value>
        public string? GifPath
        {
            get => FileContext.GifPath;
            set
            {
                if (FileContext.GifPath == value) return;
                FileContext.GifPath = value;
                OnPropertyChanged(nameof(GifPath));
            }
        }

        // --- Image Context Proxies ---

        /// <summary>
        /// Gets or sets a value indicating whether this instance is image active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is image active; otherwise, <c>false</c>.
        /// </value>
        public bool IsImageActive
        {
            get => Image.IsImageActive;
            set
            {
                if (Image.IsImageActive == value) return;
                Image.IsImageActive = value;
                OnPropertyChanged(nameof(IsImageActive));
            }
        }

        /// <summary>
        /// Gets or sets the BMP.
        /// </summary>
        /// <value>
        /// The BMP.
        /// </value>
        public BitmapImage? Bmp
        {
            get => Image.BitmapImage;
            set
            {
                if (Image.BitmapImage == value) return;
                Image.BitmapImage = value;
                OnPropertyChanged(nameof(Bmp));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [compress cif].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [compress cif]; otherwise, <c>false</c>.
        /// </value>
        public bool CompressCif
        {
            get => Image.CompressCif;
            set { Image.CompressCif = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        /// <value>
        /// The information.
        /// </value>
        public string Information
        {
            get => Image.Information;
            set { Image.Information = value; OnPropertyChanged(); }
        }

        // --- UI State Proxies ---

        /// <summary>
        /// Gets or sets the left button visibility.
        /// </summary>
        /// <value>
        /// The left button visibility.
        /// </value>
        public Visibility LeftButtonVisibility
        {
            get => UiState.LeftButtonVisibility;
            set { UiState.LeftButtonVisibility = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the right button visibility.
        /// </summary>
        /// <value>
        /// The right button visibility.
        /// </value>
        public Visibility RightButtonVisibility
        {
            get => UiState.RightButtonVisibility;
            set { UiState.RightButtonVisibility = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the thumbnail visibility.
        /// </summary>
        /// <value>
        /// The thumbnail visibility.
        /// </value>
        public Visibility ThumbnailVisibility
        {
            get => UiState.ThumbnailVisibility;
            set { UiState.ThumbnailVisibility = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the status image.
        /// </summary>
        /// <value>
        /// The status image.
        /// </value>
        public string StatusImage
        {
            get => UiState.StatusImage;
            set { UiState.StatusImage = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use sub folders].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use sub folders]; otherwise, <c>false</c>.
        /// </value>
        public bool UseSubFolders
        {
            get => UiState.UseSubFolders;
            set { UiState.UseSubFolders = value; OnPropertyChanged(); SlimViewerRegister.MainSubFolders = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic clean].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic clean]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoClean
        {
            get => UiState.AutoClean;
            set { UiState.AutoClean = value; OnPropertyChanged(); SlimViewerRegister.MainAutoClean = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is thumbs visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is thumbs visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsThumbsVisible
        {
            get => UiState.IsThumbsVisible;
            set { UiState.IsThumbsVisible = value; OnPropertyChanged(); NavigationLogic(); }
        }

        // --- Transient / Command Properties ---

        /// <summary>
        /// The pixel width
        /// </summary>
        private int _pixelWidth = 10;

        /// <summary>
        /// Gets or sets the width of the pixel.
        /// </summary>
        /// <value>
        /// The width of the pixel.
        /// </value>
        public int PixelWidth
        {
            get => _pixelWidth;
            set => SetProperty(ref _pixelWidth, value >= 2 ? value : 2);
        }

        /// <summary>
        /// The similarity
        /// </summary>
        private int _similarity;

        /// <summary>
        /// Gets or sets the similarity.
        /// </summary>
        /// <value>
        /// The similarity.
        /// </value>
        public int Similarity
        {
            get => _similarity;
            set
            {
                if (value is < 0 or > 100) return;
                SetProperty(ref _similarity, value);
                Image.Similarity = value;
                SlimViewerRegister.MainSimilarity = value;
            }
        }

        // -------------------------------------------------------------------
        // 3. INITIALIZATION
        // -------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageView"/> class.
        /// </summary>
        public ImageView()
        {
            Commands = new ImageViewCommands(this);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageView"/> class.
        /// </summary>
        /// <param name="subFolders">if set to <c>true</c> [sub folders].</param>
        /// <param name="compressCif">if set to <c>true</c> [compress cif].</param>
        /// <param name="similarity">The similarity.</param>
        /// <param name="autoClean">if set to <c>true</c> [automatic clean].</param>
        /// <param name="imageZoom">The image zoom.</param>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="thumb">The thumb.</param>
        /// <param name="colorPick">The color pick.</param>
        public ImageView(bool subFolders, bool compressCif, int similarity, bool autoClean,
                         ImageZoom imageZoom, Window mainWindow, Thumbnails thumb, ColorPickerMenu colorPick) : this()
        {
            UseSubFolders = subFolders;
            CompressCif = compressCif;
            Similarity = similarity;
            AutoClean = autoClean;

            // Assign Controls to State (No DataBinding possible for raw Controls)
            UiState.ImageZoomControl = imageZoom;
            UiState.Main = mainWindow;
            UiState.Thumb = thumb;
            UiState.Picker = colorPick;

            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            Image.CustomImageFormat = new CustomImageFormat();

            // Set Initial UI State
            LeftButtonVisibility = RightButtonVisibility = Visibility.Hidden;
            ThumbnailVisibility = Visibility.Visible;
            IsImageActive = false;

            // CRITICAL: Subscribe to the DrawingState. 
            // When user clicks the Toolbar, we map that state to the View behavior.
            MyDrawingState.ToolOrModeChanged += (s, e) => MapStateToZoomTool();

            CommandBindings = new Dictionary<Tuple<ModifierKeys, Key>, ICommand>
            {
                // Modified keys for file operations
                { Tuple.Create(ModifierKeys.Control, Key.O), Commands.Open },
                { Tuple.Create(ModifierKeys.Control, Key.S), Commands.Save },
        
                // Single keys for fast viewer navigation (ModifierKeys.None)
                { Tuple.Create(ModifierKeys.None, Key.Delete), Commands.Delete },
                { Tuple.Create(ModifierKeys.None, Key.F5), Commands.Refresh },
                { Tuple.Create(ModifierKeys.None, Key.Left), Commands.Previous },
                { Tuple.Create(ModifierKeys.None, Key.Right), Commands.Next }
            };
        }

        // -------------------------------------------------------------------
        // 4. LOGIC: MAPPING STATE TO BEHAVIOR
        // -------------------------------------------------------------------

        /// <summary>
        /// Maps the abstract DrawingState (Pencil, Shape, Mode) to the concrete Tool 
        /// required by the ImageZoom control (FreeForm, Rectangle, Move).
        /// </summary>
        private void MapStateToZoomTool()
        {
            switch (MyDrawingState.ActiveTool)
            {
                case DrawTool.Pencil:
                case DrawTool.Eraser:
                    // Pencil/Eraser behave like FreeForm drawing
                    ImageZoomTool = ImageZoomTools.FreeForm;
                    break;

                case DrawTool.Shape:
                    // Translate the specific ShapeType to the Zoom Control's tool
                    switch (MyDrawingState.SelectedShape)
                    {
                        case ShapeType.Rectangle:
                            ImageZoomTool = ImageZoomTools.Rectangle;
                            break;
                        case ShapeType.Ellipse:
                            ImageZoomTool = ImageZoomTools.Ellipse;
                            break;
                        case ShapeType.Freeform:
                            ImageZoomTool = ImageZoomTools.FreeForm;
                            break;
                        default:
                            ImageZoomTool = ImageZoomTools.Move;
                            break;
                    }
                    break;

                default:
                    // Default to Pan/Move if no tool is active
                    ImageZoomTool = ImageZoomTools.Move;
                    break;
            }
        }

        /// <summary>
        /// Action triggered when a point is clicked (Pencil drawing, Color picking).
        /// </summary>
        /// <param name="wPoint">The w point.</param>
        internal void SelectedPointAction(Point wPoint)
        {
            var point = new System.Drawing.Point((int)wPoint.X, (int)wPoint.Y);

            // 1. Pencil Logic
            if (MyDrawingState.ActiveTool == DrawTool.Pencil)
            {
                var color = ColorTranslator.FromHtml(MyDrawingState.BrushColor);
                Image.Bitmap = ImageProcessor.SetPixel(Image.Bitmap, point, color, (int)MyDrawingState.BrushSize);
                Bmp = Image.BitmapSource;
            }

            // 2. Add Color Picker logic here if you implement an Eyedropper tool
        }

        /// <summary>
        /// Action triggered when a Shape/Frame selection is completed.
        /// Applies the current Mode (Fill, Texture, Filter) to the area.
        /// </summary>
        /// <param name="frame">The frame.</param>
        internal void SelectedFrameAction(SelectionFrame frame)
        {
            // A. Eraser is a global tool override
            if (MyDrawingState.ActiveTool == DrawTool.Eraser)
            {
                Image.Bitmap = ImageProcessor.EraseImage(frame, Image.Bitmap);
                Bmp = Image.BitmapSource;
                return;
            }

            // B. Shape Tools - Apply the Active Mode
            if (MyDrawingState.ActiveTool == DrawTool.Shape)
            {
                switch (MyDrawingState.ActiveAreaMode)
                {
                    case AreaMode.Fill:
                        var color = ColorTranslator.FromHtml(MyDrawingState.Fill.Color);
                        Image.Bitmap = ImageProcessor.FillArea(Image.Bitmap, frame, color);
                        break;

                    case AreaMode.Texture:
                        // Convert String Name -> Enum safely
                        if (!string.IsNullOrEmpty(MyDrawingState.Texture.TextureName) &&
                            Enum.TryParse(MyDrawingState.Texture.TextureName, true, out TextureType texEnum))
                        {
                            Image.Bitmap = ImageProcessor.FillTexture(Image.Bitmap, frame, texEnum);
                        }
                        break;

                    case AreaMode.Filter:
                        // Convert String Name -> Enum safely
                        if (!string.IsNullOrEmpty(MyDrawingState.Filter.FilterName) &&
                            Enum.TryParse(MyDrawingState.Filter.FilterName, true, out FiltersType filterEnum))
                        {
                            Image.Bitmap = ImageProcessor.FillFilter(Image.Bitmap, frame, filterEnum);
                        }
                        break;

                    case AreaMode.Erase:
                        Image.Bitmap = ImageProcessor.EraseImage(frame, Image.Bitmap);
                        break;
                }

                Bmp = Image.BitmapSource;
            }
        }

        /// <summary>
        /// Syncs the Color Picker selection back to the Drawing State.
        /// </summary>
        /// <param name="colorHsv">The color HSV.</param>
        internal void ColorChangedAction(ColorHsv colorHsv)
        {
            // Update state
            MyDrawingState.BrushColor = colorHsv.Hex;

            // If Fill mode is active, sync the fill color too
            if (MyDrawingState.ActiveAreaMode == AreaMode.Fill)
            {
                MyDrawingState.Fill.Color = colorHsv.Hex;
            }
        }

        // -------------------------------------------------------------------
        // 5. FILE NAVIGATION & LOADING LOGIC
        // -------------------------------------------------------------------

        /// <summary>
        /// Determines whether this instance can run the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>
        ///   <c>true</c> if this instance can run the specified argument; otherwise, <c>false</c>.
        /// </returns>
        public bool CanRun(object? arg) => true;

        // Navigation Actions
        /// <summary>
        /// Next Image action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void NextAction(object obj)
        {
            if (Observer == null || !Observer.Any()) return;
            ChangeImage(Utility.GetNextElement(FileContext.CurrentId, Observer.Keys.ToList()));
            UiState.Thumb.Next();
            NavigationLogic();
        }

        /// <summary>
        /// Previous Image action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void PreviousAction(object obj)
        {
            if (Observer == null || !Observer.Any()) return;
            ChangeImage(Utility.GetPreviousElement(FileContext.CurrentId, Observer.Keys.ToList()));
            UiState.Thumb.Previous();
            NavigationLogic();
        }
        /// <summary>
        /// Window Actions.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void CloseAction(object obj)
        {
            var config = SlimViewerRegister.GetRegister();
            if (UiState.ImageZoomControl != null)
                config.MainAutoPlayGif = UiState.ImageZoomControl.AutoplayGifImage;

            Config.SetConfig(config);

            if (AutoClean)
            {
                var fileCmd = new FileProcessingCommands();
                fileCmd.CleanTempFolder(true);
            }
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Refreshes the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void RefreshAction(object obj)
        {
            FileContext.CurrentId = -1;
            FileContext.FilePath = string.Empty;
            Bmp = null;
            GifPath = null;

            if (Directory.Exists(FileContext.CurrentPath))
                LoadThumbs(FileContext.CurrentPath);
            else
                Observer = null;
        }

        /// <summary>
        /// Clears the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void ClearAction(object obj)
        {
            if (!Observer.ContainsKey(FileContext.CurrentId)) return;

            UiState.Thumb.RemoveSingleItem(FileContext.CurrentId);
            if (Count > 0) Count--;

            Image.Clear();
            GifPath = null;
            FileContext.GifPath = null;

            NextAction(this);
        }

        /// <summary>
        /// File Dialog Actions. These use the DialogHandler service to get file paths, then call the appropriate loading and thumbnail generation methods.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void OpenAction(object obj)
        {
            var pathObj = DialogHandler.HandleFileOpen(ViewResources.FileOpen, FileContext.CurrentPath);
            if (string.IsNullOrEmpty(pathObj?.FilePath)) return;

            FileContext.CurrentPath = pathObj.Folder;

            if (string.Equals(pathObj.Extension, ViewResources.CbzExt, StringComparison.OrdinalIgnoreCase))
            {
                GenerateCbrView(pathObj);
                return;
            }

            if (!ImagingResources.Appendix.Contains(pathObj.Extension?.ToLower()))
            {
                _ = MessageBox.Show(ViewResources.ErrorFileNotSupported + pathObj.Extension, ViewResources.ErrorMessage);
                return;
            }

            GenerateView(pathObj.FilePath);
            LoadThumbs(pathObj.Folder, pathObj.FilePath);
            if (Image.HasImage) IsImageActive = true;
        }

        /// <summary>
        /// Opens the CBZ action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void OpenCbzAction(object obj)
        {
            var pathObj = DialogHandler.HandleFileOpen(ViewResources.FileOpenCbz, FileContext.CurrentPath);
            if (pathObj == null || !File.Exists(pathObj.FilePath)) return;
            GenerateCbrView(pathObj);
            if (Image.HasImage) IsImageActive = true;
        }

        /// <summary>
        /// Opens the cif action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void OpenCifAction(object obj)
        {
            var pathObj = DialogHandler.HandleFileOpen(ViewResources.FileOpenCif, FileContext.CurrentPath);
            if (pathObj == null || !File.Exists(pathObj.FilePath)) return;

            Image.Bitmap = Image.CustomImageFormat.GetImageFromCif(pathObj.FilePath);
            if (Image.Bitmap == null) return;

            IsImageActive = true;
            Bmp = Image.BitmapSource;
            FileName = Path.GetFileName(FileContext.FilePath);
            Information = ViewResources.BuildImageInformation(FileContext.FilePath, FileName, Bmp);
        }

        /// <summary>
        /// Folders the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void FolderAction(object obj)
        {
            var path = DialogHandler.ShowFolder(FileContext.CurrentPath);
            if (!Directory.Exists(path)) return;
            LoadThumbs(path);
            if (!string.IsNullOrEmpty(path)) IsImageActive = true;
        }

        /// <summary>
        /// Explorers the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void ExplorerAction(object obj)
        {
            if (!Directory.Exists(FileContext.CurrentPath)) return;
            var argument = !File.Exists(FileContext.FilePath) ? FileContext.CurrentPath : ViewResources.Select + FileContext.FilePath;
            _ = Process.Start(ViewResources.Explorer, argument);
        }

        /// <summary>
        /// Exports the string action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void ExportStringAction(object obj) => ImageProcessor.ExportString(Image.Bitmap);

        // -------------------------------------------------------------------
        // 6. HELPER METHODS (Loading & Thumbnails)
        // -------------------------------------------------------------------

        /// <summary>
        /// Changes the image.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void ChangeImage(int id)
        {
            if (!Observer.ContainsKey(id)) return;
            FileContext.CurrentId = id;
            NavigationLogic();
            GenerateView(Observer[id]);
        }

        /// <summary>
        /// Changes the image.
        /// </summary>
        /// <param name="files">The files.</param>
        public void ChangeImage(IEnumerable<string> files)
        {
            FileContext.Files = files.ToList();
            Count = FileContext.Files.Count;

            try { FileContext.CurrentPath = Path.GetDirectoryName(FileContext.Files[0]); }
            catch (ArgumentException ex) { Trace.WriteLine(ex); MessageBox.Show(ex.ToString(), ViewResources.ErrorMessage); return; }

            Bmp = null; GifPath = null; FileContext.CurrentId = -1;

            _ = GenerateThumbView(FileContext.Files);
            Image.Information = string.Concat(ViewResources.DisplayImages, FileContext.Files.Count);
        }

        /// <summary>
        /// Changes the image.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void ChangeImage(string? filePath)
        {
            if (!CanLoadFile(filePath)) return;
            GenerateView(filePath);

            var folder = Path.GetDirectoryName(filePath);
            if (folder != FileContext.CurrentPath && folder !=null) LoadThumbs(folder, filePath);

            FileContext.CurrentId = FileContext.CurrentIdGetIdByFilePath(filePath);
        }

        /// <summary>
        /// Reloads the view with a new list of files and a specific target image.
        /// Used by Rename/Convert commands to refresh the state.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="info">The information.</param>
        internal void ChangeImage(IEnumerable<string?> files, string? filePath, string info)
        {
            // 1. Validate
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

            // 2. Update File List
            // We convert to List to ensure immediate evaluation
            FileContext.Files = files.ToList();
            Count = FileContext.Files.Count;

            // 3. Update Thumbnails (Fire and forget task)
            _ = GenerateThumbView(FileContext.Files);

            // 4. Load the Main Image
            // Use the async generator to keep UI responsive
            _ = GenerateImageAsync(filePath);

            // 5. Update IDs and Info
            FileContext.CurrentId = FileContext.CurrentIdGetIdByFilePath(filePath);
            Information = info;

            // 6. Refresh Navigation Buttons
            NavigationLogic();
        }

        /// <summary>
        /// Saves the image using the external ImageProcessor.
        /// Called by FileProcessingCommands.Save.
        /// </summary>
        /// <param name="path">The full file path.</param>
        /// <param name="extension">The file extension (e.g., ".png").</param>
        /// <param name="bitmap">The GDI+ Bitmap (System.Drawing.Bitmap).</param>
        /// <returns>True if successful, False otherwise.</returns>
        internal bool SaveImage(string path, string extension, System.Drawing.Bitmap bitmap)
        {
            // Update UI Status to "Working" (Red)
            StatusImage = UiState.RedIconPath;

            // Call the lower-level processor logic
            bool success = ImageProcessor.SaveImage(path, extension, bitmap);

            // Update UI Status to "Done" (Green)
            StatusImage = UiState.GreenIconPath;

            return success;
        }

        // Logic Helpers
        private bool CanLoadFile(string? path) => !string.IsNullOrEmpty(path) && File.Exists(path) && ImagingResources.Appendix.Any(path.EndsWith);

        /// <summary>
        /// Generates the view.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        internal void GenerateView(string? filePath)
        {
            var info = FileHandleSearch.GetFileDetails(filePath);
            if (info == null)
            {
                Bmp = null; GifPath = null;
                LoadThumbs(FileContext.CurrentPath);
                return;
            }
            GenerateImageAsync(filePath);
        }

        /// <summary>
        /// Generates the image asynchronous.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private async Task GenerateImageAsync(string? filePath)
        {
            try
            {
                var ext = Path.GetExtension(filePath);
                if (ext.Equals(ImagingResources.GifExt, StringComparison.OrdinalIgnoreCase))
                {
                    if (GifPath?.Equals(filePath, StringComparison.OrdinalIgnoreCase) == true) return;
                    GifPath = filePath;
                    var info = ImageGifHandler.GetImageInfo(filePath);
                    Information = ViewResources.BuildGifInformation(filePath, info);
                }
                else
                {
                    Image.Bitmap = await Task.Run(() => ImageProcessor.Render.GetOriginalBitmap(filePath));
                    Bmp = Image.BitmapSource; // Trigger UI update
                    GifPath = null;
                    Information = ViewResources.BuildImageInformation(filePath, FileName, Bmp);
                }
                FileContext.FilePath = filePath;
                FileName = Path.GetFileName(filePath);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), ViewResources.ErrorMessage);
            }
        }

        /// <summary>
        /// Loads the thumbs.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="filePath">The file path.</param>
        internal void LoadThumbs(string folder, string? filePath = null)
        {
            GenerateThumbView(folder);
            if (!string.IsNullOrEmpty(filePath))
                FileContext.CurrentId = FileContext.CurrentIdGetIdByFilePath(filePath);
            else
            {
                FileContext.CurrentId = -1; Bmp = null; GifPath = null;
            }
        }

        /// <summary>
        /// Generates the thumb view.
        /// </summary>
        /// <param name="folder">The folder.</param>
        private void GenerateThumbView(string folder)
        {
            FileContext.CurrentPath = folder;
            StatusImage = UiState.RedIconPath;
            FileContext.Files = FileHandleSearch.GetFilesByExtensionFullPath(folder, ImagingResources.Appendix, UiState.UseSubFolders);

            if (FileContext.IsFilesEmpty)
            {
                Count = 0; Observer = null; GifPath = null; Bmp = null; return;
            }

            NavigationLogic();
            Count = FileContext.Files.Count;
            FileContext.Files = FileContext.FilesSorted;
            _ = GenerateThumbView(FileContext.Files).ConfigureAwait(false);
        }

        /// <summary>
        /// Generates the thumb view.
        /// </summary>
        /// <param name="lst">List of Image files..</param>
        private async Task GenerateThumbView(IReadOnlyCollection<string?> lst)
        {
            if (!IsThumbsVisible) return;
            StatusImage = UiState.RedIconPath;
            _ = await Task.Run(() => Observer = lst.ToDictionary()).ConfigureAwait(false);
        }

        /// <summary>
        /// Generates the CBR view.
        /// </summary>
        /// <param name="pathObj">The path object.</param>
        private void GenerateCbrView(PathObject pathObj)
        {
            if (pathObj == null) return;
            UiState.UseSubFolders = true;
            var folder = ImageProcessor.UnpackFolder(pathObj.FilePath, pathObj.FileNameWithoutExt);
            if (!string.IsNullOrEmpty(folder)) FileContext.CurrentPath = folder;
            var file = ImageProcessor.UnpackFile(folder);
            if (file == null) return;
            GenerateView(file);
            LoadThumbs(folder, file);
        }

        /// <summary>
        /// Navigations the logic.
        /// </summary>
        private void NavigationLogic()
        {
            if (FileContext.Count <= 1)
                UiState.HideButtons();
            else
            {
                RightButtonVisibility = FileContext.CurrentId == FileContext.Count - 1 ? Visibility.Hidden : Visibility.Visible;
                LeftButtonVisibility = FileContext.CurrentId <= 0 ? Visibility.Hidden : Visibility.Visible;
            }
            ThumbnailVisibility = UiState.ThumbnailState();
            IsImageActive = Image.Bitmap != null;
        }

        public void ImageLoadedCommandAction(object obj)
        {
            if (!string.IsNullOrEmpty(StatusImage)) StatusImage = UiState.GreenIconPath;
        }

        /// <summary>
        /// Thumbs the image clicked action.
        /// </summary>
        /// <param name="obj">The <see cref="ImageEventArgs"/> instance containing the event data.</param>
        internal void ThumbImageClickedAction(ImageEventArgs obj) { ChangeImage(obj.Id); }
    }
}