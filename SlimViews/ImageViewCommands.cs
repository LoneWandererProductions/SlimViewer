/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        ImageViewCommands.cs
 * PURPOSE:     Main command definitions and bindings for ImageView, linking UI actions to service methods.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

#nullable enable
using Common.Images;
using Imaging;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewModel;
using Point = System.Windows.Point;

namespace SlimViews
{
    /// <summary>
    /// Provides all image view related commands and links them to the owning view and service classes.
    /// This class is responsible for command creation, binding, and routing actions to the appropriate services.
    /// </summary>
    public class ImageViewCommands
    {
        /// <summary>
        /// The owner, unused for command execution but may be needed for CanExecute checks or future command implementations.
        /// </summary>
        private readonly ImageView _owner;

        /// <summary>
        /// The image mass service
        /// </summary>
        private readonly ImageMassProcessingCommands _imageMassService = new();

        /// <summary>
        /// The image service
        /// </summary>
        private readonly ImageProcessingCommands _imageService = new();

        /// <summary>
        /// The file service
        /// </summary>
        internal FileProcessingCommands FileService { get; } = new();

        #region Commands

        /// <summary>
        /// Gets the close Command.
        /// </summary>
        /// <value>
        /// The close Command.
        /// </value>
        public ICommand Close { get; }

        /// <summary>
        /// Gets the open CBZ Command.
        /// </summary>
        /// <value>
        /// The open CBZ Command.
        /// </value>
        public ICommand OpenCbz { get; }

        /// <summary>
        /// Gets the open.
        /// </summary>
        /// <value>
        /// The open.
        /// </value>
        public ICommand Open { get; }

        /// <summary>
        /// Gets the save.
        /// </summary>
        /// <value>
        /// The save.
        /// </value>
        public ICommand Save { get; }

        /// <summary>
        /// Gets the clipboard.
        /// </summary>
        /// <value>
        /// The clipboard.
        /// </value>
        public ICommand Clipboard { get; }

        /// <summary>
        /// Gets the delete.
        /// </summary>
        /// <value>
        /// The delete.
        /// </value>
        public ICommand Delete { get; }

        /// <summary>
        /// Gets the refresh.
        /// </summary>
        /// <value>
        /// The refresh.
        /// </value>
        public ICommand Refresh { get; }

        /// <summary>
        /// Gets the pixelate.
        /// </summary>
        /// <value>
        /// The pixelate.
        /// </value>
        public ICommand Pixelate { get; }

        /// <summary>
        /// Gets the similar.
        /// </summary>
        /// <value>
        /// The similar.
        /// </value>
        public ICommand Similar { get; }

        /// <summary>
        /// Gets the duplicate.
        /// </summary>
        /// <value>
        /// The duplicate.
        /// </value>
        public ICommand Duplicate { get; }

        /// <summary>
        /// Gets the rename.
        /// </summary>
        /// <value>
        /// The rename.
        /// </value>
        public ICommand Rename { get; }

        /// <summary>
        /// Gets the folder.
        /// </summary>
        /// <value>
        /// The folder.
        /// </value>
        public ICommand Folder { get; }

        public ICommand Mirror { get; }
        public ICommand RotateForward { get; }
        public ICommand RotateBackward { get; }
        public ICommand Explorer { get; }
        public ICommand Scale { get; }
        public ICommand FolderRename { get; }
        public ICommand FolderConvert { get; }

        /// <summary>
        /// Gets the clear.
        /// </summary>
        /// <value>
        /// The clear.
        /// </value>
        public ICommand Clear { get; }

        public ICommand CleanTempFolder { get; }
        public ICommand FolderSearch { get; }
        public ICommand Move { get; }
        public ICommand MoveAll { get; }
        public ICommand OpenCif { get; }
        public ICommand ConvertCif { get; }

        /// <summary>
        /// Gets the GIF window.
        /// </summary>
        /// <value>
        /// The GIF window.
        /// </value>
        public ICommand GifWindow { get; }

