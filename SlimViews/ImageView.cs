/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/ImageView.cs
 * PURPOSE:     View Model for the SlimViewer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global, if we make it private the Property Changed event will not be triggered in the Window
// ReSharper disable MemberCanBeInternal, must be public, else the View Model won't work

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
using CommonControls;
using CommonDialogs;
using ExtendedSystemObjects;
using FileHandler;
using Imaging;
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
        ///     The analyzer window command
        /// </summary>
        private ICommand _analyzerWindowCommand;

        /// <summary>
        ///     The apply filter command
        /// </summary>
        private ICommand _applyFilterCommand;

        /// <summary>
        ///     The apply texture command
        /// </summary>
        private ICommand _applyTextureCommand;

        /// <summary>
        ///     The automatic clean
        /// </summary>
        private bool _autoClean;

        /// <summary>
        ///     The BitmapImage
        /// </summary>
        private BitmapImage _bmp;

        /// <summary>
        ///     The brighten command
        /// </summary>
        private ICommand _brightenCommand;

        /// <summary>
        ///     The brush size
        /// </summary>
        private double _brushSize;

        /// <summary>
        ///     The Bitmap
        /// </summary>
        private Bitmap _btm;

        /// <summary>
        ///     The render
        /// </summary>
        private CustomImageFormat _cif;

        /// <summary>
        ///     Clean the temporary folder
        /// </summary>
        private ICommand _cleanTempFolder;

        /// <summary>
        ///     The clear command.
        /// </summary>
        private ICommand _clearCommand;

        /// <summary>
        ///     The close command.
        /// </summary>
        private ICommand _closeCommand;

        /// <summary>
        ///     The color changed command
        /// </summary>
        private ICommand _colorChangedCommand;

        /// <summary>
        ///     Check if we compress cif files.
        /// </summary>
        private bool _compress;

        /// <summary>
        ///     The convert cif command.
        /// </summary>
        private ICommand _convertCommandCif;

        /// <summary>
        ///     The File count
        /// </summary>
        private int _count;

        /// <summary>
        ///     The current identifier of the Image
        /// </summary>
        private int _currentId;

        /// <summary>
        ///     The darken command
        /// </summary>
        private ICommand _darkenCommand;

        /// <summary>
        ///     The delete command
        /// </summary>
        private ICommand _deleteCommand;

        /// <summary>
        ///     The duplicate command
        /// </summary>
        private ICommand _duplicateCommand;

        /// <summary>
        ///     The explorer command
        /// </summary>
        private ICommand _explorerCommand;

        /// <summary>
        ///     The export string command
        /// </summary>
        private ICommand _exportStringCommand;

        /// <summary>
        ///     The file list
        ///     Holds the current List of Files we are viewing.
        ///     Needed for Move Files
        /// </summary>
        private List<string> _fileList;

        /// <summary>
        ///     The file name
        /// </summary>
        private string _fileName;

        /// <summary>
        ///     The current path
        /// </summary>
        private string _filePath;

        /// <summary>
        ///     The filter configuration command
        /// </summary>
        private ICommand _filterConfigCommand;

        /// <summary>
        ///     The folder command
        /// </summary>
        private ICommand _folderCommand;

        /// <summary>
        ///     The folder convert command
        /// </summary>
        private ICommand _folderConvertCommand;

        /// <summary>
        ///     The folder rename command
        /// </summary>
        private ICommand _folderRenameCommand;

        /// <summary>
        ///     The GIF path
        /// </summary>
        private string _gifPath;

        /// <summary>
        ///     The GIF window command
        /// </summary>
        private ICommand _gifWindowCommand;

        /// <summary>
        ///     The green icon
        /// </summary>
        private string _greenIcon;

        /// <summary>
        ///     The image loaded command
        /// </summary>
        private ICommand _imageLoadedCommand;

        /// <summary>
        ///     The information
        /// </summary>
        private string _information;

        /// <summary>
        ///     Is the Menu active
        /// </summary>
        private bool _isActive;

        /// <summary>
        ///     The is image active
        /// </summary>
        private bool _isImageActive;

        /// <summary>
        ///     The left button visibility
        /// </summary>
        private Visibility _leftButtonVisibility;

        /// <summary>
        ///     The mirror command
        /// </summary>
        private ICommand _mirrorCommand;

        /// <summary>
        ///     The move all command
        /// </summary>
        private ICommand _moveAllCommand;

        /// <summary>
        ///     The move command
        /// </summary>
        private ICommand _moveCommand;

        /// <summary>
        ///     The next command
        /// </summary>
        private ICommand _nextCommand;

        /// <summary>
        ///     The observer
        /// </summary>
        private Dictionary<int, string> _observer;

        /// <summary>
        ///     The open CBZ command.
        /// </summary>
        private ICommand _openCbzCommand;

        /// <summary>
        ///     The open command
        /// </summary>
        private ICommand _openCommand;

        /// <summary>
        ///     The open cif command.
        /// </summary>
        private ICommand _openCommandCif;

        /// <summary>
        ///     The pixelate command
        /// </summary>
        private ICommand _pixelateCommand;

        /// <summary>
        ///     The pixel width
        /// </summary>
        private int _pixelWidth;

        /// <summary>
        ///     The previous command
        /// </summary>
        private ICommand _previousCommand;

        /// <summary>
        ///     The red icon
        /// </summary>
        private string _redIcon;

        /// <summary>
        ///     The refresh command
        /// </summary>
        private ICommand _refreshCommand;

        /// <summary>
        ///     The rename command
        /// </summary>
        private ICommand _renameCommand;

        /// <summary>
        ///     The resizer window command
        /// </summary>
        private ICommand _resizerWindowCommand;

        /// <summary>
        ///     The right button visibility
        /// </summary>
        private Visibility _rightButtonVisibility;

        /// <summary>
        ///     Gets or sets the root.
        /// </summary>
        /// <value>
        ///     The root.
        /// </value>
        private string _root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        ///     The rotate backward command
        /// </summary>
        private ICommand _rotateBackwardCommandCommand;

        /// <summary>
        ///     The rotate forward command
        /// </summary>
        private ICommand _rotateForwardCommand;

        /// <summary>
        ///     The save command
        /// </summary>
        private ICommand _saveCommand;

        /// <summary>
        ///     The scale command
        /// </summary>
        private ICommand _scaleCommand;

        /// <summary>
        ///     The search command
        /// </summary>
        private ICommand _searchCommand;

        /// <summary>
        ///     The selected fill type
        /// </summary>
        private string _selectedFillType;

        /// <summary>
        ///     The selected filter
        /// </summary>
        private string _selectedFilter;

        /// <summary>
        ///     The selected form
        /// </summary>
        private SelectionTools _selectedForm;

        /// <summary>
        ///     The selected frame command
        /// </summary>
        private ICommand _selectedFrameCommand;

        /// <summary>
        ///     The selected point command
        /// </summary>
        private ICommand _selectedPointCommand;

        /// <summary>
        ///     The selected texture
        /// </summary>
        private string _selectedTexture;

        /// <summary>
        ///     The selected tool
        /// </summary>
        private ImageTools _selectedTool;

        /// <summary>
        ///     The selected tool type
        /// </summary>
        private string _selectedToolType;

        /// <summary>
        ///     The similar command
        /// </summary>
        private ICommand _similarCommand;

        /// <summary>
        ///     The similarity in Percent for a Image, Start value is 90
        ///     Configured from Register
        /// </summary>
        private int _similarity;

        /// <summary>
        ///     The status image
        /// </summary>
        private string _statusImage;

        /// <summary>
        ///     Check if Subfolders should be used too
        /// </summary>
        private bool _subFolders;

        /// <summary>
        ///     The texture configuration command
        /// </summary>
        private ICommand _textureConfigCommand;

        /// <summary>
        ///     The thumb image clicked command
        /// </summary>
        private ICommand _thumbImageClickedCommand;

        /// <summary>
        ///     The thumbnail visibility
        /// </summary>
        private Visibility _thumbnailVisibility;

        /// <summary>
        ///     Check if we show thumbnails.
        /// </summary>
        private bool _thumbs = true;

        /// <summary>
        ///     The tolerance
        /// </summary>
        private double _tolerance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageView" /> class.
        ///     Initiates all necessary Collections as well
        /// </summary>
        public ImageView()
        {
            Initialize();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageView" /> class.
        ///     Initiates all necessary Collections as well
        /// </summary>
        /// <param name="subFolders">if set to <c>true</c> [sub folders].</param>
        /// <param name="compressCif">if set to <c>true</c> [compress cif].</param>
        /// <param name="similarity">The similarity.</param>
        /// <param name="autoClean">if set to <c>true</c> [automatic clean].</param>
        public ImageView(bool subFolders, bool compressCif, int similarity, bool autoClean)
        {
            SubFolders = subFolders;
            Compress = compressCif;
            Similarity = similarity;
            AutoClean = autoClean;

            Initialize();
        }

        /// <summary>
        ///     Gets or sets the current filter.
        /// </summary>
        /// <value>
        ///     The current filter.
        /// </value>
        private ImageFilters CurrentFilter { get; set; }

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
        ///     The Bitmap
        /// </summary>
        /// <value>
        ///     The BTM.
        /// </value>
        private Bitmap Btm
        {
            get => _btm;
            set
            {
                _btm = value;
                NavigationLogic(); // Call a method whenever the property is set
            }
        }

        /// <summary>
        ///     Gets or sets the thumb.
        /// </summary>
        /// <value>
        ///     The thumb.
        /// </value>
        public Thumbnails Thumb { get; init; }

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        /// <value>
        ///     The color.
        /// </value>
        public ColorHsv Color { get; set; }

        /// <summary>
        ///     Sets the status.
        /// </summary>
        /// <value>
        ///     The status.
        /// </value>
        public ColorPickerMenu Picker { get; init; }

        /// <summary>
        ///     Gets the selections.
        /// </summary>
        /// <value>
        ///     The selections.
        /// </value>
        public IEnumerable<SelectionTools> Selections =>
            Enum.GetValues(typeof(SelectionTools))
                .Cast<SelectionTools>();

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
        ///     Gets or sets the selected tool.
        /// </summary>
        /// <value>
        ///     The selected tool.
        /// </value>
        public SelectionTools SelectedForm
        {
            get => _selectedForm;
            set => SetProperty(ref _selectedForm, value, nameof(SelectedForm));
        }

        /// <summary>
        ///     Gets or sets the type of the selected tool.
        /// </summary>
        /// <value>
        ///     The type of the selected tool.
        /// </value>
        public string SelectedToolType
        {
            get => _selectedToolType;
            set => SetProperty(ref _selectedToolType, value, nameof(SelectedToolType));
        }

        /// <summary>
        ///     Gets or sets the size of the brush.
        /// </summary>
        /// <value>
        ///     The size of the brush.
        /// </value>
        public double BrushSize
        {
            get => _brushSize;
            set => SetProperty(ref _brushSize, value, nameof(BrushSize));
        }

        /// <summary>
        ///     Gets or sets the tolerance.
        /// </summary>
        /// <value>
        ///     The tolerance.
        /// </value>
        public double Tolerance
        {
            get => _tolerance;
            set => SetProperty(ref _tolerance, value, nameof(Tolerance));
        }

        /// <summary>
        ///     Gets or sets the type of the selected fill.
        /// </summary>
        /// <value>
        ///     The type of the selected fill.
        /// </value>
        public string SelectedFillType
        {
            get => _selectedFillType;
            set => SetProperty(ref _selectedFillType, value, nameof(SelectedFillType));
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
            get => _information;
            set => SetProperty(ref _information, value, nameof(Information));
        }

        /// <summary>
        ///     Gets or sets the similarity. In percent, other values that are bigger or smaller won't be accepted.
        /// </summary>
        /// <value>
        ///     The similarity.
        /// </value>
        public int Similarity
        {
            get => _similarity;
            set
            {
                if (value is >= 0 and <= 100) // Only set if value is within valid range
                {
                    SetProperty(ref _similarity, value, nameof(Similarity));
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
            get => _count;
            set
            {
                SetProperty(ref _count, value, nameof(Count));
                NavigationLogic();
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
            get => _fileName;
            set => SetProperty(ref _fileName, value, nameof(FileName));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value, nameof(IsActive));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is image active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is image active; otherwise, <c>false</c>.
        /// </value>
        public bool IsImageActive
        {
            get => _isImageActive;
            set => SetProperty(ref _isImageActive, value, nameof(IsImageActive));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [left button visibility].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [left button visibility]; otherwise, <c>false</c>.
        /// </value>
        public Visibility LeftButtonVisibility
        {
            get => _leftButtonVisibility;
            set => SetProperty(ref _leftButtonVisibility, value, nameof(LeftButtonVisibility));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [right button visibility].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [right button visibility]; otherwise, <c>false</c>.
        /// </value>
        public Visibility RightButtonVisibility
        {
            get => _rightButtonVisibility;
            set => SetProperty(ref _rightButtonVisibility, value, nameof(RightButtonVisibility));
        }

        /// <summary>
        ///     Gets or sets the thumbnail visibility.
        /// </summary>
        /// <value>
        ///     The thumbnail visibility.
        /// </value>
        public Visibility ThumbnailVisibility
        {
            get => _thumbnailVisibility;
            set => SetProperty(ref _thumbnailVisibility, value, nameof(ThumbnailVisibility));
        }


        /// <summary>
        ///     Gets or sets a value indicating whether [sub folders].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [sub folders]; otherwise, <c>false</c>.
        /// </value>
        public bool SubFolders
        {
            get => _subFolders;
            set
            {
                SetProperty(ref _subFolders, value, nameof(SubFolders));
                SlimViewerRegister.MainSubFolders = value;
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
            get => _autoClean;
            set
            {
                SetProperty(ref _autoClean, value, nameof(AutoClean));
                SlimViewerRegister.MainAutoClean = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ImageView" /> shows Thumbnails.
        /// </summary>
        /// <value>
        ///     <c>true</c> if thumbs; otherwise, <c>false</c>.
        /// </value>
        public bool Thumbs
        {
            get => _thumbs;
            set
            {
                SetProperty(ref _thumbs, value, nameof(Thumbs));
                NavigationLogic();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ImageView" /> compresses the new CIF format.
        /// </summary>
        /// <value>
        ///     <c>true</c> if compress; otherwise, <c>false</c>.
        /// </value>
        public bool Compress
        {
            get => _compress;
            set => SetProperty(ref _compress, value, nameof(Compress));
        }

        /// <summary>
        ///     Gets or sets the observer.
        /// </summary>
        /// <value>
        ///     The observer.
        /// </value>
        public Dictionary<int, string> Observer
        {
            get => _observer;
            set => SetProperty(ref _observer, value, nameof(Observer));
        }

        /// <summary>
        ///     Gets or sets the BitmapImage.
        /// </summary>
        /// <value>
        ///     The BitmapImage.
        /// </value>
        public BitmapImage Bmp
        {
            get => _bmp;
            set => SetProperty(ref _bmp, value, nameof(Bmp));
        }

        /// <summary>
        ///     Gets or sets the status image.
        /// </summary>
        /// <value>
        ///     The status image.
        /// </value>
        public string StatusImage
        {
            get => _statusImage;
            set => SetProperty(ref _statusImage, value, nameof(StatusImage));
        }

        /// <summary>
        ///     Gets or sets the GIF path.
        /// </summary>
        /// <value>
        ///     The GIF path.
        /// </value>
        public string GifPath
        {
            get => _gifPath;
            set => SetProperty(ref _gifPath, value, nameof(GifPath));
        }

        /// <summary>
        ///     Gets the close command.
        /// </summary>
        /// <value>
        ///     The close command.
        /// </value>
        public ICommand CloseCommand =>
            _closeCommand ??= new DelegateCommand<object>(CloseAction, CanExecute);

        /// <summary>
        ///     Gets the open CBZ command.
        /// </summary>
        /// <value>
        ///     The open CBZ command.
        /// </value>
        public ICommand OpenCbzCommand =>
            _openCbzCommand ??= new DelegateCommand<object>(OpenCbzAction, CanExecute);

        /// <summary>
        ///     Gets the open command.
        /// </summary>
        /// <value>
        ///     The open command.
        /// </value>
        public ICommand OpenCommand =>
            _openCommand ??= new DelegateCommand<object>(OpenAction, CanExecute);

        /// <summary>
        ///     Gets the open command.
        /// </summary>
        /// <value>
        ///     s
        ///     The open command.
        /// </value>
        public ICommand SaveCommand =>
            _saveCommand ??= new DelegateCommand<object>(SaveAction, CanExecute);


        /// <summary>
        ///     Gets the delete command.
        /// </summary>
        /// <value>
        ///     The delete command.
        /// </value>
        public ICommand DeleteCommand =>
            _deleteCommand ??= new DelegateCommand<object>(DeleteAction, CanExecute);

        /// <summary>
        ///     Gets the refresh command.
        /// </summary>
        /// <value>
        ///     The refresh command.
        /// </value>
        public ICommand RefreshCommand =>
            _refreshCommand ??= new DelegateCommand<object>(RefreshAction, CanExecute);

        /// <summary>
        ///     Gets the pixelate.
        /// </summary>
        /// <value>
        ///     The pixelate.
        /// </value>
        public ICommand Pixelate =>
            _pixelateCommand ??= new DelegateCommand<object>(PixelateAction, CanExecute);

        /// <summary>
        ///     Gets the compare command.
        /// </summary>
        /// <value>
        ///     The compare command.
        /// </value>
        public ICommand SimilarCommand =>
            _similarCommand ??= new DelegateCommand<object>(SimilarWindowAction, CanExecute);

        /// <summary>
        ///     Gets the compare command.
        /// </summary>
        /// <value>
        ///     The compare command.
        /// </value>
        public ICommand DuplicateCommand =>
            _duplicateCommand ??= new DelegateCommand<object>(DuplicateWindowAction, CanExecute);

        /// <summary>
        ///     Gets the rename command.
        /// </summary>
        /// <value>
        ///     The rename command.
        /// </value>
        public ICommand RenameCommand =>
            _renameCommand ??= new AsyncDelegateCommand<object>(RenameAction, CanExecute);

        /// <summary>
        ///     Gets the folder command.
        /// </summary>
        /// <value>
        ///     The folder command.
        /// </value>
        public ICommand FolderCommand =>
            _folderCommand ??= new DelegateCommand<object>(FolderAction, CanExecute);

        /// <summary>
        ///     Gets the mirror command.
        /// </summary>
        /// <value>
        ///     The mirror command.
        /// </value>
        public ICommand MirrorCommand =>
            _mirrorCommand ??= new DelegateCommand<object>(MirrorAction, CanExecute);

        /// <summary>
        ///     Gets the rotate forward command.
        /// </summary>
        /// <value>
        ///     The rotate forward command.
        /// </value>
        public ICommand RotateForwardCommand =>
            _rotateForwardCommand ??= new DelegateCommand<object>(RotateForwardAction, CanExecute);

        /// <summary>
        ///     Gets the rotate backward command.
        /// </summary>
        /// <value>
        ///     The rotate backward command.
        /// </value>
        public ICommand RotateBackwardCommand => _rotateBackwardCommandCommand ??=
            new DelegateCommand<object>(RotateBackwardAction, CanExecute);

        /// <summary>
        ///     Gets the explorer command.
        /// </summary>
        /// <value>
        ///     The explorer command.
        /// </value>
        public ICommand ExplorerCommand => _explorerCommand ??= new DelegateCommand<object>(ExplorerAction, CanExecute);

        /// <summary>
        ///     Gets the scale command.
        /// </summary>
        /// <value>
        ///     The scale command.
        /// </value>
        public ICommand ScaleCommand =>
            _scaleCommand ??= new DelegateCommand<object>(ScaleWindowAction, CanExecute);

        /// <summary>
        ///     Gets the folder rename command.
        /// </summary>
        /// <value>
        ///     The folder rename command.
        /// </value>
        public ICommand FolderRenameCommand =>
            _folderRenameCommand ??= new DelegateCommand<object>(FolderRenameWindowAction, CanExecute);

        /// <summary>
        ///     Gets the folder convert command.
        /// </summary>
        /// <value>
        ///     The folder convert command.
        /// </value>
        public ICommand FolderConvertCommand =>
            _folderConvertCommand ??= new DelegateCommand<object>(FolderConvertWindowAction, CanExecute);

        /// <summary>
        ///     Gets the folder convert command.
        /// </summary>
        /// <value>
        ///     The folder convert command.
        /// </value>
        public ICommand ClearCommand => _clearCommand ??= new DelegateCommand<object>(ClearAction, CanExecute);

        /// <summary>
        ///     Gets the folder convert command.
        /// </summary>
        /// <value>
        ///     The folder convert command.
        /// </value>
        public ICommand CleanTempFolder =>
            _cleanTempFolder ??= new DelegateCommand<object>(CleanTempAction, CanExecute);

        /// <summary>
        ///     Gets the folder search command.
        /// </summary>
        /// <value>
        ///     The folder search command.
        /// </value>
        public ICommand FolderSearchCommand =>
            _searchCommand ??= new DelegateCommand<object>(SearchWindowAction, CanExecute);

        /// <summary>
        ///     Gets the move command.
        /// </summary>
        /// <value>
        ///     The move command.
        /// </value>
        public ICommand MoveCommand =>
            _moveCommand ??= new DelegateCommand<object>(MoveAction, CanExecute);

        /// <summary>
        ///     Gets the move all command.
        /// </summary>
        /// <value>
        ///     The move all command.
        /// </value>
        public ICommand MoveAllCommand =>
            _moveAllCommand ??= new DelegateCommand<object>(MoveAllAction, CanExecute);

        /// <summary>
        ///     Gets the open Cif command.
        /// </summary>
        /// <value>
        ///     The open Cif command.
        /// </value>
        public ICommand OpenCommandCif =>
            _openCommandCif ??= new DelegateCommand<object>(OpenCifAction, CanExecute);

        /// <summary>
        ///     Gets the convert cif command.
        /// </summary>
        /// <value>
        ///     The convert cif command.
        /// </value>
        public ICommand ConvertCommandCif =>
            _convertCommandCif ??= new DelegateCommand<object>(ConvertCifAction, CanExecute);

        /// <summary>
        ///     The GIF window command.
        /// </summary>
        /// <value>
        ///     The GIF window command.
        /// </value>
        public ICommand GifWindowCommand =>
            _gifWindowCommand ??= new DelegateCommand<object>(GifWindowAction, CanExecute);

        /// <summary>
        ///     Gets the analyzer window command.
        /// </summary>
        /// <value>
        ///     The analyzer window command.
        /// </value>
        public ICommand AnalyzerWindowCommand =>
            _analyzerWindowCommand ??= new DelegateCommand<object>(AnalyzerWindowAction, CanExecute);

        /// <summary>
        ///     Gets the export string command.
        /// </summary>
        /// <value>
        ///     The export string command.
        /// </value>
        public ICommand ExportStringCommand =>
            _exportStringCommand ??= new DelegateCommand<object>(ExportStringAction, CanExecute);

        /// <summary>
        ///     Gets the resizer window command.
        /// </summary>
        /// <value>
        ///     The resizer window command.
        /// </value>
        public ICommand ResizerWindowCommand =>
            _resizerWindowCommand ??= new DelegateCommand<object>(ResizerWindowAction, CanExecute);


        /// <summary>
        ///     Gets the apply filter command.
        /// </summary>
        /// <value>
        ///     The apply filter command.
        /// </value>
        public ICommand ApplyFilterCommand =>
            _applyFilterCommand ??= new DelegateCommand<string>(ApplyFilterAction, CanExecute);


        /// <summary>
        ///     Gets the apply texture command.
        ///     TODO implement
        /// </summary>
        /// <value>
        ///     The apply texture command.
        /// </value>
        public ICommand ApplyTextureCommand =>
            _applyTextureCommand ??= new DelegateCommand<string>(ApplyTextureAction, CanExecute);

        /// <summary>
        ///     Gets the filter configuration command.
        /// </summary>
        /// <value>
        ///     The filter configuration command.
        /// </value>
        public ICommand FilterConfigCommand =>
            _filterConfigCommand ??= new DelegateCommand<string>(FilterConfigWindowAction, CanExecute);

        /// <summary>
        ///     Gets the filter configuration command.
        /// </summary>
        /// <value>
        ///     The filter configuration command.
        /// </value>
        public ICommand TextureConfigCommand =>
            _textureConfigCommand ??= new DelegateCommand<string>(TextureConfigWindowAction, CanExecute);


        /// <summary>
        ///     Gets the brighten command.
        /// </summary>
        /// <value>
        ///     The brighten command.
        /// </value>
        public ICommand BrightenCommand =>
            _brightenCommand ??= new DelegateCommand<string>(BrightenAction, CanExecute);

        /// <summary>
        ///     Gets the darken command.
        /// </summary>
        /// <value>
        ///     The darken command.
        /// </value>
        public ICommand DarkenCommand =>
            _darkenCommand ??= new DelegateCommand<string>(DarkenAction, CanExecute);

        /// <summary>
        ///     Gets the thumb image clicked command.
        /// </summary>
        /// <value>
        ///     The thumb image clicked command.
        /// </value>
        public ICommand ThumbImageClickedCommand =>
            _thumbImageClickedCommand ??= new DelegateCommand<ImageEventArgs>(ThumbImageClickedAction, CanExecute);

        /// <summary>
        ///     Gets the image loaded command.
        /// </summary>
        /// <value>
        ///     The image loaded command.
        /// </value>
        public ICommand ImageLoadedCommand =>
            _imageLoadedCommand ??= new DelegateCommand<object>(ImageLoadedCommandAction, CanExecute);

        /// <summary>
        ///     Gets the selected point command.
        /// </summary>
        /// <value>
        ///     The selected point command.
        /// </value>
        public ICommand SelectedPointCommand =>
            _selectedPointCommand ??= new DelegateCommand<Point>(SelectedPointAction, CanExecute);

        /// <summary>
        ///     Gets the selected frame command.
        /// </summary>
        /// <value>
        ///     The selected frame command.
        /// </value>
        public ICommand SelectedFrameCommand =>
            _selectedFrameCommand ??= new DelegateCommand<SelectionFrame>(SelectedFrameAction, CanExecute);

        /// <summary>
        ///     Gets the color changed command.
        /// </summary>
        /// <value>
        ///     The color changed command.
        /// </value>
        public ICommand ColorChangedCommand =>
            _colorChangedCommand ??= new DelegateCommand<ColorHsv>(ColorChangedAction, CanExecute);

        /// <summary>
        ///     Gets the next command.
        /// </summary>
        /// <value>
        ///     The next command.
        /// </value>
        public ICommand NextCommand =>
            _nextCommand ??= new DelegateCommand<object>(NextAction, CanExecute);

        /// <summary>
        ///     Gets the previous command.
        /// </summary>
        /// <value>
        ///     The previous command.
        /// </value>
        public ICommand PreviousCommand =>
            _previousCommand ??= new DelegateCommand<object>(PreviousAction, CanExecute);

        /// <summary>
        ///     Gets or sets the main.
        /// </summary>
        /// <value>
        ///     The main.
        /// </value>
        public Window Main { get; init; }

        /// <summary>
        ///     Gets or sets the image zoom.
        /// </summary>
        /// <value>
        ///     The image zoom.
        /// </value>
        public ImageZoom ImageZoom { get; init; }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            _cif = new CustomImageFormat();
            Observer = new Dictionary<int, string>();

            _greenIcon = Path.Combine(_root, ViewResources.IconPathGreen);
            _redIcon = Path.Combine(_root, ViewResources.IconPathRed);

            LeftButtonVisibility = RightButtonVisibility = Visibility.Hidden;
            ThumbnailVisibility = Visibility.Visible;
            IsImageActive = false;

            //TODO check if textbox is selected

            // Initialize key bindings using DelegateCommand<T>
            CommandBindings = new Dictionary<Key, ICommand>
            {
                { Key.O, OpenCommand },
                { Key.S, SaveCommand },
                { Key.Delete, DeleteCommand },
                { Key.F5, RefreshCommand },
                { Key.Left, PreviousCommand },
                { Key.Right, NextCommand }
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
            switch (e.PropertyName)
            {
                case nameof(SelectedToolType):
                    SelectedForm = Translator.GetToolsFromString(SelectedToolType);
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
        ///     Gets a value indicating whether this instance can execute.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     <c>true</c> if this instance can execute the specified object; otherwise, <c>false</c>.
        /// </returns>
        /// <value>
        ///     <c>true</c> if this instance can execute; otherwise, <c>false</c>.
        /// </value>
        private bool CanExecute(object obj)
        {
            // check if executing is allowed, not used right now
            return true;
        }

        /// <summary>
        ///     Determines whether this instance can execute the specified object.
        /// </summary>
        /// <typeparam name="T">Generic Parameter</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     <c>true</c> if this instance can execute the specified object; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecute<T>(T obj)
        {
            // check if executing is allowed, not used right now
            return true;
        }

        /// <summary>
        ///     Thumbs the image clicked action.
        /// </summary>
        /// <param name="obj">The identifier.</param>
        private void ThumbImageClickedAction(ImageEventArgs obj)
        {
            ChangeImage(obj.Id);
        }

        /// <summary>
        ///     Set the selected point.
        /// </summary>
        /// <param name="wPoint">The w point.</param>
        private void SelectedPointAction(Point wPoint)
        {
            if (SelectedForm != SelectionTools.Trace)
                return;

            var point = new System.Drawing.Point((int)wPoint.X, (int)wPoint.Y);

            switch (SelectedTool)
            {
                case ImageTools.Paint:
                    if (Color == null) return;

                    var color = Color.GetDrawingColor();

                    Btm = ImageProcessor.SetPixel(Btm, point, color);

                    Bmp = Btm.ToBitmapImage();
                    return;

                case ImageTools.ColorSelect:
                    Color = ImageProcessor.GetPixel(Btm, point);
                    Picker.SetColors(Color.R, Color.G, Color.B, Color.A);
                    Color = Picker.Colors;
                    return;
            }
        }

        /// <summary>
        ///     Selected frame.
        /// </summary>
        /// <param name="frame">The selected area.</param>
        private void SelectedFrameAction(SelectionFrame frame)
        {
            if (SelectedForm == SelectionTools.Move)
                return;
            if (SelectedForm == SelectionTools.Trace)
                return;

            var point = new System.Drawing.Point(frame.X, frame.Y);

            switch (SelectedTool)
            {
                case ImageTools.Erase:
                    Btm = ImageProcessor.EraseImage(frame, Btm);
                    break;
                case ImageTools.Cut:
                    Btm = ImageProcessor.CutImage(frame, Btm);
                    break;

                case ImageTools.Paint:
                    if (Color == null) return;

                    var color = Color.GetDrawingColor();

                    Btm = ImageProcessor.SetPixel(Btm, point, color);
                    return;

                case ImageTools.ColorSelect:
                    Color = ImageProcessor.GetPixel(Btm, point);
                    return;
            }

            Bmp = Btm.ToBitmapImage();
        }

        /// <summary>
        ///     Colors the changed action.
        /// </summary>
        /// <param name="colorHsv">The color HSV.</param>
        private void ColorChangedAction(ColorHsv colorHsv)
        {
            Color = colorHsv;
        }


        /// <summary>
        ///     Closes the app
        /// </summary>
        /// <param name="obj">The object.</param>
        private void CloseAction(object obj)
        {
            var config = SlimViewerRegister.GetRegister();
            config.MainAutoPlayGif = ImageZoom.AutoplayGifImage;

            Config.SetConfig(config);
            if (AutoClean) CleanTempAction(true);

            Application.Current.Shutdown();
        }

        /// <summary>
        ///     Opens a picture
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OpenAction(object obj)
        {
            var pathObj = FileIoHandler.HandleFileOpen(ViewResources.FileOpen, SlimViewerRegister.CurrentFolder);

            if (string.IsNullOrEmpty(pathObj?.FilePath)) return;

            SlimViewerRegister.CurrentFolder = pathObj.Folder;

            //handle cbz files
            if (string.Equals(pathObj.Extension, ViewResources.CbzExt, StringComparison.OrdinalIgnoreCase))
            {
                GenerateCbrView(pathObj);
                return;
            }

            //check if file extension is supported
            if (!ImagingResources.Appendix.Contains(pathObj.Extension.ToLower()))
            {
                _ = MessageBox.Show(string.Concat(ViewResources.ErrorFileNotSupported, pathObj.Extension),
                    ViewResources.ErrorMessage);
                return;
            }

            GenerateView(pathObj.FilePath);
            LoadThumbs(pathObj.Folder, pathObj.FilePath);

            //activate Menus
            if (_bmp != null) IsActive = true;
        }

        /// <summary>
        ///     Opens a CBR Format.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OpenCbzAction(object obj)
        {
            var pathObj =
                FileIoHandler.HandleFileOpen(ViewResources.FileOpenCbz, SlimViewerRegister.CurrentFolder);

            if (pathObj == null || !File.Exists(pathObj.FilePath)) return;

            GenerateCbrView(pathObj);

            //activate Menus
            if (_bmp != null) IsActive = true;
        }

        /// <summary>
        ///     Open the cif Format.
        ///     Todo TEST
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OpenCifAction(object obj)
        {
            var pathObj =
                FileIoHandler.HandleFileOpen(ViewResources.FileOpenCif, SlimViewerRegister.CurrentFolder);

            if (pathObj == null || !File.Exists(pathObj.FilePath)) return;

            Btm = _cif.GetImageFromCif(pathObj.FilePath);

            if (Btm == null) return;

            //activate Menus
            if (_bmp != null) IsActive = true;

            Bmp = Btm.ToBitmapImage();

            //set Filename
            FileName = Path.GetFileName(_filePath);
            //set Infos
            Information = ViewResources.BuildImageInformation(_filePath, FileName, Bmp);
        }

        /// <summary>
        ///     Convert the cif Format.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ConvertCifAction(object obj)
        {
            var pathObj = FileIoHandler.HandleFileOpen(ViewResources.FileOpen, SlimViewerRegister.CurrentFolder);

            if (pathObj == null || !File.Exists(pathObj.FilePath)) return;

            if (Compress) _cif.GenerateCifCompressedFromBitmap(Btm, pathObj.FilePath);
            else _cif.GenerateBitmapToCifFile(Btm, pathObj.FilePath);
        }

        /// <summary>
        ///     Saves the picture.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SaveAction(object obj)
        {
            if (Bmp == null) return;

            var btm = Bmp.ToBitmap();

            var pathObj = FileIoHandler.HandleFileSave(ViewResources.FileOpen, SlimViewerRegister.CurrentFolder);

            if (pathObj == null) return;

            if (string.Equals(pathObj.FilePath, _filePath, StringComparison.OrdinalIgnoreCase))
                _ = FileHandleDelete.DeleteFile(_filePath);

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
        private void NextAction(object obj)
        {
            var lst = Observer.Keys.ToList();
            if (lst.IsNullOrEmpty()) return;

            ChangeImage(Utility.GetNextElement(_currentId, lst));
            Thumb.Next();
            NavigationLogic();
        }

        /// <summary>
        ///     Previous Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void PreviousAction(object obj)
        {
            var lst = Observer.Keys.ToList();
            if (lst.IsNullOrEmpty()) return;

            ChangeImage(Utility.GetPreviousElement(_currentId, lst));
            Thumb.Previous();
            NavigationLogic();
        }

        /// <summary>
        ///     Applies the filter.
        /// </summary>
        /// <param name="filterName">The filter name.</param>
        private void ApplyFilterAction(string filterName)
        {
            var filter = Translator.GetFilterFromString(filterName);

            var btm = ImageProcessor.Filter(Btm, filter);
            Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        ///     Applies the texture.
        /// </summary>
        /// <param name="textureName">The name of the texture.</param>
        private void ApplyTextureAction(string textureName)
        {
            var texture = Translator.GetTextureFromString(textureName);

            var btm = ImageProcessor.Texture(Btm, texture);
            Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        ///     Brightens the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void BrightenAction(object obj)
        {
            var btm = ImageProcessor.DBrighten(Btm);
            Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        ///     Darkens the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void DarkenAction(object obj)
        {
            var btm = ImageProcessor.Darken(Btm);
            Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        ///     Pixelate action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void PixelateAction(object obj)
        {
            var btm = ImageProcessor.Pixelate(Btm, PixelWidth);
            Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        ///     Deletes the Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void DeleteAction(object obj)
        {
            if (!Observer.ContainsKey(_currentId) && Thumb.Selection.IsNullOrEmpty()) return;

            if (!Thumb.Selection.IsNullOrEmpty())
            {
                var count = 0;
                foreach (var id in Thumb.Selection)
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
                Btm = null;
                GifPath = null;
                _gifPath = null;

                try
                {
                    var check = FileHandleSafeDelete.DeleteFile(Observer[_currentId]);

                    //decrease File Count
                    if (Count > 0 && check) Count--;
                }
                catch (FileHandlerException ex)
                {
                    Trace.WriteLine(ex);
                    _ = MessageBox.Show(ex.ToString(),
                        string.Concat(ViewResources.ErrorMessage, nameof(DeleteAction)));
                }

                Thumb.RemoveSingleItem(_currentId);

                NextAction(this);
            }
        }

        /// <summary>
        ///     Renames the Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async Task RenameAction(object obj)
        {
            if (!IsActive) return;
            if (!Observer.ContainsKey(_currentId)) return;

            var file = Observer[_currentId];
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

            Observer[_currentId] = filePath;
            GenerateView(filePath);
        }

        /// <summary>
        ///     Refresh the Control
        /// </summary>
        /// <param name="obj">The object.</param>
        private void RefreshAction(object obj)
        {
            _currentId = -1;
            _filePath = string.Empty;
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
        private void ExportStringAction(object obj)
        {
            ImageProcessor.ExportString(Btm);
        }

        /// <summary>
        ///     Rotates the backward action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void RotateBackwardAction(object obj)
        {
            Btm = ImageProcessor.RotateImage(Btm, -90);
            Bmp = Btm.ToBitmapImage();
        }

        /// <summary>
        ///     Rotates the forward action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void RotateForwardAction(object obj)
        {
            Btm = ImageProcessor.RotateImage(Btm, 90);
            Bmp = Btm.ToBitmapImage();
        }

        /// <summary>
        ///     Mirrors the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void MirrorAction(object obj)
        {
            if (Btm == null) return;

            Btm.RotateFlip(RotateFlipType.RotateNoneFlipX);
            Bmp = Btm.ToBitmapImage();
        }

        /// <summary>
        ///     Open Folder
        /// </summary>
        /// <param name="obj">The object.</param>
        private void FolderAction(object obj)
        {
            //get target Folder
            var path = FileIoHandler.ShowFolder(SlimViewerRegister.CurrentFolder);

            if (!Directory.Exists(path)) return;

            LoadThumbs(path);

            //activate Menus
            if (!string.IsNullOrEmpty(path)) IsActive = true;
        }

        /// <summary>
        ///     Clears the Image the current View.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ClearAction(object obj)
        {
            if (!Observer.ContainsKey(_currentId)) return;

            Bmp = null;
            GifPath = null;

            Thumb.RemoveSingleItem(_currentId);

            //decrease File Count
            if (Count > 0) Count--;

            Btm = null;
            Bmp = null;
            GifPath = null;
            _gifPath = null;

            NextAction(this);
        }

        /// <summary>
        ///     Open the Explorer
        ///     https://ss64.com/nt/explorer.html
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ExplorerAction(object obj)
        {
            if (!Directory.Exists(SlimViewerRegister.CurrentFolder)) return;

            var argument = !File.Exists(_filePath)
                ? SlimViewerRegister.CurrentFolder
                : string.Concat(ViewResources.Select, _filePath, ViewResources.Close);
            _ = Process.Start(ViewResources.Explorer, argument);
        }

        /// <summary>
        ///     Filter configuration Window.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void FilterConfigWindowAction(string obj)
        {
            var filterConfig = new FilterConfig
            {
                Topmost = true,
                Owner = Main
            };
            filterConfig.Show();
        }

        /// <summary>
        ///     Textures the configuration Window.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void TextureConfigWindowAction(string obj)
        {
            var textureConfig = new TextureConfig
            {
                Topmost = true,
                Owner = Main
            };
            textureConfig.Show();
        }

        /// <summary>
        ///     Rename the Folder action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void FolderRenameWindowAction(object obj)
        {
            SlimViewerRegister.ResetRenaming();

            var dct = new Dictionary<int, string>();

            if (!Thumb.Selection.IsNullOrEmpty())
                foreach (var id in Thumb.Selection.Where(id => _observer.ContainsKey(id)))
                    dct.Add(id, _observer[id]);
            else
                dct = new Dictionary<int, string>(_observer);

            var rename = new Rename(dct)
            {
                Topmost = true,
                Owner = Main
            };
            _ = rename.ShowDialog();

            //refresh the Filename, no need to refresh all, we don't need to reload everything, to save time
            if (SlimViewerRegister.Changed && Thumb.Selection.IsNullOrEmpty())
                _observer = rename.Observer;
            else
                foreach (var (key, value) in rename.Observer)
                    _observer[key] = value;
        }

        /// <summary>
        ///     Image Scaling Window.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ScaleWindowAction(object obj)
        {
            SlimViewerRegister.ResetScaling();

            var scaleWindow = new Scale
            {
                Topmost = true,
                Owner = Main
            };
            scaleWindow.ShowDialog();

            //_btm = ImageProcessor.Render.BitmapScaling(_btm, SlimViewerRegister.Scaling);
            Btm = ImageProcessor.BitmapScaling(Btm, SlimViewerRegister.Scaling);
            Btm = ImageProcessor.RotateImage(Btm, SlimViewerRegister.Degree);
            Btm = ImageProcessor.CropImage(Btm);
            Bmp = Btm.ToBitmapImage();
        }

        /// <summary>
        ///     Folder convert Window.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void FolderConvertWindowAction(object obj)
        {
            SlimViewerRegister.ResetConvert();

            var converterWindow = new Converter
            {
                Topmost = true,
                Owner = Main
            };
            _ = converterWindow.ShowDialog();

            if (string.IsNullOrEmpty(SlimViewerRegister.Target) ||
                string.IsNullOrEmpty(SlimViewerRegister.Source)) return;

            //TODO implement a viewmodel


            ImageProcessor.FolderConvert(SlimViewerRegister.Target, SlimViewerRegister.Source, _observer);
        }

        /// <summary>
        ///     Similar Action Window.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SimilarWindowAction(object obj)
        {
            var compareWindow = new Compare(SubFolders, SlimViewerRegister.CurrentFolder, this, Similarity)
            {
                Topmost = true,
                Owner = Main
            };
            compareWindow.Show();


            SlimViewerRegister.CompareView = true;
        }

        /// <summary>
        ///     Duplicate Action Window.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void DuplicateWindowAction(object obj)
        {
            var compareWindow = new Compare(SubFolders, SlimViewerRegister.CurrentFolder, this)
            {
                Topmost = true,
                Owner = Main
            };
            compareWindow.Show();

            SlimViewerRegister.CompareView = true;
        }

        /// <summary>
        ///     GIFs window action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void GifWindowAction(object obj)
        {
            var gifWindow = new Gif
            {
                Topmost = true,
                Owner = Main
            };
            gifWindow.Show();
        }

        /// <summary>
        ///     Analyzer Window.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void AnalyzerWindowAction(object obj)
        {
            var detailWindow = new DetailCompare
            {
                Topmost = true,
                Owner = Main
            };
            detailWindow.Show();
        }

        /// <summary>
        ///     Resizer Window.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ResizerWindowAction(object obj)
        {
            var resizer = new Resizer
            {
                Topmost = true,
                Owner = Main
            };
            resizer.Show();
        }

        /// <summary>
        ///     Searches Window action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SearchWindowAction(object obj)
        {
            var searchWindow = new Search(SubFolders, SlimViewerRegister.CurrentFolder, this, Color)
            {
                Topmost = true,
                Owner = Main
            };
            searchWindow.Show();
        }

        /// <summary>
        ///     Cleans the temporary Folder action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void CleanTempAction(object obj)
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
        private void MoveAction(object obj)
        {
            if (!File.Exists(FileName) && Thumb.Selection.IsNullOrEmpty()) return;
            //Initiate Folder
            if (string.IsNullOrEmpty(SlimViewerRegister.CurrentFolder))
                SlimViewerRegister.CurrentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //get target Folder
            var path = FileIoHandler.ShowFolder(SlimViewerRegister.CurrentFolder ?? Directory.GetCurrentDirectory());

            if (!Thumb.Selection.IsNullOrEmpty())
            {
                var count = 0;

                foreach (var id in Thumb.Selection)
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
        private void MoveAllAction(object obj)
        {
            //Initiate Folder
            if (string.IsNullOrEmpty(SlimViewerRegister.CurrentFolder))
                SlimViewerRegister.CurrentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //get target Folder
            var path = FileIoHandler.ShowFolder(SlimViewerRegister.CurrentFolder ?? Directory.GetCurrentDirectory());

            if (!Directory.Exists(path)) return;

            if (_fileList.IsNullOrEmpty()) return;

            var lst = FileHandleSearch.GetFilesByExtensionFullPath(path, ImagingResources.Appendix, _subFolders);

            if (lst == null) return;

            var i = _fileList.Intersect(lst);

            if (i.Any())
            {
                var dialogResult = MessageBox.Show(ViewResources.MessageFileAlreadyExists,
                    ViewResources.CaptionFileAlreadyExists,
                    MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.No) return;
            }

            //Move all Contents from this folder into another
            _ = FileHandleCut.CutFiles(_fileList, path, false);
        }

        /// <summary>
        ///     Changes the image.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void ChangeImage(int id)
        {
            if (!Observer.ContainsKey(id)) return;

            _currentId = id;
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
            var lst = files.ToList();
            _fileList = lst;

            Count = lst.Count;

            try
            {
                SlimViewerRegister.CurrentFolder = Path.GetDirectoryName(lst[0]);
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

            _currentId = -1;

            _ = GenerateThumbView(lst);
            _information = string.Concat(ViewResources.DisplayImages, lst.Count);
        }

        /// <summary>
        ///     Changes the image.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void ChangeImage(string filePath)
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
            _currentId = Observer.FirstOrDefault(x => x.Value == filePath).Key;
        }

        /// <summary>
        ///     Changes the image.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="info">The information about the selected Images.</param>
        internal void ChangeImage(IEnumerable<string> files, string filePath, string info)
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
            _currentId = Observer.FirstOrDefault(x => x.Value == filePath).Key;

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

            _subFolders = true;
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
        private void GenerateView(string filePath)
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
        private void GenerateImage(string filePath)
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
                    Btm = ImageProcessor.Render.GetOriginalBitmap(filePath);

                    //reset gif Image
                    GifPath = null;

                    Bmp = Btm.ToBitmapImage();
                    //set Infos
                    Information = ViewResources.BuildImageInformation(filePath, FileName, Bmp);
                }

                _filePath = filePath;
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
        private void LoadThumbs(string folder, string filePath = null)
        {
            GenerateThumbView(folder);

            // If filePath is provided, get the Id of the displayed image.
            if (!string.IsNullOrEmpty(filePath))
            {
                _currentId = Observer.FirstOrDefault(x => x.Value == filePath).Key;
            }
            else
            {
                // Reset the Id of the displayed image if no file path is provided.
                _currentId = -1;
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
            StatusImage = _redIcon;

            _fileList = FileHandleSearch.GetFilesByExtensionFullPath(folder, ImagingResources.Appendix, _subFolders);

            //decrease File Count
            if (_fileList.IsNullOrEmpty())
            {
                Count = 0;
                Observer = null;
                Bmp = null;
                GifPath = null;
                return;
            }

            NavigationLogic();

            // ReSharper disable once PossibleNullReferenceException, already checked
            Count = _fileList.Count;

            _fileList = _fileList.PathSort();

            _ = GenerateThumbView(_fileList);
        }

        /// <summary>
        ///     Generates the thumb view.
        /// </summary>
        /// <param name="lst">The File List.</param>
        private async Task GenerateThumbView(IReadOnlyCollection<string> lst)
        {
            //if we don't want to generate Thumbs don't
            if (!Thumbs) return;

            _root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(_root)) return;

            StatusImage = _redIcon;

            //load Thumbnails
            _ = await Task.Run(() => Observer = lst.ToDictionary()).ConfigureAwait(false);

            NavigationLogic();
        }

        /// <summary>
        ///     Navigation logic.
        /// </summary>
        private void NavigationLogic()
        {
            if (_count <= 1)
            {
                LeftButtonVisibility = RightButtonVisibility = Visibility.Hidden;
            }
            else
            {
                // Set visibility based on _currentId and _count
                RightButtonVisibility = _currentId == _count - 1 ? Visibility.Hidden : Visibility.Visible;
                LeftButtonVisibility = _currentId <= 0 ? Visibility.Hidden : Visibility.Visible;
            }

            // show or hide the Thumbnail Bar
            ThumbnailVisibility = Thumbs ? Visibility.Visible : Visibility.Hidden;

            //show or hide image edit
            IsImageActive = Btm != null;
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
            StatusImage = _redIcon;

            var check = ImageProcessor.SaveImage(path, extension, btm);
            StatusImage = _greenIcon;

            return check;
        }

        /// <summary>
        ///     Thumbnails are loaded.
        /// </summary>
        public void ImageLoadedCommandAction(object obj)
        {
            //if (Status == null) return;
            if (string.IsNullOrEmpty(StatusImage)) return;

            StatusImage = _greenIcon;
        }
    }
}