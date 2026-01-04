/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/ImageView.cs
 * PURPOSE:     View Model for the SlimViewer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global, if we make it private the Property Changed event will not be triggered in the Window
// ReSharper disable MemberCanBeInternal, must be public, else the View Model won't work
// ReSharper disable BadBracesSpaces
// ReSharper disable MissingSpace
// ReSharper disable WrongIndentSize

#nullable enable
using CommonControls;
using CommonDialogs;
using Exp;
using ExtendedSystemObjects;
using FileHandler;
using Imaging;
using Imaging.Enums;
using SlimControls;
using SlimViews.Contexts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

// TODO Save and Export Settings
// TODO Activity Monitor
// TODO improve Image Compare Interface
// TODO add Zoom lock and rotate as it's own Control
// TODO add more User Feedback
// TODO add layer functions
// TODO improve tooling
// Todo improve textures and filters

namespace SlimViews
{
    /// <inheritdoc />
    /// <summary>
    ///     Basic View and main entry Point
    /// </summary>
    /// <seealso cref="T:ViewModel.ViewModelBase" />
    public sealed class ImageView : ViewModelBase
    {
        /// <summary>
        ///     The pixel width
        /// </summary>
        private int _pixelWidth;

        /// <summary>
        ///     The selected filter
        /// </summary>
        private string _selectedFilter;

        /// <summary>
        /// The selected texture
        /// </summary>
        private string _selectedTexture;

        /// <summary>
        ///     The selected tool
        /// </summary>
        private ImageTools _selectedTool;

        /// <summary>
        ///     The tolerance
        /// </summary>
        private int _tolerance;

        /// <summary>
        /// The erase radius
        /// </summary>
        private double _eraseRadius;

        /// <summary>
        /// The image zoom tool
        /// </summary>
        private ImageZoomTools _imageZoomTool;

        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <value>
        /// The commands.
        /// </value>
        public ImageViewCommands Commands { get; }

        /// <summary>
        /// Gets or sets the state of my drawing.
        /// </summary>
        /// <value>
        /// The state of my drawing.
        /// </value>
        public DrawingState MyDrawingState { get; set; } = new DrawingState();

