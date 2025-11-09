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
using System.Reflection;
using System.Runtime.InteropServices;
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

        private double _eraseRadius;
        private ImageZoomTools _imageZoomTool;

        public ImageViewCommands Commands { get; }

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

        internal readonly ImageContext Image = new();

        internal readonly UiState UiState = new();

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

            PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //here we handle the Selection of Image Zoom tools, as well as filters as textures
            switch (e.PropertyName)
            {
                case nameof(SelectedTool):
                    switch (SelectedTool)
                    {
                        case ImageTools.Move:
                            ImageZoomTool = ImageZoomTools.Move;

                            break;
                        case ImageTools.Paint:
                        case ImageTools.Erase:
                        case ImageTools.ColorSelect:
                            ImageZoomTool = ImageZoomTools.Trace;
                            break;
                        case ImageTools.Area:
                            // no need to handle anything here
                            break;
                    }

                    break;
                case nameof(SelectedTexture):
                    CurrentTexture = Translator.GetTextureFromString(SelectedTexture);
                    break;
                case nameof(SelectedFilter):
                    CurrentFilter = Translator.GetFilterFromString(SelectedFilter);
                    break;
            }
        }

        /// <summary>
        ///     Tools the changed action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void ToolChangedAction(ImageZoomTools obj)
        {
            if (UiState.ImageZoomControl != null)
                ImageZoomTool = obj;
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
            if (AutoClean) CleanTempAction(true);

            Application.Current.Shutdown();
        }

        /// <summary>
        ///     Opens a picture
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void OpenAction(object obj)
        {
            var pathObj = DialogHandler.HandleFileOpen(ViewResources.FileOpen, SlimViewerRegister.CurrentFolder);

            if (string.IsNullOrEmpty(pathObj?.FilePath)) return;

            SlimViewerRegister.CurrentFolder = pathObj.Folder;

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
                DialogHandler.HandleFileOpen(ViewResources.FileOpenCbz, SlimViewerRegister.CurrentFolder);

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
                DialogHandler.HandleFileOpen(ViewResources.FileOpenCif, SlimViewerRegister.CurrentFolder);

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
        ///     Convert the cif Format.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void ConvertCifAction(object obj)
        {
            var pathObj = DialogHandler.HandleFileOpen(ViewResources.FileOpen, SlimViewerRegister.CurrentFolder);

            if (pathObj == null || !File.Exists(pathObj.FilePath)) return;

            if (CompressCif) Image.CustomImageFormat.GenerateCifCompressedFromBitmap(Image.Bitmap, pathObj.FilePath);
            else Image.CustomImageFormat.GenerateBitmapToCifFile(Image.Bitmap, pathObj.FilePath);
        }

        /// <summary>
        ///     Saves the picture.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void SaveAction(object obj)
        {
            if (Bmp == null) return;

            var btm = Bmp.ToBitmap();

            var pathObj = DialogHandler.HandleFileSave(ViewResources.FileOpen, SlimViewerRegister.CurrentFolder);

            if (pathObj == null) return;

            if (string.Equals(pathObj.FilePath, FileContext.FilePath, StringComparison.OrdinalIgnoreCase))
                _ = FileHandleDelete.DeleteFile(FileContext.FilePath);

            try
            {
                var check = SaveImage(pathObj.FilePath, pathObj.Extension, btm);
                if (!check) _ = MessageBox.Show(ViewResources.ErrorCouldNotSaveFile);
            }
            catch (Exception ex) when (ex is ArgumentException or IOException or ExternalException)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), string.Concat(ViewResources.ErrorMessage, nameof(SaveAction)));
            }
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
        ///     Applies the filter.
        /// </summary>
        /// <param name="filterName">The filter name.</param>
        internal void ApplyFilterAction(string filterName)
        {
            var filter = Translator.GetFilterFromString(filterName);

            var btm = ImageProcessor.Filter(Image.Bitmap, filter);
            Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        ///     Applies the texture.
        /// </summary>
        /// <param name="textureName">The name of the texture.</param>
        internal void ApplyTextureAction(string textureName)
        {
            var texture = Translator.GetTextureFromString(textureName);

            var btm = ImageProcessor.Texture(Image.Bitmap, texture);
            Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        ///     Brightens the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void BrightenAction(object obj)
        {
            var btm = ImageProcessor.DBrighten(Image.Bitmap);
            Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        ///     Darkens the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void DarkenAction(object obj)
        {
            var btm = ImageProcessor.Darken(Image.Bitmap);
            Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        ///     Pixelate action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void PixelateAction(object obj)
        {
            var btm = ImageProcessor.Pixelate(Image.Bitmap, PixelWidth);
            Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        ///     Deletes the Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void DeleteAction(object obj)
        {
            if (!Observer.ContainsKey(FileContext.CurrentId) && UiState.Thumb.Selection.IsNullOrEmpty()) return;

            if (!UiState.Thumb.Selection.IsNullOrEmpty())
            {
                var count = 0;
                foreach (var id in UiState.Thumb.Selection)
                    try
                    {
                        var check = FileHandleSafeDelete.DeleteFile(Observer[id]);

                        //decrease File Count
                        if (Count > 0 && check) Count--;
                        if (check) count++;
                    }
                    catch (FileHandlerException ex)
                    {
                        Trace.WriteLine(ex);
                        _ = MessageBox.Show(ex.ToString(),
                            string.Concat(ViewResources.ErrorMessage, nameof(DeleteAction)));
                    }

                LoadThumbs(SlimViewerRegister.CurrentFolder);

                _ = MessageBox.Show(string.Concat(ViewResources.MessageCount, count),
                    ViewResources.MessageSuccess, MessageBoxButton.OK);
            }
            else
            {
                Bmp = null;
                Image.Bitmap = null;
                GifPath = null;
                FileContext.GifPath = null;

                try
                {
                    var check = FileHandleSafeDelete.DeleteFile(Observer[FileContext.CurrentId]);

                    //decrease File Count
                    if (Count > 0 && check) Count--;
                }
                catch (FileHandlerException ex)
                {
                    Trace.WriteLine(ex);
                    _ = MessageBox.Show(ex.ToString(),
                        string.Concat(ViewResources.ErrorMessage, nameof(DeleteAction)));
                }

                UiState.Thumb.RemoveSingleItem(FileContext.CurrentId);

                NextAction(this);
            }
        }

        /// <summary>
        ///     Renames the Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal async Task RenameAction(object obj)
        {
            if (!IsImageActive) return;

            if (!Observer.TryGetValue(FileContext.CurrentId, out string? file)) return;

            if (!File.Exists(file)) return;

            var folder = Path.GetDirectoryName(file);
            if (string.IsNullOrEmpty(folder)) return;

            var filePath = Path.Combine(folder, FileName);

            // Check if we have a duplicate; if true, shall we overwrite?
            if (File.Exists(filePath))
            {
                var dialogResult = await Task.Run(() =>
                    _ = MessageBox.Show(ViewResources.MessageFileAlreadyExists,
                        ViewResources.CaptionFileAlreadyExists,
                        MessageBoxButton.YesNo));

                if (dialogResult == MessageBoxResult.No) return;
            }

            try
            {
                var check = await FileHandleRename.RenameFile(file, filePath);
                if (!check) return;
            }
            catch (FileHandlerException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    string.Concat(ViewResources.ErrorMessage, nameof(RenameAction)));
            }

            Observer[FileContext.CurrentId] = filePath;
            GenerateView(filePath);
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

            if (!Directory.Exists(SlimViewerRegister.CurrentFolder))
            {
                Observer = null;
                return;
            }

            LoadThumbs(SlimViewerRegister.CurrentFolder);
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
        ///     Rotates the backward action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void RotateBackwardAction(object obj)
        {
            Image.Bitmap = ImageProcessor.RotateImage(Image.Bitmap, -90);
            Bmp = Image.BitmapSource;
        }

        /// <summary>
        ///     Rotates the forward action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void RotateForwardAction(object obj)
        {
            Image.Bitmap = ImageProcessor.RotateImage(Image.Bitmap, 90);
            Bmp = Image.BitmapSource;
        }

        /// <summary>
        ///     Mirrors the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void MirrorAction(object obj)
        {
            if (Image.Bitmap == null) return;

            Image.Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            Bmp = Image.BitmapSource;
        }

        /// <summary>
        ///     Open Folder
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void FolderAction(object obj)
        {
            //get target Folder
            var path = DialogHandler.ShowFolder(SlimViewerRegister.CurrentFolder);

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
            if (!Directory.Exists(SlimViewerRegister.CurrentFolder)) return;

            var argument = !File.Exists(FileContext.FilePath)
                ? SlimViewerRegister.CurrentFolder
                : string.Concat(ViewResources.Select, FileContext.FilePath, ViewResources.Close);
            _ = Process.Start(ViewResources.Explorer, argument);
        }

        /// <summary>
        ///     Cleans the temporary Folder action.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void CleanTempAction(object obj)
        {
            var check = false;

            if (obj != null) check = (bool)obj;

            var root = Path.Combine(Directory.GetCurrentDirectory(), ViewResources.TempFolder);

            try
            {
                _ = FileHandleDelete.DeleteAllContents(root);
            }
            catch (FileHandlerException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(),
                    string.Concat(ViewResources.ErrorMessage, nameof(CleanTempAction)));
                return;
            }

            if (!check)
                _ = MessageBox.Show(ViewResources.StatusDone, ViewResources.CaptionDone,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
        }

        /// <summary>
        ///     Moves selected Image
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void MoveAction(object obj)
        {
            if (!File.Exists(FileName) && UiState.Thumb.Selection.IsNullOrEmpty()) return;
            //Initiate Folder
            if (string.IsNullOrEmpty(SlimViewerRegister.CurrentFolder))
                SlimViewerRegister.CurrentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //get target Folder
            var path = DialogHandler.ShowFolder(SlimViewerRegister.CurrentFolder ?? Directory.GetCurrentDirectory());

            if (!UiState.Thumb.Selection.IsNullOrEmpty())
            {
                var count = 0;

                foreach (var id in UiState.Thumb.Selection)
                {
                    if (!Directory.Exists(path)) return;

                    var fileName = Observer[id];
                    if (!File.Exists(fileName)) continue;

                    //Copy Single File
                    var info = new FileInfo(fileName);
                    var target = Path.Combine(path, info.Name);

                    if (File.Exists(target))
                    {
                        var dialogResult = MessageBox.Show(ViewResources.MessageFileAlreadyExists,
                            ViewResources.CaptionFileAlreadyExists,
                            MessageBoxButton.YesNo);
                        if (dialogResult == MessageBoxResult.No) continue;
                    }

                    info.MoveTo(target, true);
                    count++;
                }

                _ = MessageBox.Show(string.Concat(ViewResources.MessageMoved, count),
                    ViewResources.MessageSuccess, MessageBoxButton.OK);
            }
            else
            {
                if (!Directory.Exists(path)) return;

                //Copy Single File
                var info = new FileInfo(FileName);
                var target = Path.Combine(path, info.Name);

                if (File.Exists(target))
                {
                    var dialogResult = MessageBox.Show(ViewResources.MessageFileAlreadyExists,
                        ViewResources.CaptionFileAlreadyExists,
                        MessageBoxButton.YesNo);
                    if (dialogResult == MessageBoxResult.No) return;
                }

                info.MoveTo(target, true);
            }
        }

        /// <summary>
        ///     Moves all Images.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal void MoveAllAction(object obj)
        {
            //Initiate Folder
            if (string.IsNullOrEmpty(SlimViewerRegister.CurrentFolder))
                SlimViewerRegister.CurrentFolder = Path.GetDirectoryName(UiState.Root);

            //get target Folder
            var path = DialogHandler.ShowFolder(SlimViewerRegister.CurrentFolder ?? Directory.GetCurrentDirectory());

            if (!Directory.Exists(path)) return;

            if (FileContext.IsFilesEmpty) return;

            var lst = FileHandleSearch.GetFilesByExtensionFullPath(path, ImagingResources.Appendix,
                UiState.UseSubFolders);

            if (lst == null) return;

            var i = FileContext.Files.Intersect(lst);

            if (i.Any())
            {
                var dialogResult = MessageBox.Show(ViewResources.MessageFileAlreadyExists,
                    ViewResources.CaptionFileAlreadyExists,
                    MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.No) return;
            }

            //Move all Contents from this folder into another
            _ = FileHandleCut.CutFiles(FileContext.Files, path, false);
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
                SlimViewerRegister.CurrentFolder = Path.GetDirectoryName(files.ToList()[0]);
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
            if (!File.Exists(filePath)) return;

            //check if we even handle this file type
            if (!ImagingResources.Appendix.Any(filePath.EndsWith)) return;

            //load into the Image Viewer
            GenerateView(filePath);

            // load all other Pictures in the Folder
            var folder = Path.GetDirectoryName(filePath);
            if (folder == SlimViewerRegister.CurrentFolder) return;

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
            if (!File.Exists(filePath)) return;

            //check if we even handle this file type
            if (!ImagingResources.Appendix.Any(filePath.EndsWith)) return;

            var lst = files.ToList();

            _ = GenerateThumbView(lst);

            //load into the Image Viewer
            GenerateImage(filePath);

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
            if (!string.IsNullOrEmpty(folder)) SlimViewerRegister.CurrentFolder = folder;
            var file = ImageProcessor.UnpackFile(folder);

            if (file == null) return;

            GenerateView(file);
            LoadThumbs(folder, file);
        }

        /// <summary>
        ///     Generates the view.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private void GenerateView(string? filePath)
        {
            var info = FileHandleSearch.GetFileDetails(filePath);

            if (info == null)
            {
                Bmp = null;
                GifPath = null;
                LoadThumbs(SlimViewerRegister.CurrentFolder);
                return;
            }

            //load into the Image Viewer
            GenerateImage(filePath);
        }

        /// <summary>
        ///     Generates the image.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private void GenerateImage(string? filePath)
        {
            try
            {
                var ext = Path.GetExtension(filePath);

                if (ext.Equals(ImagingResources.GifExt, StringComparison.OrdinalIgnoreCase))
                {
                    if (GifPath?.Equals(filePath, StringComparison.OrdinalIgnoreCase) == true) return;

                    GifPath = filePath;

                    var info = ImageGifHandler.GetImageInfo(filePath);

                    //set Infos
                    Information = ViewResources.BuildGifInformation(filePath, info);
                }
                else
                {
                    Image.Bitmap = ImageProcessor.Render.GetOriginalBitmap(filePath);

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
                    string.Concat(ViewResources.ErrorMessage, nameof(GenerateImage)));
            }
        }

        /// <summary>
        ///     Loads the thumbs.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="filePath">The file path, optional.</param>
        private void LoadThumbs(string folder, string? filePath = null)
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
            SlimViewerRegister.CurrentFolder = folder;
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

            _ = GenerateThumbView(FileContext.Files);
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
        ///     Thumbnails are loaded.
        /// </summary>
        public void ImageLoadedCommandAction(object obj)
        {
            //if (Status == null) return;
            if (string.IsNullOrEmpty(StatusImage)) return;

            StatusImage = UiState.GreenIconPath;
        }
    }
}