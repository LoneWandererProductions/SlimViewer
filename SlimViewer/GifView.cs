﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViewer/GifView.cs
 * PURPOSE:     View Model for the Gif Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable MemberCanBeInternal

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        ///     The automatic clear
        ///     Configured from Register
        /// </summary>
        private bool _autoClear = SlimViewerRegister.GifCleanUp;

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
        ///     The file path
        /// </summary>
        private string _filePath;

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
        ///     The information
        /// </summary>
        private string _information;

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
        ///     The output path
        /// </summary>
        private string _outputPath = Directory.GetCurrentDirectory();

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
        ///     Gets or sets the file path.
        /// </summary>
        /// <value>
        ///     The file path.
        /// </value>
        public string FilePath
        {
            get => _filePath;
            set
            {
                if (value == _filePath) return;

                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        /// <summary>
        ///     Gets or sets the output path.
        /// </summary>
        /// <value>
        ///     The output path.
        /// </value>
        public string OutputPath
        {
            get => _outputPath;
            set
            {
                if (value == _outputPath) return;

                _outputPath = value;
                OnPropertyChanged(nameof(OutputPath));
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

        /// <summary>
        ///     Gets or sets a value indicating whether [automatic clear].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [automatic clear]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoClear
        {
            get => _autoClear;
            set
            {
                if (_autoClear == value) return;

                _autoClear = value;
                SlimViewerRegister.GifCleanUp = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        /// <summary>
        ///     Gets or sets the thumbnail.
        /// </summary>
        /// <value>
        ///     The thumbnail.
        /// </value>
        internal Thumbnails Thumbnail { private get; set; }

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

            var filePath = Observer[id];
            GifPath = null;
            var bmp = Helper.Render.GetBitmapImage(filePath);
            Bmp = bmp;

            var fileName = Path.GetFileName(filePath);

            var info = ImageGifHandler.GetImageInfo(filePath);

            if (info == null)
            {
                Information = string.Concat(SlimViewerResources.MessageErrorFileNotFound, filePath);
                return;
            }

            //set Infos
            Information = string.Concat(SlimViewerResources.ImagePath, filePath, SlimViewerResources.ImageName,
                fileName, SlimViewerResources.ImageHeight, bmp.Height, SlimViewerResources.ImageWidth,
                bmp.Width,
                SlimViewerResources.ImageSize, bmp.Height * bmp.Width);
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
            Initiate(OutputPath);

            var pathObj = FileIoHandler.HandleFileOpen(SlimViewerResources.FileOpenGif, OutputPath);

            if (pathObj == null || !File.Exists(pathObj.FilePath) ||
                !string.Equals(pathObj.Extension, ImagingResources.GifExt)) return;

            GifPath = pathObj.FilePath;

            FilePath = GifPath;

            var info = ImageGifHandler.GetImageInfo(GifPath);
            if (info == null)
            {
                Information = string.Concat(SlimViewerResources.MessageErrorFileNotFound, FilePath);
                return;
            }

            //set Infos
            Information = string.Concat(GifPath, SlimViewerResources.ImageName,
                info.Name, SlimViewerResources.ImageHeight, info.Height, SlimViewerResources.ImageWidth,
                info.Width,
                SlimViewerResources.ImageSize, info.Size, SlimViewerResources.Frames.Length, info.Frames);

            //add name of the split files
            var name = Path.Combine(_imageExport, SlimViewerResources.ImagesPath);
            Helper.ConvertGifAction(GifPath, name);
            var currentFolder = _imageExport;

            var fileList =
                FileHandleSearch.GetFilesByExtensionFullPath(currentFolder, ImagingResources.JpgExt, false);
            _ = GenerateThumbView(fileList);
        }

        /// <summary>
        ///     Opens the folder action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OpenFolderAction(object obj)
        {
            Initiate(OutputPath);

            //get target Folder
            var path = FileIoHandler.ShowFolder(OutputPath);

            var fileList =
                FileHandleSearch.GetFilesByExtensionFullPath(path, ImagingResources.Appendix, false);

            if (fileList is not { Count: < 200 })
                //TODO MessageBox
                return;

            _ = GenerateThumbView(fileList);

            _gifPath = Helper.ConvertToGifAction(path, _gifPath);

            FilePath = _gifPath;

            var info = ImageGifHandler.GetImageInfo(_gifPath);
            if (info == null)
            {
                Information = string.Concat(SlimViewerResources.MessageErrorFileNotFound, FilePath);
                return;
            }

            //set Infos
            Information = string.Concat(_gifPath, SlimViewerResources.ImageName,
                info.Name, SlimViewerResources.ImageHeight, info.Height, SlimViewerResources.ImageWidth,
                info.Width,
                SlimViewerResources.ImageSize, info.Size, SlimViewerResources.Frames.Length, info.Frames);
        }

        /// <summary>
        ///     Clears the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ClearAction(object obj)
        {
            if (Directory.Exists(OutputPath)) Directory.Delete(OutputPath, true);
        }

        /// <summary>
        ///     Closes the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void CloseAction(object obj)
        {
            ClearAction(null);
            Application.Current.Shutdown();
        }

        /// <summary>
        ///     Saves the GIF action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SaveGifAction(object obj)
        {
            var pathObj = FileIoHandler.HandleFileSave(SlimViewerResources.FileOpenGif, OutputPath);

            if (pathObj == null) return;

            var lst = Thumbnail.Selection.Select(id => Observer[id]).ToList();
            lst = lst.PathSort();

            Helper.ConvertGifAction(lst, pathObj.FilePath);
        }

        /// <summary>
        ///     Saves the images action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SaveImagesAction(object obj)
        {
            //get target Folder
            var path = FileIoHandler.ShowFolder(OutputPath);

            if (string.IsNullOrEmpty(path)) return;

            var lst = Thumbnail.Selection.Select(id => Observer[id]).ToList();

            _ = FileHandleCopy.CopyFiles(lst, path, false);
        }

        /// <summary>
        ///     Initiates this instance.
        /// </summary>
        /// <param name="path"></param>
        private void Initiate(string path)
        {
            //if exists, clean up
            if (Directory.Exists(path)) Directory.Delete(path, true);

            _ = Directory.CreateDirectory(path);

            OutputPath = path;

            _imageExport = Path.Combine(path, SlimViewerResources.ImagesPath);
            {
                Directory.CreateDirectory(_imageExport);
            }

            _gifExport = Path.Combine(path, SlimViewerResources.NewGifPath);
            {
                Directory.CreateDirectory(_gifExport);
            }

            IsActive = true;
        }
    }
}