        /// <summary>
        /// Determines whether this instance can run the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>
        ///   <c>true</c> if this instance can run the specified argument; otherwise, <c>false</c>.
        /// </returns>
        public bool CanRun(object? arg) => CanExecute(arg);

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageView"/> class.
        /// </summary>
        public ImageView()
        {
            Commands = new ImageViewCommands(this);

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageView" /> class.
        /// Initiates all necessary Collections as well
        /// </summary>
        /// <param name="subFolders">if set to <c>true</c> [sub folders].</param>
        /// <param name="compressCif">if set to <c>true</c> [compress cif].</param>
        /// <param name="similarity">The similarity.</param>
        /// <param name="autoClean">if set to <c>true</c> [automatic clean].</param>
        /// <param name="imageZoom">The image zoom.</param>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="thumb">The thumb.</param>
        /// <param name="colorPick">The color pick.</param>
        public ImageView(
            bool subFolders,
            bool compressCif,
            int similarity,
            bool autoClean,
            ImageZoom imageZoom,
            Window mainWindow,
            Thumbnails thumb,
            ColorPickerMenu colorPick)
            : this()
        {
            UseSubFolders = subFolders;
            CompressCif = compressCif;
            Similarity = similarity;
            AutoClean = autoClean;
            UiState.ImageZoomControl = imageZoom;
            UiState.Main = mainWindow;
            UiState.Thumb = thumb;
            UiState.Picker = colorPick;

            Initialize();
        }

        /// <summary>
        ///     Gets or sets the current filter.
        /// </summary>
        /// <value>
        ///     The current filter.
        /// </value>
        private FiltersType CurrentFilter { get; set; }

        /// <summary>
        ///     Gets or sets the current texture.
        /// </summary>
        /// <value>
        ///     The current texture.
        /// </value>
        private TextureType CurrentTexture { get; set; }

        /// <summary>
        ///     Gets the command bindings.
        /// </summary>
        /// <value>
        ///     The command bindings.
        /// </value>
        public Dictionary<Key, ICommand> CommandBindings { get; set; }

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        /// <value>
        ///     The color.
        /// </value>
        public ColorHsv Color { get; set; }

        /// <summary>
        ///     Gets or sets the tool code.
        /// </summary>
        /// <value>
        ///     The tool code.
        /// </value>
        public EnumTools ToolCode { get; set; }

        /// <summary>
        ///     Gets or sets the selected tool.
        /// </summary>
        /// <value>
        ///     The selected tool.
        /// </value>
        public ImageTools SelectedTool
        {
            get => _selectedTool;
            set => SetProperty(ref _selectedTool, value, nameof(SelectedTool));
        }

        /// <summary>
        ///     Gets or sets the size of the brush.
        /// </summary>
        /// <value>
        ///     The size of the brush.
        /// </value>
        public int BrushSize
        {
            get => Image.BrushSize;
            set
            {
                if (Image.BrushSize != value)
                {
                    Image.BrushSize = value;
                    OnPropertyChanged(nameof(BrushSize)); // notify WPF
                }
            }
        }

        /// <summary>
        ///     Gets or sets the erase radius.
        /// </summary>
        /// <value>
        ///     The erase radius.
        /// </value>
        public double EraseRadius
        {
            get => _eraseRadius;
            set => SetProperty(ref _eraseRadius, value, nameof(EraseRadius));
        }

        /// <summary>
        ///     Gets or sets the tolerance.
        /// </summary>
        /// <value>
        ///     The tolerance.
        /// </value>
        public int Tolerance
        {
            get => _tolerance;
            set => SetProperty(ref _tolerance, value, nameof(Tolerance));
        }

        /// <summary>
        ///     Gets or sets the selected texture.
        /// </summary>
        /// <value>
        ///     The selected texture.
        /// </value>
        public string SelectedTexture
        {
            get => _selectedTexture;
            set => SetProperty(ref _selectedTexture, value, nameof(SelectedTexture));
        }

        /// <summary>
        ///     Gets or sets the selected filter.
        /// </summary>
        /// <value>
        ///     The selected filter.
        /// </value>
        public string SelectedFilter
        {
            get => _selectedFilter;
            set => SetProperty(ref _selectedFilter, value, nameof(SelectedFilter));
        }

        /// <summary>
        ///     Gets or sets the basic File information.
        /// </summary>
        /// <value>
        ///     The information.
        /// </value>
        public string Information
        {
            get => Image.Information;
            set
            {
                if (Image.Information != value)
                {
                    Image.Information = value;
                    OnPropertyChanged(nameof(Information)); // notify WPF
                }
            }
        }

        /// <summary>
        ///     Gets or sets the similarity. In percent, other values that are bigger or smaller won't be accepted.
        /// </summary>
        /// <value>
        ///     The similarity.
        /// </value>
        public int Similarity
        {
            get => Image.Similarity;
            set
            {
                if (Image.Similarity == value) return;

                if (value is >= 0 and <= 100) // Only set if value is within valid range
                {
                    Image.Similarity = value;
                    OnPropertyChanged(nameof(Similarity));
                    SlimViewerRegister.MainSimilarity = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the width of the pixel.
        /// </summary>
        /// <value>
        ///     The width of the pixel.
        /// </value>
        public int PixelWidth
        {
            get => _pixelWidth;
            set
            {
                if (value >= 2) // Only set if value is valid
                    SetProperty(ref _pixelWidth, value, nameof(PixelWidth));
            }
        }

        /// <summary>
        ///     Gets or sets the File count.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        public int Count
        {
            get => FileContext.Count;
            set
            {
                if (FileContext.Count != value)
                {
                    FileContext.Count = value;
                    OnPropertyChanged(nameof(Count)); // notify WPF
                    NavigationLogic();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the Filename.
        /// </summary>
        /// <value>
        ///     The name of the file.
        /// </value>
        public string FileName
        {
            get => FileContext.FileName;
            set
            {
                if (FileContext.FileName != value)
                {
                    FileContext.FileName = value;
                    OnPropertyChanged(nameof(FileName)); // notify WPF
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is image active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is image active; otherwise, <c>false</c>.
        /// </value>
        public bool IsImageActive
        {
            get => Image.IsImageActive;
            set
            {
                if (Image.IsImageActive != value)
                {
                    Image.IsImageActive = value;
                    OnPropertyChanged(nameof(IsImageActive)); // notify WPF
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [left button visibility].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [left button visibility]; otherwise, <c>false</c>.
        /// </value>
        public Visibility LeftButtonVisibility
        {
            get => UiState.LeftButtonVisibility;
            set
            {
                if (UiState.LeftButtonVisibility != value)
                {
                    UiState.LeftButtonVisibility = value;
                    OnPropertyChanged(nameof(LeftButtonVisibility)); // notify WPF
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [right button visibility].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [right button visibility]; otherwise, <c>false</c>.
        /// </value>
        public Visibility RightButtonVisibility
        {
            get => UiState.RightButtonVisibility;
            set
            {
                if (UiState.RightButtonVisibility != value)
                {
                    UiState.RightButtonVisibility = value;
                    OnPropertyChanged(nameof(RightButtonVisibility)); // notify WPF
                }
            }
        }

        /// <summary>
        ///     Gets or sets the thumbnail visibility.
        /// </summary>
        /// <value>
        ///     The thumbnail visibility.
        /// </value>
        public Visibility ThumbnailVisibility
        {
            get => UiState.ThumbnailVisibility;
            set
            {
                if (UiState.ThumbnailVisibility != value)
                {
                    UiState.ThumbnailVisibility = value;
                    OnPropertyChanged(nameof(ThumbnailVisibility)); // notify WPF
                }
            }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether [sub folders].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [sub folders]; otherwise, <c>false</c>.
        /// </value>
        public bool UseSubFolders
        {
            get => UiState.UseSubFolders;
            set
            {
                if (UiState.UseSubFolders != value)
                {
                    UiState.UseSubFolders = value;
                    OnPropertyChanged(nameof(UseSubFolders)); // notify WPF
                    SlimViewerRegister.MainSubFolders = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [automatic clean].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [automatic clean]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoClean
        {
            get => UiState.AutoClean;
            set
            {
                if (UiState.AutoClean == value) return;

                UiState.AutoClean = value;
                OnPropertyChanged(nameof(AutoClean)); // notify WPF
                SlimViewerRegister.MainAutoClean = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ImageView" /> shows Thumbnails.
        /// </summary>
        /// <value>
        ///     <c>true</c> if thumbs; otherwise, <c>false</c>.
        /// </value>
        public bool IsThumbsVisible
        {
            get => UiState.IsThumbsVisible;
            set
            {
                if (UiState.IsThumbsVisible != value)
                {
                    UiState.IsThumbsVisible = value;
                    OnPropertyChanged(nameof(IsThumbsVisible)); // notify WPF
                    NavigationLogic();
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ImageView" /> compresses the new CIF format.
        /// </summary>
        /// <value>
        ///     <c>true</c> if compress; otherwise, <c>false</c>.
        /// </value>
        public bool CompressCif
        {
            get => Image.CompressCif;
            set
            {
                if (Image.CompressCif != value)
                {
                    Image.CompressCif = value;
                    OnPropertyChanged(nameof(CompressCif)); // notify WPF
                }
            }
        }

        /// <summary>
        ///     Gets or sets the observer.
        /// </summary>
        /// <value>
        ///     The observer.
        /// </value>
        public Dictionary<int, string?> Observer
        {
            get => FileContext.Observer;
            set
            {
                if (FileContext.Observer != value)
                {
                    FileContext.Observer = value;
                    OnPropertyChanged(nameof(Observer)); // notify WPF
                    NavigationLogic();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the BitmapImage.
        /// </summary>
        /// <value>
        ///     The BitmapImage.
        /// </value>
        public BitmapImage? Bmp
        {
            get => Image.BitmapImage;
            set
            {
                if (Image.BitmapImage == value) return;

                Image.BitmapImage = value;
                OnPropertyChanged(nameof(Bmp)); // notify WPF
            }
        }

        /// <summary>
        ///     Gets or sets the status image.
        ///     Red or Green Icon
        /// </summary>
        /// <value>
        ///     The status image.
        /// </value>
        public string StatusImage
        {
            get => UiState.StatusImage;
            set
            {
                if (UiState.StatusImage != value)
                {
                    UiState.StatusImage = value;
                    OnPropertyChanged(nameof(StatusImage)); // notify WPF
                }
            }
        }

        /// <summary>
        ///     Gets or sets the GIF path.
        /// </summary>
        /// <value>
        ///     The GIF path.
        /// </value>
        public string? GifPath
        {
            get => FileContext.GifPath;
            set
            {
                if (FileContext.GifPath != value)
                {
                    FileContext.GifPath = value;
                    OnPropertyChanged(nameof(GifPath)); // notify WPF
                }
            }
        }

        /// <summary>
        ///     Gets or sets the selection tool.
        /// </summary>
        /// <value>
        ///     The selection tool.
        /// </value>
        public ImageZoomTools ImageZoomTool
        {
            get => _imageZoomTool;
            set => SetProperty(ref _imageZoomTool, value, nameof(ImageZoomTool));
        }

        /// <summary>
        /// The image
        /// </summary>
        internal readonly ImageContext Image = new();

        /// <summary>
        /// The UI state
        /// </summary>
        internal readonly UiState UiState = new();

        /// <summary>
        /// The file context
        /// </summary>
        internal readonly FileContext FileContext = new();

        /// <summary>
        ///     Gets the selections.
        /// </summary>
        /// <value>
        ///     The selections.
        /// </value>
        public IEnumerable<ImageTools> Tooling =>
            Enum.GetValues(typeof(ImageTools))
                .Cast<ImageTools>();

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            Image.CustomImageFormat = new CustomImageFormat();
            Observer = new Dictionary<int, string?>();

            LeftButtonVisibility = RightButtonVisibility = Visibility.Hidden;
            ThumbnailVisibility = Visibility.Visible;
            IsImageActive = false;

            // Initialize key bindings using DelegateCommand<T>
            CommandBindings = new Dictionary<Key, ICommand>
            {
                { Key.O, Commands.Open },
                { Key.S, Commands.Save },
                { Key.Delete, Commands.Delete },
                { Key.F5, Commands.Refresh },
                { Key.Left, Commands.Previous },
                { Key.Right, Commands.Next }
            };

            //PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        //private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    //here we handle the Selection of Image Zoom tools, as well as filters as textures
        //    switch (e.PropertyName)
        //    {
        //        case nameof(SelectedTool):
        //            switch (SelectedTool)
        //            {
        //                case ImageTools.Move:
        //                    ImageZoomTool = ImageZoomTools.Move;

        //                    break;
        //                case ImageTools.Paint:
        //                case ImageTools.Erase:
        //                case ImageTools.ColorSelect:
        //                    ImageZoomTool = ImageZoomTools.Trace;
        //                    break;
        //                case ImageTools.Area:
        //                    // no need to handle anything here
        //                    break;
        //            }

        //            break;
        //        case nameof(SelectedTexture):
        //            CurrentTexture = Translator.GetTextureFromString(SelectedTexture);
        //            break;
        //        case nameof(SelectedFilter):
        //            CurrentFilter = Translator.GetFilterFromString(SelectedFilter);
        //            break;
        //    }
        //}

        /// <summary>
        ///     Tools the changed action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void ToolChangedAction(ImageZoomTools obj)
        {
            if (UiState.ImageZoomControl != null)
                ImageZoomTool = obj;
        }

        internal void ToolChangedActionNew(object tools)
        {
            Trace.WriteLine($"Tool changed to: {tools}");
        }

        /// <summary>
        ///     Thumbs the image clicked action.
        /// </summary>
        /// <param name="obj">The identifier.</param>
        internal void ThumbImageClickedAction(ImageEventArgs obj)
        {
            ChangeImage(obj.Id);
        }

        /// <summary>
        ///     Set the selected point.
        /// </summary>
        /// <param name="wPoint">The w point.</param>
        internal void SelectedPointAction(Point wPoint)
        {
            var point = new System.Drawing.Point((int)wPoint.X, (int)wPoint.Y);

            switch (ToolCode)
            {
                case EnumTools.Paint:

                    var color = Color.GetDrawingColor();

                    Image.Bitmap = ImageProcessor.SetPixel(Image.Bitmap, point, color, BrushSize);

                    Bmp = Image.BitmapSource;
                    return;

                case EnumTools.ColorSelect:
                    Color = ImageProcessor.GetPixel(Image.Bitmap, point, Tolerance);
                    UiState.Picker.SetColors(Color.R, Color.G, Color.B, Color.A);
                    Color = UiState.Picker.Colors;
                    return;
                case EnumTools.Move:
                case EnumTools.Erase:
                case EnumTools.SolidColor:
                case EnumTools.Filter:
                    break;
            }
        }

        /// <summary>
        ///     Selected frame.
        /// </summary>
        /// <param name="frame">The selected area.</param>
        internal void SelectedFrameAction(SelectionFrame frame)
        {
            if (ImageZoomTool == ImageZoomTools.Move)
                return;
            if (ImageZoomTool == ImageZoomTools.Trace)
                return;

            switch (ToolCode)
            {
                case EnumTools.Erase:
                    Image.Bitmap = ImageProcessor.EraseImage(frame, Image.Bitmap);
                    break;
                case EnumTools.Move:
                    break;
                case EnumTools.SolidColor:

                    var color = Color.GetDrawingColor();

                    Image.Bitmap = ImageProcessor.FillArea(Image.Bitmap, frame, color);
                    break;
                case EnumTools.Texture:
                    Image.Bitmap = ImageProcessor.FillTexture(Image.Bitmap, frame, CurrentTexture);
                    break;
                case EnumTools.Filter:
                    Image.Bitmap = ImageProcessor.FillFilter(Image.Bitmap, frame, CurrentFilter);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Bmp = Image.BitmapSource;
        }

        /// <summary>
        ///     Colors the changed action.
        /// </summary>
        /// <param name="colorHsv">The color HSV.</param>
        internal void ColorChangedAction(ColorHsv colorHsv)
        {
            Color = colorHsv;
        }

        /// <summary>
        ///     Closes the app
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void CloseAction(object obj)
        {
            var config = SlimViewerRegister.GetRegister();
            config.MainAutoPlayGif = UiState.ImageZoomControl.AutoplayGifImage;

            Config.SetConfig(config);
            if (AutoClean)
            {
                var file = new FileProcessingCommands();
                file.CleanTempFolder(true);
            }

            Application.Current.Shutdown();
        }

        /// <summary>
        ///     Opens a picture
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void OpenAction(object obj)
        {
            var pathObj = DialogHandler.HandleFileOpen(ViewResources.FileOpen, FileContext.CurrentPath);

            if (string.IsNullOrEmpty(pathObj?.FilePath)) return;

            FileContext.CurrentPath = pathObj.Folder;

            //handle cbz files
            if (string.Equals(pathObj.Extension, ViewResources.CbzExt, StringComparison.OrdinalIgnoreCase))
            {
                GenerateCbrView(pathObj);
                return;
            }

            //check if file extension is supported
            if (!ImagingResources.Appendix.Contains(pathObj.Extension?.ToLower()))
            {
                _ = MessageBox.Show(string.Concat(ViewResources.ErrorFileNotSupported, pathObj.Extension),
                    ViewResources.ErrorMessage);
                return;
            }

            GenerateView(pathObj.FilePath);
            LoadThumbs(pathObj.Folder, pathObj.FilePath);

            //activate Menus
            if (Image.BitmapImage != null) IsImageActive = true;
        }

        /// <summary>
        ///     Opens a CBR Format.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void OpenCbzAction(object obj)
        {
            var pathObj =
                DialogHandler.HandleFileOpen(ViewResources.FileOpenCbz, FileContext.CurrentPath);

            if (pathObj == null || !File.Exists(pathObj.FilePath)) return;

            GenerateCbrView(pathObj);

            //activate Menus
            if (Image.BitmapImage != null) IsImageActive = true;
        }

        /// <summary>
        ///     Open the cif Format.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void OpenCifAction(object obj)
        {
            var pathObj =
                DialogHandler.HandleFileOpen(ViewResources.FileOpenCif, FileContext.CurrentPath);

            if (pathObj == null || !File.Exists(pathObj.FilePath)) return;

            Image.Bitmap = Image.CustomImageFormat.GetImageFromCif(pathObj.FilePath);

            if (Image.Bitmap == null) return;

            //activate Menus
            if (Image.BitmapImage != null) IsImageActive = true;

            Bmp = Image.BitmapSource;

            //set Filename
            FileName = Path.GetFileName(FileContext.FilePath);
            //set Infos
            Information = ViewResources.BuildImageInformation(FileContext.FilePath, FileName, Bmp);
        }

        /// <summary>
        ///     Next Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void NextAction(object obj)
        {
            var lst = Observer.Keys.ToList();
            if (lst.IsNullOrEmpty()) return;

            ChangeImage(Utility.GetNextElement(FileContext.CurrentId, lst));
            UiState.Thumb.Next();
            NavigationLogic();
        }

        /// <summary>
        ///     Previous Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void PreviousAction(object obj)
        {
            var lst = Observer.Keys.ToList();
            if (lst.IsNullOrEmpty()) return;

            ChangeImage(Utility.GetPreviousElement(FileContext.CurrentId, lst));
            UiState.Thumb.Previous();
            NavigationLogic();
        }

        /// <summary>
        ///     Refresh the Control
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void RefreshAction(object obj)
        {
            FileContext.CurrentId = -1;
            FileContext.FilePath = string.Empty;
            Bmp = null;
            GifPath = null;

            if (!Directory.Exists(FileContext.CurrentPath))
            {
                Observer = null;
                return;
            }

            LoadThumbs(FileContext.CurrentPath);
        }

        /// <summary>
        ///     Exports the string action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void ExportStringAction(object obj)
        {
            ImageProcessor.ExportString(Image.Bitmap);
        }

        /// <summary>
        ///     Open Folder
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void FolderAction(object obj)
        {
            //get target Folder
            var path = DialogHandler.ShowFolder(FileContext.CurrentPath);

            if (!Directory.Exists(path)) return;

            LoadThumbs(path);

            //activate Menus
            if (!string.IsNullOrEmpty(path)) IsImageActive = true;
        }

        /// <summary>
        ///     Clears the Image the current View.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void ClearAction(object obj)
        {
            if (!Observer.ContainsKey(FileContext.CurrentId)) return;

            Bmp = null;
            GifPath = null;

            UiState.Thumb.RemoveSingleItem(FileContext.CurrentId);

            //decrease File Count
            if (Count > 0) Count--;

            Image.Bitmap = null;
            Bmp = null;
            GifPath = null;
            FileContext.GifPath = null;

            NextAction(this);
        }

        /// <summary>
        ///     Open the Explorer
        ///     https://ss64.com/nt/explorer.html
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void ExplorerAction(object obj)
        {
            if (!Directory.Exists(FileContext.CurrentPath)) return;

            var argument = !File.Exists(FileContext.FilePath)
                ? FileContext.CurrentPath
                : string.Concat(ViewResources.Select, FileContext.FilePath, ViewResources.Close);
            _ = Process.Start(ViewResources.Explorer, argument);
        }

        /// <summary>
        ///     Changes the image.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void ChangeImage(int id)
        {
            if (!Observer.ContainsKey(id)) return;

            FileContext.CurrentId = id;
            NavigationLogic();

            var filePath = Observer[id];
            GenerateView(filePath);
        }

        /// <summary>
        ///     Changes the image.
        /// </summary>
        /// <param name="files">The files we want to view.</param>
        public void ChangeImage(IEnumerable<string> files)
        {
            //no need to check for null it was already checked
            FileContext.Files = files.ToList();

            Count = FileContext.Files.Count;

            try
            {
                FileContext.CurrentPath = Path.GetDirectoryName(files.ToList()[0]);
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), ViewResources.ErrorMessage);
                return;
            }
            catch (PathTooLongException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    string.Concat(ViewResources.ErrorMessage, nameof(ChangeImage)));
                return;
            }

            Bmp = null;
            GifPath = null;

            FileContext.CurrentId = -1;

            _ = GenerateThumbView(FileContext.Files);
            Image.Information = string.Concat(ViewResources.DisplayImages, FileContext.Files.Count);
        }

        /// <summary>
        ///     Changes the image.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void ChangeImage(string? filePath)
        {
            //check if it exists
            if (!CanLoadFile(filePath)) return;

            //load into the Image Viewer
            GenerateView(filePath);

            // load all other Pictures in the Folder
            var folder = Path.GetDirectoryName(filePath);
            if (folder == FileContext.CurrentPath) return;

            LoadThumbs(folder, filePath);

            //set the Id of the loaded Image
            FileContext.CurrentId = FileContext.CurrentIdGetIdByFilePath(filePath);
        }

        /// <summary>
        ///     Changes the image.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="info">The information about the selected Images.</param>
        internal void ChangeImage(IEnumerable<string?> files, string? filePath, string info)
        {
            //check if it exists
            if (!CanLoadFile(filePath)) return;

            var lst = files.ToList();

            _ = GenerateThumbView(lst);

            //load into the Image Viewer
            GenerateImageAsync(filePath);

            //set the Id of the loaded Image
            FileContext.CurrentId = FileContext.CurrentIdGetIdByFilePath(filePath);

            //set new Information
            Information = info;
        }

        /// <summary>
        ///     Generates the CBR view.
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
        ///     Generates the view.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        internal void GenerateView(string? filePath)
        {
            var info = FileHandleSearch.GetFileDetails(filePath);

            if (info == null)
            {
                Bmp = null;
                GifPath = null;
                LoadThumbs(FileContext.CurrentPath);
                return;
            }

            //load into the Image Viewer
            GenerateImageAsync(filePath);
        }

        /// <summary>
        ///     Generates the image.
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

                    GifPath = null;
                    GifPath = filePath;

                    var info = ImageGifHandler.GetImageInfo(filePath);

                    //set Infos
                    Information = ViewResources.BuildGifInformation(filePath, info);
                }
                else
                {
                    Image.Bitmap = await Task.Run(() => ImageProcessor.Render.GetOriginalBitmap(filePath));
                    Bmp = Image.BitmapSource; // now safe on UI thread

                    //reset gif Image
                    GifPath = null;

                    Bmp = Image.BitmapSource;
                    //set Infos
                    Information = ViewResources.BuildImageInformation(filePath, FileName, Bmp);
                }

                FileContext.FilePath = filePath;
                //set Filename
                FileName = Path.GetFileName(filePath);
            }
            catch (Exception ex) when (ex is IOException or ArgumentException or NotSupportedException
                                           or InvalidOperationException)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    string.Concat(ViewResources.ErrorMessage, nameof(GenerateImageAsync)));
            }
        }

        /// <summary>
        ///     Loads the thumbs.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="filePath">The file path, optional.</param>
        internal void LoadThumbs(string folder, string? filePath = null)
        {
            GenerateThumbView(folder);

            // If filePath is provided, get the Id of the displayed image.
            if (!string.IsNullOrEmpty(filePath))
            {
                FileContext.CurrentId = FileContext.CurrentIdGetIdByFilePath(filePath);
            }
            else
            {
                // Reset the Id of the displayed image if no file path is provided.
                FileContext.CurrentId = -1;
                Bmp = null;
                GifPath = null;
            }
        }

        /// <summary>
        ///     Generates the thumb view.
        /// </summary>
        /// <param name="folder">The folder.</param>
        private void GenerateThumbView(string folder)
        {
            //initiate Basic values
            FileContext.CurrentPath = folder;
            StatusImage = string.Empty;
            StatusImage = UiState.RedIconPath;

            FileContext.Files =
                FileHandleSearch.GetFilesByExtensionFullPath(folder, ImagingResources.Appendix, UiState.UseSubFolders);

            //decrease File Count
            if (FileContext.IsFilesEmpty)
            {
                Count = 0;
                Observer = null;
                GifPath = null;
                Bmp = null;

                return;
            }

            NavigationLogic();

            // ReSharper disable once PossibleNullReferenceException, already checked
            Count = FileContext.Files.Count;

            FileContext.Files = FileContext.FilesSorted;

            _ = GenerateThumbView(FileContext.Files).ConfigureAwait(false);
        }

        /// <summary>
        ///     Generates the thumb view.
        /// </summary>
        /// <param name="lst">The File List.</param>
        private async Task GenerateThumbView(IReadOnlyCollection<string?> lst)
        {
            //if we don't want to generate Thumbs don't
            if (!IsThumbsVisible) return;

            StatusImage = UiState.RedIconPath;

            //load Thumbnails
            _ = await Task.Run(() => Observer = lst.ToDictionary()).ConfigureAwait(false);
        }

        /// <summary>
        ///     Navigation logic.
        /// </summary>
        private void NavigationLogic()
        {
            if (FileContext.Count <= 1)
            {
                UiState.HideButtons();
            }
            else
            {
                // Set visibility based on _file.CurrentId and _file.Count
                RightButtonVisibility = FileContext.CurrentId == FileContext.Count - 1
                    ? Visibility.Hidden
                    : Visibility.Visible;
                LeftButtonVisibility = FileContext.CurrentId <= 0 ? Visibility.Hidden : Visibility.Visible;
            }

            // show or hide the Thumbnail Bar
            ThumbnailVisibility = UiState.ThumbnailState();

            //show or hide image edit
            IsImageActive = Image.Bitmap != null;
        }

        /// <summary>
        ///     Saves the image.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="extension">The File Extension.</param>
        /// <param name="btm">The Bitmap.</param>
        /// <returns>
        ///     Success Status
        /// </returns>
        internal bool SaveImage(string path, string extension, Bitmap btm)
        {
            StatusImage = UiState.RedIconPath;

            var check = ImageProcessor.SaveImage(path, extension, btm);
            StatusImage = UiState.GreenIconPath;

            return check;
        }

        /// <summary>
        /// Thumbnails are loaded.
        /// </summary>
        /// <param name="obj">The object.</param>
        public void ImageLoadedCommandAction(object obj)
        {
            //if (Status == null) return;
            if (string.IsNullOrEmpty(StatusImage)) return;

            StatusImage = UiState.GreenIconPath;
        }

        /// <summary>
        /// Determines whether this instance [can load file] the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can load file] the specified path; otherwise, <c>false</c>.
        /// </returns>
        private bool CanLoadFile(string? path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path) &&
                   ImagingResources.Appendix.Any(path.EndsWith);
        }
    }
}