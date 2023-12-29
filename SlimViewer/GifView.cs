/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViewer/GifView.cs
 * PURPOSE:     View Model for the Gif Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CommonControls;
using ExtendedSystemObjects;
using FileHandler;
using Imaging;
using ViewModel;

namespace SlimViewer
{
    /// <inheritdoc />
    /// <summary>
    ///     Gif Viewer
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public sealed class GifView : INotifyPropertyChanged
    {
        /// <summary>
        ///     The BMP
        /// </summary>
        private BitmapImage _bmp;

        /// <summary>
        ///     The clear command
        /// </summary>
        private ICommand _clearCommand;

        /// <summary>
        ///     The close command
        /// </summary>
        private ICommand _closeCommand;

        /// <summary>
        ///     The current folder
        /// </summary>
        private string _currentFolder = Directory.GetCurrentDirectory();

        /// <summary>
        ///     The current identifier
        /// </summary>
        private int _currentId;

        /// <summary>
        ///     The GIF export
        /// </summary>
        private string _gifExport;

        /// <summary>
        ///     The GIF path
        /// </summary>
        private string _gifPath;

        /// <summary>
        ///     The image export
        /// </summary>
        private string _imageExport;

        /// <summary>
        ///     The is active
        /// </summary>
        private bool _isActive;

        /// <summary>
        ///     The observer
        /// </summary>
        private Dictionary<int, string> _observer;

        /// <summary>
        ///     The open command
        /// </summary>
        private ICommand _openCommand;

        /// <summary>
        ///     The open folder command
        /// </summary>
        private ICommand _openFolderCommand;

        /// <summary>
        ///     The output command
        /// </summary>
        private ICommand _outputCommand;

        /// <summary>
        ///     The save GIF command
        /// </summary>
        private ICommand _saveGifCommand;

        /// <summary>
        ///     The save images command
        /// </summary>
        private ICommand _saveImagesCommand;

        /// <summary>
        ///     Gets the open command.
        /// </summary>
        /// <value>
        ///     The open command.
        /// </value>
        public ICommand OpenCommand =>
            _openCommand ??= new DelegateCommand<object>(OpenAction, CanExecute);

        /// <summary>
        ///     Gets the open folder command.
        /// </summary>
        /// <value>
        ///     The open folder command.
        /// </value>
        public ICommand OpenFolderCommand =>
            _openFolderCommand ??= new DelegateCommand<object>(OpenFolderAction, CanExecute);

        /// <summary>
        ///     Gets the open folder command.
        /// </summary>
        /// <value>
        ///     The open folder command.
        /// </value>
        public ICommand OutputCommand =>
            _outputCommand ??= new DelegateCommand<object>(OutputAction, CanExecute);

        /// <summary>
        ///     Gets the clear command.
        /// </summary>
        /// <value>
        ///     The clear command.
        /// </value>
        public ICommand ClearCommand =>
            _clearCommand ??= new DelegateCommand<object>(ClearAction, CanExecute);


        /// <summary>
        ///     Gets the save GIF command.
        /// </summary>
        /// <value>
        ///     The save GIF command.
        /// </value>
        public ICommand SaveGifCommand =>
            _saveGifCommand ??= new DelegateCommand<object>(SaveGifAction, CanExecute);

        /// <summary>
        ///     Gets the save images command.
        /// </summary>
        /// <value>
        ///     The save images command.
        /// </value>
        public ICommand SaveImagesCommand =>
            _saveImagesCommand ??= new DelegateCommand<object>(SaveImagesAction, CanExecute);

        /// <summary>
        ///     Gets the close command.
        /// </summary>
        /// <value>
        ///     The close command.
        /// </value>
        public ICommand CloseCommand =>
            _closeCommand ??= new DelegateCommand<object>(CloseAction, CanExecute);

        /// <summary>
        ///     Gets or sets the image.
        /// </summary>
        /// <value>
        ///     The image.
        /// </value>
        public ImageZoom Image { get; set; }

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
        ///     Changes the image.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void ChangeImage(int id)
        {
            if (!Observer.ContainsKey(id)) return;

            _currentId = id;

            var filePath = Observer[id];
            //TODO!
        }

        /// <summary>
        ///     Generates the thumb view.
        /// </summary>
        /// <param name="lst">The File List.</param>
        private async Task GenerateThumbView(IReadOnlyCollection<string> lst)
        {
            //load Thumbnails
            _ = await Task.Run(() => Observer = lst.ToDictionary()).ConfigureAwait(false);
        }

        /// <summary>
        ///     Opens the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OpenAction(object obj)
        {
            //Initiate Folder
            if (string.IsNullOrEmpty(_currentFolder)) _currentFolder = Directory.GetCurrentDirectory();

            var pathObj = FileIoHandler.HandleFileOpen(SlimViewerResources.FileOpenGif, _currentFolder);

            if (pathObj == null || !File.Exists(pathObj.FilePath) ||
                !string.Equals(pathObj.Extension, ImagingResources.GifExt)) return;

            Initiate();

            _gifPath = pathObj.FilePath;
            Helper.ConvertGifAction(_gifPath, _imageExport);
            _currentFolder = _imageExport;

            var fileList =
                FileHandleSearch.GetFilesByExtensionFullPath(_currentFolder, ImagingResources.JpgExt, false);
            _ = GenerateThumbView(fileList);
        }

        /// <summary>
        ///     Opens the folder action.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OpenFolderAction(object obj)
        {
            //Initiate Folder
            if (string.IsNullOrEmpty(_currentFolder)) _currentFolder = Directory.GetCurrentDirectory();

            //get target Folder
            var path = FileIoHandler.ShowFolder(_currentFolder);

            var fileList =
                FileHandleSearch.GetFilesByExtensionFullPath(path, ImagingResources.Appendix, false);

            if (fileList.Count >= 200)
                //TODO error
                return;

            Initiate();

            _currentFolder = path;
            _ = GenerateThumbView(fileList);

            _gifPath = Helper.ConvertToGifAction(path, _gifPath);
        }

        private void OutputAction(object obj)
        {
            throw new NotImplementedException();
        }

        private void ClearAction(object obj)
        {
        }

        private void SaveImagesAction(object obj)
        {
            throw new NotImplementedException();
        }

        private void CloseAction(object obj)
        {
        }

        private void SaveGifAction(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Initiates this instance.
        /// </summary>
        private void Initiate()
        {
            var root = Path.Combine(_currentFolder, SlimViewerResources.GifPath);
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

            _imageExport = Path.Combine(root, SlimViewerResources.ImagesPath);
            {
                Directory.CreateDirectory(_imageExport);
            }

            _gifExport = Path.Combine(root, SlimViewerResources.NewGifPath);
            {
                Directory.CreateDirectory(_gifExport);
            }
        }
    }
}