        public ICommand AnalyzerWindow { get; }
        public ICommand ExportString { get; }
        public ICommand ResizerWindow { get; }
        public ICommand ApplyFilter { get; }
        public ICommand ApplyTexture { get; }
        public ICommand FilterConfig { get; }
        public ICommand TextureConfig { get; }
        public ICommand Brighten { get; }
        public ICommand Darken { get; }

        public ICommand ThumbImageClicked { get; }
        public ICommand ImageLoaded { get; }
        public ICommand SelectedPoint { get; }
        public ICommand SelectedFrame { get; }

        /// <summary>
        /// Gets the color changed.
        /// </summary>
        /// <value>
        /// The color changed.
        /// </value>
        public ICommand ColorChanged { get; }

        /// <summary>
        /// Gets the next.
        /// </summary>
        /// <value>
        /// The next.
        /// </value>
        public ICommand Next { get; }


        /// <summary>
        /// Gets the previous.
        /// </summary>
        /// <value>
        /// The previous.
        /// </value>
        public ICommand Previous { get; }

        /// <summary>
        /// Gets the show help.
        /// </summary>
        /// <value>
        /// The show help.
        /// </value>
        public ICommand ShowHelp { get; }

        /// <summary>
        /// Gets the show about.
        /// </summary>
        /// <value>
        /// The show about.
        /// </value>
        public ICommand ShowAbout { get; }

        /// <summary>
        /// Gets the undo.
        /// </summary>
        /// <value>
        /// The undo.
        /// </value>
        public ICommand Undo { get; }

