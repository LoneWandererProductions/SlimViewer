/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViewer/ImageView.cs
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
using ExtendedSystemObjects;
using FileHandler;
using Imaging;
using ViewModel;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

// TODO Save and Export Settings
// TODO Activity Monitor
// TODO improve Image Compare Interface
// TODO add Zoom lock and rotate as it's own Control
// TODO add more User Feedback

namespace SlimViewer
{
    public sealed class ImageView : INotifyPropertyChanged
    {
        /// <summary>
        ///     The render
        /// </summary>
        private readonly CustomImageFormat _cif;

        /// <summary>
        ///     The black and white command.
        /// </summary>
        private ICommand _blackAndWhite;

        /// <summary>
        ///     The BitmapImage
        /// </summary>
        private BitmapImage _bmp;

        /// <summary>
        ///     The Bitmap
        /// </summary>
        private Bitmap _btm;

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
        ///     Check if we compress cif files.
        /// </summary>
        private bool _compress;

        /// <summary>
        ///     The convert cif command.
        /// </summary>
        private ICommand _convertCommandCif;

        /// <summary>
        ///     The convert gif command
        /// </summary>
        private ICommand _convertCommandGif;

        /// <summary>
        ///     The convert to gif command
        /// </summary>
        private ICommand _convertCommandToGif;

        /// <summary>
        ///     The File count
        /// </summary>
        private int _count;

        /// <summary>
        ///     The current folder
        /// </summary>
        private string _currentFolder;

        /// <summary>
        ///     The current identifier of the Image
        /// </summary>
        private int _currentId;

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
        ///     The gray scale command
        /// </summary>
        private ICommand _grayScaleCommand;

        /// <summary>
        ///     The information
        /// </summary>
        private string _information;

        /// <summary>
        ///     The invert command
        /// </summary>
        private ICommand _invertCommand;

        /// <summary>
        ///     Is the Menu active
        /// </summary>
        private bool _isActive;

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
        ///     The polaroid command
        /// </summary>
        private ICommand _polaroidCommand;

        /// <summary>
        ///     The previous command
        /// </summary>
        private ICommand _previousCommand;

        /// <summary>
        ///     The refresh command
        /// </summary>
        private ICommand _refreshCommand;

        /// <summary>
        ///     The rename command
        /// </summary>
        private ICommand _renameCommand;

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
        ///     The selected tool
        /// </summary>
        private SelectionTools _selectedTool;

        /// <summary>
        ///     The sepia command
        /// </summary>
        private ICommand _sepiaCommand;

        /// <summary>
        ///     The similar command
        /// </summary>
        private ICommand _similarCommand;

        /// <summary>
        ///     The similarity in Percent for a Image, Start value is 90
        ///     Configured from Register
        /// </summary>
        private int _similarity = SlimViewerRegister.MainSimilarity;

        /// <summary>
        ///     Check if Subfolders should be used too
        /// </summary>
        private bool _subFolders;

        /// <summary>
        ///     Check if we show thumbnails.
        /// </summary>
        private bool _thumbs = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageView" /> class.
        ///     Initiates all necessary Collections as well
        /// </summary>
        public ImageView()
        {
            _cif = new CustomImageFormat();
            Observer = new Dictionary<int, string>();
        }

        /// <summary>
        ///     Gets or sets the thumb.
        /// </summary>
        /// <value>
        ///     The thumb.
        /// </value>
        internal Thumbnails Thumb { get; set; }

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        /// <value>
        ///     The color.
        /// </value>
        internal ColorHsv Color { get; set; }

        /// <summary>
        ///     Sets the status.
        /// </summary>
        /// <value>
        ///     The status.
        /// </value>
        internal ColorPickerMenu Picker { get; set; }

        /// <summary>
        ///     Sets the status Image.
        /// </summary>
        /// <value>
        ///     The status Image.
        /// </value>
        internal Image Status { private get; set; }

