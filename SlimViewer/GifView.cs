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
using System.Diagnostics;
using System.IO;
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
        private string _currentFolder;

        /// <summary>
        ///     The current identifier
        /// </summary>
        private int _currentId;

        /// <summary>
        ///     The GIF path
        /// </summary>
        private string _gifPath;

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
        /// The is active
        /// </summary>
        private bool _isActive;

        /// <summary>
        /// Gets the open command.
        /// </summary>
        /// <value>
        /// The open command.
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
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
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
            GenerateView(filePath);
        }

        /// <summary>
        ///     Generates the view.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private void GenerateView(string filePath)
        {
            _currentFolder = filePath;
            LoadThumbs();
        }

        /// <summary>
        ///     Loads the thumbs.
        /// </summary>
        private void LoadThumbs()
        {
            var fileList =
                FileHandleSearch.GetFilesByExtensionFullPath(_currentFolder, ImagingResources.Appendix, false);
            _ = GenerateThumbView(fileList);
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
            var pathObj = FileIoHandler.HandleFileOpen(SlimViewerResources.FileOpenGif, _currentFolder);

            if (pathObj == null || !File.Exists(pathObj.FilePath) || !string.Equals(pathObj.Extension, "gif")) return;

            //TODO still shit
            var lst = Helper.Render.LoadGif(pathObj.FilePath);

            _gifPath = pathObj.FilePath;
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

            if (!Directory.Exists(path)) return;


            //TODO still shit
        }

        private void OutputAction(object obj)
        {
            throw new NotImplementedException();
        }

        private void ClearAction(object obj)
        {
            throw new NotImplementedException();
        }

        private void SaveImagesAction(object obj)
        {
            throw new NotImplementedException();
        }

        private void CloseAction(object obj)
        {
            throw new NotImplementedException();
        }

        private void SaveGifAction(object obj)
        {
            throw new NotImplementedException();
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
                    var check = Helper.SaveImage(path, ImagingResources.JpgExt, image);
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
    }
}