        /// <summary>
        /// Gets the redo.
        /// </summary>
        /// <value>
        /// The redo.
        /// </value>
        public ICommand Redo { get; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageViewCommands"/> class.
        /// </summary>
        /// <param name="owner">The owning <see cref="ImageView"/> instance.</param>
        public ImageViewCommands(ImageView owner)
        {
            _owner = owner;

            // Uniform CanExecute
            bool CanRun<T>(T? arg) => owner.CanRun(arg);

            // ---- Helpers (explicit names matching parameter patterns) ----

            // No-parameter on service: Action<ImageView>
            DelegateCommand<object> Make_NoParamCmd(Action<ImageView> svcAction) =>
                new(_ => svcAction(owner), CanRun);

            // Service that expects (ImageView, object?) -> use object command parameter
            DelegateCommand<object> Make_ObjParamCmd(Action<ImageView, object?> svcAction) =>
                new(p => svcAction(owner, p), CanRun);

            // Service that expects (ImageView, string?) -> use string-typed command
            DelegateCommand<string> Make_StringParamCmd(Action<ImageView, string?> svcAction) =>
                new(s => svcAction(owner, s), CanRun);

            // Service that expects (ImageView, SelectionFrame) etc. -- handled directly where needed
            // Bool? parameter commands (for CleanTempFolder)
            DelegateCommand<bool?> Make_BoolParamCmd(Action<bool?> action) =>
                new(action, CanRun);

            // Async commands which take object? parameter and return Task (e.g. Rename)
            AsyncDelegateCommand<object> Make_AsyncObjCmd(Func<object?, Task> asyncAction) =>
                new(asyncAction, CanRun);

            // ---- UI / direct owner commands ----
            Close = new DelegateCommand<object>(owner.CloseAction, CanRun);
            OpenCbz = new DelegateCommand<object>(owner.OpenCbzAction, CanRun);
            Open = new DelegateCommand<object>(owner.OpenAction, CanRun);
            Refresh = new AsyncDelegateCommand<string>(owner.RefreshActionAsync, CanRun);
            Folder = new DelegateCommand<object>(owner.FolderAction, CanRun);
            Clear = new DelegateCommand<object>(owner.ClearAction, CanRun);
            OpenCif = new DelegateCommand<object>(owner.OpenCifAction, CanRun);
            ThumbImageClicked = new DelegateCommand<ImageEventArgs>(owner.ThumbImageClickedAction, CanRun);
            ImageLoaded = new DelegateCommand<object>(owner.ImageLoadedCommandAction, CanRun);
            SelectedPoint = new DelegateCommand<Point>(owner.SelectedPointAction, CanRun);
            SelectedFrame = new DelegateCommand<SelectionFrame>(owner.SelectedFrameAction, CanRun);
            ColorChanged = new DelegateCommand<ColorHsv>(owner.ColorChangedAction, CanRun);
            Next = new DelegateCommand<object>(owner.NextAction, CanRun);
            Previous = new DelegateCommand<object>(owner.PreviousAction, CanRun);
            Explorer = new DelegateCommand<object>(owner.ExplorerAction, CanRun);
            ExportString = new DelegateCommand<object>(owner.ExportStringAction, CanRun);
            Clipboard = new DelegateCommand<object>(owner.ExportClipboardAction, CanRun);

            // ---- UI / direct owner commands ----
            Undo = new DelegateCommand<object>(_ => owner.Undo(), CanRun);
            Redo = new DelegateCommand<object>(_ => owner.Redo(), CanRun);

            // ---- Image mass processing (service methods mostly take ImageView or ImageView+param) ----
            Scale = Make_NoParamCmd(_imageMassService.ScaleWindow);
            FolderConvert = Make_NoParamCmd(_imageMassService.FolderConvertWindow);
            FolderRename = Make_NoParamCmd(_imageMassService.FolderRenameWindow);
            Duplicate = Make_NoParamCmd(_imageMassService.DuplicateWindow);
            Similar = Make_ObjParamCmd(_imageMassService.SimilarWindow);
            FolderSearch = Make_NoParamCmd(_imageMassService.FolderSearch);
            ResizerWindow = Make_NoParamCmd(_imageMassService.ResizerWindow);
            AnalyzerWindow = Make_NoParamCmd(_imageMassService.AnalyzerWindow);
            ShowHelp = Make_NoParamCmd(_imageMassService.ShowHelp);
            ShowAbout = Make_NoParamCmd(_imageMassService.ShowAbout);
            GifWindow = Make_StringParamCmd(_imageMassService.GifWindow);

            // FilterConfig and TextureConfig previously accepted a string? parameter in original
            FilterConfig = Make_StringParamCmd(_imageMassService.FilterConfigWindow);
            TextureConfig = Make_StringParamCmd(_imageMassService.TextureConfigWindow);

            // ---- Image processing (service methods that take owner + param) ----
            Brighten = Make_StringParamCmd(_imageService.Brighten);
            Darken = Make_StringParamCmd(_imageService.Darken);

            ApplyFilter = Make_StringParamCmd(_imageService.ApplyFilter);
            ApplyTexture = Make_StringParamCmd(_imageService.ApplyTexture);

            // Many image ops accept (ImageView, object?) in your original code
            Mirror = Make_ObjParamCmd(_imageService.Mirror);
            RotateForward = Make_ObjParamCmd(_imageService.RotateForward);
            RotateBackward = Make_ObjParamCmd(_imageService.RotateBackward);
            Pixelate = Make_ObjParamCmd(_imageService.Pixelate);

            // ---- File operations (service methods generally accept ImageView + param) ----
            Delete = Make_AsyncObjCmd(obj => FileService.DeleteAsync(owner));
            Move = Make_ObjParamCmd(FileService.Move);
            MoveAll = Make_ObjParamCmd(FileService.MoveAll);



            // Rename is asynchronous in your original; pass the owner in the lambda to call the service method
            Rename = Make_AsyncObjCmd(obj => FileService.RenameCurrentAsync(owner));

            ConvertCif = Make_ObjParamCmd(FileService.ConvertCif);

            // CleanTempFolder expects bool? (original: CleanTempAction(bool? obj) => _fileService.CleanTempFolder(obj))
            CleanTempFolder = Make_BoolParamCmd(FileService.CleanTempFolder);

            Save = Make_ObjParamCmd(FileService.Save);
        }
    }
}