        /// <summary>
        ///     Gets or sets the selected tool.
        /// </summary>
        /// <value>
        ///     The selected tool.
        /// </value>
        public SelectionTools SelectedTool
        {
            get => _selectedTool;
            set
            {
                _selectedTool = value;
                OnPropertyChanged(nameof(SelectedTool));
            }
        }

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
        ///     Gets or sets the basic File information.
        /// </summary>
        /// <value>
        ///     The information.
        /// </value>
        public string Information
        {
            get => _information;
            set
            {
                if (value == _information) return;

                _information = value;
                OnPropertyChanged(nameof(Information));
            }
        }

        /// <summary>
        ///     Gets or sets the similarity. In percent, other Values that are bigger or smaller won't be accepted
        /// </summary>
        /// <value>
        ///     The similarity.
        /// </value>
        public int Similarity
        {
            get => _similarity;
            set
            {
                if (value == _similarity) return;
                if (value is > 100 or < 0) return;

                _similarity = value;
                SlimViewerRegister.MainSimilarity = value;

                OnPropertyChanged(nameof(Similarity));
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
                if (value == _count) return;

                _count = value;
                OnPropertyChanged(nameof(Count));
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
            set
            {
                if (_fileName == value) return;

                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
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
            set
            {
                if (_isActive == value) return;

                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
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
                if (_subFolders == value) return;

                _subFolders = value;
                OnPropertyChanged(nameof(SubFolders));
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
                if (_thumbs == value) return;

                _thumbs = value;
                OnPropertyChanged(nameof(Thumbs));
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ImageView" /> compresses the new cif format.
        /// </summary>
        /// <value>
        ///     <c>true</c> if thumbs; otherwise, <c>false</c>.
        /// </value>
        public bool Compress
        {
            get => _compress;
            set
            {
                if (_compress == value) return;

                _compress = value;
                OnPropertyChanged(nameof(Compress));
            }
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
            set
            {
                if (_observer == value) return;

                _observer = value;
                OnPropertyChanged(nameof(Observer));
            }
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
            set
            {
                if (_bmp == value) return;

                _bmp = value;
                OnPropertyChanged(nameof(Bmp));
            }
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
            set
            {
                if (_gifPath == value) return;

                _gifPath = value;
                OnPropertyChanged(nameof(GifPath));
            }
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
        ///     Gets the open command.
        /// </summary>
        /// <value>
        ///     The open command.
        /// </value>
        public ICommand OpenCommand =>
            _openCommand ??= new DelegateCommand<object>(OpenAction, CanExecute);

        public ICommand OpenCbzCommand =>
            _openCbzCommand ??= new DelegateCommand<object>(OpenCbzAction, CanExecute);

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
        ///     Gets the sepia command.
        /// </summary>
        /// <value>
        ///     The sepia command.
        /// </value>
        public ICommand SepiaCommand =>
            _sepiaCommand ??= new DelegateCommand<object>(SepiaAction, CanExecute);

        /// <summary>
        ///     Gets the gray scale command.
        /// </summary>
        /// <value>
        ///     The gray scale command.
        /// </value>
        public ICommand GrayScaleCommand =>
            _grayScaleCommand ??= new DelegateCommand<object>(GrayScaleAction, CanExecute);

        /// <summary>
        ///     Gets the gray scale command.
        /// </summary>
        /// <value>
        ///     The gray scale command.
        /// </value>
        public ICommand BlackAndWhite =>
            _blackAndWhite ??= new DelegateCommand<object>(BlackAndWhiteAction, CanExecute);

        /// <summary>
        ///     Gets the invert command.
        /// </summary>
        /// <value>
        ///     The invert command.
        /// </value>
        public ICommand InvertCommand =>
            _invertCommand ??= new DelegateCommand<object>(InvertAction, CanExecute);

        /// <summary>
        ///     Gets the polaroid command.
        /// </summary>
        /// <value>
        ///     The polaroid command.
        /// </value>
        public ICommand PolaroidCommand =>
            _polaroidCommand ??= new DelegateCommand<object>(PolaroidAction, CanExecute);

        /// <summary>
        ///     Gets the compare command.
        /// </summary>
        /// <value>
        ///     The compare command.
        /// </value>
        public ICommand SimilarCommand =>
            _similarCommand ??= new DelegateCommand<object>(SimilarAction, CanExecute);

        /// <summary>
        ///     Gets the compare command.
        /// </summary>
        /// <value>
        ///     The compare command.
        /// </value>
        public ICommand DuplicateCommand =>
            _duplicateCommand ??= new DelegateCommand<object>(DuplicateAction, CanExecute);

        /// <summary>
        ///     Gets the previous Image.
        /// </summary>
        /// <value>
        ///     The previous Image.
        /// </value>
        public ICommand PreviousCommand => _previousCommand ??= new DelegateCommand<object>(PreviousAction, CanExecute);

        /// <summary>
        ///     Gets the next Image.
        /// </summary>
        /// <value>
        ///     The next Image.
        /// </value>
        public ICommand NextCommand =>
            _nextCommand ??= new DelegateCommand<object>(NextAction, CanExecute);

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
        ///     Gets the rename command.
        /// </summary>
        /// <value>
        ///     The rename command.
        /// </value>
        public ICommand RenameCommand =>
            _renameCommand ??= new DelegateCommand<object>(RenameAction, CanExecute);

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
            _scaleCommand ??= new DelegateCommand<object>(ScaleAction, CanExecute);

        /// <summary>
        ///     Gets the folder rename command.
        /// </summary>
        /// <value>
        ///     The folder rename command.
        /// </value>
        public ICommand FolderRenameCommand =>
            _folderRenameCommand ??= new DelegateCommand<object>(FolderRenameAction, CanExecute);

        /// <summary>
        ///     Gets the folder convert command.
        /// </summary>
        /// <value>
        ///     The folder convert command.
        /// </value>
        public ICommand FolderConvertCommand =>
            _folderConvertCommand ??= new DelegateCommand<object>(FolderConvertAction, CanExecute);

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
            _searchCommand ??= new DelegateCommand<object>(SearchAction, CanExecute);

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
        ///     Gets the convert gif command.
        /// </summary>
        /// <value>
        ///     The convert GIF to jpg.
        /// </value>
        public ICommand ConvertCommandGif =>
            _convertCommandGif ??= new DelegateCommand<object>(ConvertGifAction, CanExecute);


        /// <summary>
        ///     Gets the convert to gif Command.
        /// </summary>
        /// <value>
        ///     Converts Images to gif.
        /// </value>
        public ICommand ConvertCommandToGif =>
            _convertCommandToGif ??= new DelegateCommand<object>(ConvertToGifAction, CanExecute);

        /// <summary>
        ///     The GIF window command.
        /// </summary>
        /// <value>
        ///     The GIF window command.
        /// </value>
        public ICommand GifWindowCommand =>
            _gifWindowCommand ??= new DelegateCommand<object>(GifWindowAction, CanExecute);

        /// <summary>
        ///     Gets or sets the main.
        /// </summary>
        /// <value>
        ///     The main.
        /// </value>
        public MainWindow Main { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Triggers if an Attribute gets changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        ///     Closes the app
        /// </summary>
        /// <param name="obj">The object.</param>
        private void CloseAction(object obj)
        {
            var config = SlimViewerRegister.GetRegister();
            Config.SetConfig(config);
            Application.Current.Shutdown();
        }

        /// <summary>
        ///     Opens a picture
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OpenAction(object obj)
        {
            var pathObj = FileIoHandler.HandleFileOpen(SlimViewerResources.FileOpen, _currentFolder);

            if (pathObj == null || !File.Exists(pathObj.FilePath)) return;

            if (!string.IsNullOrEmpty(pathObj.Folder)) _currentFolder = pathObj.Folder;

            if (string.Equals(pathObj.Extension, SlimViewerResources.CbzExt, StringComparison.OrdinalIgnoreCase))
            {
                GenerateCbrView(pathObj);
            }
            else
            {
                if (!string.IsNullOrEmpty(pathObj.Folder)) _currentFolder = pathObj.Folder;
                GenerateView(pathObj.FilePath);
                LoadThumbs(pathObj.Folder, pathObj.FilePath);
            }

            //activate Menus
            if (_bmp != null) IsActive = true;
        }

        /// <summary>
        ///     Opens a CBR Format.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OpenCbzAction(object obj)
        {
            var pathObj = FileIoHandler.HandleFileOpen(SlimViewerResources.FileOpenCbz, _currentFolder);

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
            var pathObj = FileIoHandler.HandleFileOpen(SlimViewerResources.FileOpenCif, _currentFolder);

            if (pathObj == null || !File.Exists(pathObj.FilePath)) return;

            _btm = _cif.GetCifFile(pathObj.FilePath);

            if (_btm == null) return;

            //activate Menus
            if (_bmp != null) IsActive = true;

            Bmp = _btm.ToBitmapImage();

            //set Filename
            FileName = Path.GetFileName(_filePath);
            //set Infos

            Information = string.Concat(SlimViewerResources.ImagePath, _filePath, SlimViewerResources.ImageName,
                FileName, SlimViewerResources.ImageHeight, _btm.Height, SlimViewerResources.ImageWidth, _btm.Width,
                SlimViewerResources.ImageSize, _btm.Height * _btm.Width);
        }

        /// <summary>
        ///     Convert the cif Format.
        ///     Todo TEST
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ConvertCifAction(object obj)
        {
            var pathObj = FileIoHandler.HandleFileOpen(SlimViewerResources.FileOpen, _currentFolder);

            if (pathObj == null || !File.Exists(pathObj.FilePath)) return;

            if (Compress) _cif.CompressedToCifFile(_btm, pathObj.FilePath);
            else _cif.SaveToCifFile(_btm, pathObj.FilePath);
        }

        /// <summary>
        ///     Converts the gif to images action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ConvertGifAction(object obj)
        {
            //Initiate Folder
            if (string.IsNullOrEmpty(_currentFolder)) _currentFolder = Directory.GetCurrentDirectory();

            var pathObj = FileIoHandler.HandleFileOpen(SlimViewerResources.FileOpenGif, _currentFolder);

            if (pathObj == null) return;

            var images = Helper.Render.SplitGif(pathObj.FilePath);

            var count = 0;

            foreach (var image in images)
                try
                {
                    count++;
                    var path = string.Concat(pathObj.Folder, SlimViewerResources.Slash, count);
                    var check = SaveImage(path, ImagingResources.JpgExt, image);
                    if (!check) _ = MessageBox.Show(SlimViewerResources.ErrorCouldNotSaveFile);
                }
                catch (ArgumentException ex)
                {
                    Trace.WriteLine(ex);
                    _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
                }
                catch (IOException ex)
                {
                    Trace.WriteLine(ex);
                    _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
                }
                catch (ExternalException ex)
                {
                    Trace.WriteLine(ex);
                    _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
                }
        }

        /// <summary>
        ///     Converts to GIF action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ConvertToGifAction(object obj)
        {
            //TODo not working correct
            //Initiate Folder
            if (string.IsNullOrEmpty(_currentFolder)) _currentFolder = Directory.GetCurrentDirectory();

            //get target Folder
            var path = FileIoHandler.ShowFolder(_currentFolder);

            var target = string.Concat(path, SlimViewerResources.Slash, SlimViewerResources.NewGif);

            Helper.Render.CreateGif(path, target);
        }

        /// <summary>
        ///     Saves the picture.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SaveAction(object obj)
        {
            if (Bmp == null) return;

            var btm = Bmp.ToBitmap();

            var pathObj = FileIoHandler.HandleFileSave(SlimViewerResources.FileOpen, _currentFolder);

            if (pathObj == null) return;

            if (string.Equals(pathObj.FilePath, _filePath, StringComparison.OrdinalIgnoreCase))
                _ = FileHandleDelete.DeleteFile(_filePath);

            try
            {
                var check = SaveImage(pathObj.FilePath, pathObj.Extension, btm);
                if (!check) _ = MessageBox.Show(SlimViewerResources.ErrorCouldNotSaveFile);
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (IOException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (ExternalException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
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
        }

        /// <summary>
        ///     Gray scales the Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void GrayScaleAction(object obj)
        {
            try
            {
                var btm = Helper.Render.FilterImage(_btm, ImageFilter.GrayScale);
                Bmp = btm.ToBitmapImage();
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (OutOfMemoryException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
        }

        /// <summary>
        ///     Black and White the Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void BlackAndWhiteAction(object obj)
        {
            try
            {
                var btm = Helper.Render.FilterImage(_btm, ImageFilter.BlackAndWhite);
                Bmp = btm.ToBitmapImage();
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (OutOfMemoryException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
        }

        /// <summary>
        ///     Sepia the Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SepiaAction(object obj)
        {
            try
            {
                var btm = Helper.Render.FilterImage(_btm, ImageFilter.Sepia);
                Bmp = btm.ToBitmapImage();
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (OutOfMemoryException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
        }

        /// <summary>
        ///     Inverts the Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void InvertAction(object obj)
        {
            try
            {
                var btm = Helper.Render.FilterImage(_btm, ImageFilter.Invert);
                Bmp = btm.ToBitmapImage();
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (OutOfMemoryException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
        }

        /// <summary>
        ///     Polaroids the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void PolaroidAction(object obj)
        {
            try
            {
                var btm = Helper.Render.FilterImage(_btm, ImageFilter.Polaroid);
                Bmp = btm.ToBitmapImage();
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (OutOfMemoryException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
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
                        _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
                    }

                LoadThumbs(_currentFolder);

                _ = MessageBox.Show(string.Concat(SlimViewerResources.MessageCount, count),
                    SlimViewerResources.MessageSuccess, MessageBoxButton.OK);
            }
            else
            {
                Bmp = null;
                _btm = null;
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
                    _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
                }

                Thumb.RemoveSingleItem(_currentId);

                NextAction(null);
            }
        }

        /// <summary>
        ///     Renames the Image.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void RenameAction(object obj)
        {
            if (!IsActive) return;
            if (!Observer.ContainsKey(_currentId)) return;

            var file = Observer[_currentId];
            if (!File.Exists(file)) return;

            var folder = Path.GetDirectoryName(file);
            if (string.IsNullOrEmpty(folder)) return;

            var filePath = Path.Combine(folder, FileName);

            //Check if we have an duplicate, if true, shall we overwrite?
            if (File.Exists(filePath))
            {
                var dialogResult = MessageBox.Show(SlimViewerResources.MessageFileAlreadyExists,
                    SlimViewerResources.CaptionFileAlreadyExists,
                    MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.No) return;
            }

            try
            {
                if (!FileHandleRename.RenameFile(file, filePath)) return;
            }
            catch (FileHandlerException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
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

            if (!Directory.Exists(_currentFolder))
            {
                Observer = null;
                return;
            }

            LoadThumbs(_currentFolder);
        }

        /// <summary>
        ///     Rotates the backward action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void RotateBackwardAction(object obj)
        {
            try
            {
                _btm = Helper.Render.RotateImage(_btm, -90);
                Bmp = _btm.ToBitmapImage();
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (OverflowException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
        }

        /// <summary>
        ///     Rotates the forward action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void RotateForwardAction(object obj)
        {
            try
            {
                _btm = Helper.Render.RotateImage(_btm, 90);
                Bmp = _btm.ToBitmapImage();
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (OverflowException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
        }

        /// <summary>
        ///     Mirrors the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void MirrorAction(object obj)
        {
            if (_btm == null) return;

            _btm.RotateFlip(RotateFlipType.RotateNoneFlipX);
            Bmp = _btm.ToBitmapImage();
        }

        /// <summary>
        ///     Open Folder
        /// </summary>
        /// <param name="obj">The object.</param>
        private void FolderAction(object obj)
        {
            //Initiate Folder
            if (string.IsNullOrEmpty(_currentFolder)) _currentFolder = Directory.GetCurrentDirectory();

            //get target Folder
            var path = FileIoHandler.ShowFolder(_currentFolder);

            if (!Directory.Exists(path)) return;

            _currentId = -1;

            Bmp = null;
            GifPath = null;

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

            _btm = null;
            Bmp = null;
            GifPath = null;
            _gifPath = null;

            NextAction(null);
        }

        /// <summary>
        ///     Open the Explorer
        ///     https://ss64.com/nt/explorer.html
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ExplorerAction(object obj)
        {
            if (!Directory.Exists(_currentFolder)) return;

            var argument = !File.Exists(_filePath)
                ? _currentFolder
                : string.Concat(SlimViewerResources.Select, _filePath, SlimViewerResources.Close);
            _ = Process.Start(SlimViewerResources.Explorer, argument);
        }

        /// <summary>
        ///     Rename the Folder action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void FolderRenameAction(object obj)
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
        ///     Image Scaling action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ScaleAction(object obj)
        {
            SlimViewerRegister.ResetScaling();

            var scaleWindow = new Scale
            {
                Topmost = true,
                Owner = Main
            };
            scaleWindow.ShowDialog();

            try
            {
                _btm = Helper.Render.BitmapScaling(_btm, SlimViewerRegister.Scaling);
                _btm = Helper.Render.RotateImage(_btm, SlimViewerRegister.Degree);
                _btm = Helper.Render.CropImage(_btm);
                Bmp = _btm.ToBitmapImage();
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (OverflowException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
        }

        /// <summary>
        ///     Folders the convert action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void FolderConvertAction(object obj)
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

            try
            {
                //do not sear in the folder but instead we use the _observer
                var lst = new List<string>();
                lst.AddRange(_observer.Values.Where(element =>
                    Path.GetExtension(element) == SlimViewerRegister.Source));

                var count = 0;
                var error = 0;

                foreach (var check in from image in lst
                         let btm = Helper.Render.GetOriginalBitmap(image)
                         select SaveImage(image, SlimViewerRegister.Target, btm))
                    if (check)
                        count++;
                    else
                        error++;

                _ = MessageBox.Show(string.Concat(SlimViewerResources.InformationConverted, count, Environment.NewLine,
                    SlimViewerResources.InformationErrors, error));
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (IOException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (ExternalException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
        }

        /// <summary>
        ///     Similar Action.
        ///     TODO add Collection Action
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SimilarAction(object obj)
        {
            var compareWindow = new Compare(SubFolders, _currentFolder, this, Similarity)
            {
                Topmost = true,
                Owner = Main
            };
            compareWindow.Show();
        }

        /// <summary>
        ///     Duplicates the action.
        ///     TODO add Collection Action
        /// </summary>
        /// <param name="obj">The object.</param>
        private void DuplicateAction(object obj)
        {
            var compareWindow = new Compare(SubFolders, _currentFolder, this)
            {
                Topmost = true,
                Owner = Main
            };
            compareWindow.Show();
        }

        /// <summary>
        ///     GIFs the window action.
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
        ///     Cleans the temporary Folder action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void CleanTempAction(object obj)
        {
            var root = Path.Combine(Directory.GetCurrentDirectory(), SlimViewerResources.TempFolder);
            try
            {
                _ = FileHandleDelete.DeleteAllContents(root, true);
            }
            catch (FileHandlerException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
                return;
            }

            _ = MessageBox.Show(SlimViewerResources.StatusDone, SlimViewerResources.CaptionDone, MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
        }

        /// <summary>
        ///     Searches the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SearchAction(object obj)
        {
            var searchWindow = new Search(SubFolders, _currentFolder, this, Color)
            {
                Topmost = true,
                Owner = Main
            };
            searchWindow.Show();
        }

        /// <summary>
        ///     Moves selected Image
        /// </summary>
        /// <param name="obj">The object.</param>
        private void MoveAction(object obj)
        {
            if (!File.Exists(FileName) && Thumb.Selection.IsNullOrEmpty()) return;
            //Initiate Folder
            if (string.IsNullOrEmpty(_currentFolder)) _currentFolder = Directory.GetCurrentDirectory();
            //get target Folder
            var path = FileIoHandler.ShowFolder(_currentFolder);

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
                        var dialogResult = MessageBox.Show(SlimViewerResources.MessageFileAlreadyExists,
                            SlimViewerResources.CaptionFileAlreadyExists,
                            MessageBoxButton.YesNo);
                        if (dialogResult == MessageBoxResult.No) continue;
                    }

                    info.MoveTo(target, true);
                    count++;
                }

                _ = MessageBox.Show(string.Concat(SlimViewerResources.MessageMoved, count),
                    SlimViewerResources.MessageSuccess, MessageBoxButton.OK);
            }
            else
            {
                if (!Directory.Exists(path)) return;

                //Copy Single File
                var info = new FileInfo(FileName);
                var target = Path.Combine(path, info.Name);

                if (File.Exists(target))
                {
                    var dialogResult = MessageBox.Show(SlimViewerResources.MessageFileAlreadyExists,
                        SlimViewerResources.CaptionFileAlreadyExists,
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
            if (string.IsNullOrEmpty(_currentFolder)) _currentFolder = Directory.GetCurrentDirectory();

            //get target Folder
            var path = FileIoHandler.ShowFolder(_currentFolder);

            if (!Directory.Exists(path)) return;

            if (_fileList.IsNullOrEmpty()) return;

            var lst = FileHandleSearch.GetFilesByExtensionFullPath(path, ImagingResources.Appendix, _subFolders);

            if (lst == null) return;

            var i = _fileList.Intersect(lst);

            if (i.Any())
            {
                var dialogResult = MessageBox.Show(SlimViewerResources.MessageFileAlreadyExists,
                    SlimViewerResources.CaptionFileAlreadyExists,
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
        internal void ChangeImage(int id)
        {
            if (!Observer.ContainsKey(id)) return;

            _currentId = id;

            var filePath = Observer[id];
            GenerateView(filePath);
        }

        /// <summary>
        ///     Changes the image.
        /// </summary>
        /// <param name="files">The files we want to view.</param>
        internal void ChangeImage(IEnumerable<string> files)
        {
            //no need to check for null it was already checked
            var lst = files.ToList();
            _fileList = lst;

            Count = lst.Count;

            try
            {
                _currentFolder = Path.GetDirectoryName(lst[0]);
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
                return;
            }
            catch (PathTooLongException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
                return;
            }

            Bmp = null;
            GifPath = null;

            _currentId = -1;

            _ = GenerateThumbView(lst);
            _information = string.Concat(SlimViewerResources.DisplayImages, lst.Count);
        }

        /// <summary>
        ///     Changes the image.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        internal void ChangeImage(string filePath)
        {
            //check if it exists
            if (!File.Exists(filePath)) return;

            //check if we even handle this file type
            if (!ImagingResources.Appendix.Any(filePath.EndsWith)) return;

            //load into the Image Viewer
            GenerateView(filePath);

            // load all other Pictures in the Folder
            var folder = Path.GetDirectoryName(filePath);
            if (folder == _currentFolder) return;

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
        ///     Cuts the image.
        /// </summary>
        /// <param name="frame">The frame.</param>
        internal void CutImage(SelectionFrame frame)
        {
            try
            {
                _btm = Helper.Render.CutBitmap(_btm, frame.X, frame.Y, frame.Height, frame.Width);
                Bmp = _btm.ToBitmapImage();
            }
            catch (ArgumentNullException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
        }

        /// <summary>
        ///     Gets the color of the point.
        /// </summary>
        /// <param name="point">The point.</param>
        internal void GetPointColor(Point point)
        {
            var color = _btm.GetPixel((int)point.X, (int)point.Y);
            Picker.SetColors(color.R, color.G, color.B, color.A);
            Color = Picker.Colors;
        }

        /// <summary>
        ///     Generates the CBR view.
        /// </summary>
        /// <param name="pathObj">The path object.</param>
        private void GenerateCbrView(PathObject pathObj)
        {
            _subFolders = true;
            var folder = SlimViewerHelper.UnpackFolder(pathObj.FilePath, pathObj.FileNameWithoutExt);
            if (!string.IsNullOrEmpty(folder)) _currentFolder = folder;
            var file = SlimViewerHelper.UnpackFile(folder);

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
                LoadThumbs(_currentFolder);
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
                    Information = string.Concat(SlimViewerResources.ImagePath, filePath, SlimViewerResources.ImageName,
                        info.Name, SlimViewerResources.ImageHeight, info.Height, SlimViewerResources.ImageWidth,
                        info.Width,
                        SlimViewerResources.ImageSize, info.Size, SlimViewerResources.Frames.Length, info.Frames);
                }

                else
                {
                    _btm = Helper.Render.GetOriginalBitmap(filePath);

                    //reset gif Image
                    GifPath = null;

                    Bmp = _btm.ToBitmapImage();
                    //set Infos
                    Information = string.Concat(SlimViewerResources.ImagePath, filePath, SlimViewerResources.ImageName,
                        FileName, SlimViewerResources.ImageHeight, _btm.Height, SlimViewerResources.ImageWidth,
                        _btm.Width,
                        SlimViewerResources.ImageSize, _btm.Height * _btm.Width);
                }

                _filePath = filePath;
                //set Filename
                FileName = Path.GetFileName(filePath);
            }
            catch (IOException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (NotSupportedException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
        }

        /// <summary>
        ///     Loads the thumbs.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="filePath">The file path.</param>
        private void LoadThumbs(string folder, string filePath)
        {
            GenerateThumbView(folder);

            //Get the Id of the displayed image
            _currentId = Observer.FirstOrDefault(x => x.Value == filePath).Key;
        }

        /// <summary>
        ///     Loads the thumbs.
        /// </summary>
        /// <param name="folder">The folder.</param>
        private void LoadThumbs(string folder)
        {
            GenerateThumbView(folder);

            //Reset the Id of the displayed image
            _currentId = -1;
        }

        /// <summary>
        ///     Generates the thumb view.
        /// </summary>
        /// <param name="folder">The folder.</param>
        private void GenerateThumbView(string folder)
        {
            //initiate Basic values
            _currentFolder = folder;
            Status.Source = null;
            Status.Source = Helper.Render.GetBitmapImageFileStream(Path.Combine(_root,
                SlimViewerResources.IconPathRed));

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

            Status.Source = Helper.Render.GetBitmapImageFileStream(Path.Combine(_root,
                SlimViewerResources.IconPathRed));

            //load Thumbnails
            _ = await Task.Run(() => Observer = lst.ToDictionary()).ConfigureAwait(false);
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
            Status.Source = Helper.Render.GetBitmapImageFileStream(Path.Combine(_root,
                SlimViewerResources.IconPathRed));

            var check = Helper.SaveImage(path, extension, btm);

            Status.Source = Helper.Render.GetBitmapImageFileStream(Path.Combine(_root,
                SlimViewerResources.IconPathGreen));

            return check;
        }

        /// <summary>
        ///     Thumbnails are loaded.
        /// </summary>
        internal void Loaded()
        {
            if (Status == null)
                return;

            Status.Source = Helper.Render.GetBitmapImageFileStream(Path.Combine(_root,
                SlimViewerResources.IconPathGreen));
        }
    }
}