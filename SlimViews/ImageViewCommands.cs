#nullable enable
using CommonControls;
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
    public partial class ImageViewCommands
    {
        private readonly ImageView _owner;
        private readonly ImageMassProcessingCommands _imageMassService = new();
        private readonly ImageProcessingCommands _imageService = new();
        private readonly FileProcessingCommands _fileService = new();

        #region Commands

        public ICommand Close { get; }
        public ICommand OpenCbz { get; }
        public ICommand Open { get; }
        public ICommand Save { get; }
        public ICommand Delete { get; }
        public ICommand Refresh { get; }
        public ICommand Pixelate { get; }
        public ICommand Similar { get; }
        public ICommand Duplicate { get; }
        public ICommand Rename { get; }
        public ICommand Folder { get; }
        public ICommand Mirror { get; }
        public ICommand RotateForward { get; }
        public ICommand RotateBackward { get; }
        public ICommand Explorer { get; }
        public ICommand Scale { get; }
        public ICommand FolderRename { get; }
        public ICommand FolderConvert { get; }
        public ICommand Clear { get; }
        public ICommand CleanTempFolder { get; }
        public ICommand FolderSearch { get; }
        public ICommand Move { get; }
        public ICommand MoveAll { get; }
        public ICommand OpenCif { get; }
        public ICommand ConvertCif { get; }
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
        public ICommand ColorChanged { get; }
        public ICommand Next { get; }
        public ICommand Previous { get; }
        public ICommand ToolChanged { get; }

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
                new(_ => svcAction(owner), _ => CanRun(_));

            // Service that expects (ImageView, object?) -> use object command parameter
            DelegateCommand<object> Make_ObjParamCmd(Action<ImageView, object?> svcAction) =>
                new(p => svcAction(owner, p), p => CanRun(p));

            // Service that expects (ImageView, string?) -> use string-typed command
            DelegateCommand<string> Make_StringParamCmd(Action<ImageView, string?> svcAction) =>
                new(s => svcAction(owner, s), s => CanRun(s));

            // Service that expects (ImageView, SelectionFrame) etc. -- handled directly where needed
            // Bool? parameter commands (for CleanTempFolder)
            DelegateCommand<bool?> Make_BoolParamCmd(Action<bool?> action) =>
                new(action, b => CanRun(b));

            // Async commands which take object? parameter and return Task (e.g. Rename)
            AsyncDelegateCommand<object> Make_AsyncObjCmd(Func<object?, Task> asyncAction) =>
                new(asyncAction, p => CanRun(p));

            // ---- UI / direct owner commands ----
            Close = new DelegateCommand<object>(owner.CloseAction, CanRun);
            OpenCbz = new DelegateCommand<object>(owner.OpenCbzAction, CanRun);
            Open = new DelegateCommand<object>(owner.OpenAction, CanRun);
            Refresh = new DelegateCommand<object>(owner.RefreshAction, CanRun);
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
            ToolChanged = new DelegateCommand<ImageZoomTools>(owner.ToolChangedAction, CanRun);
            Explorer = new DelegateCommand<object>(owner.ExplorerAction, CanRun);
            ExportString = new DelegateCommand<object>(owner.ExportStringAction, CanRun);

            // ---- Image mass processing (service methods mostly take ImageView or ImageView+param) ----
            Scale = Make_NoParamCmd(_imageMassService.ScaleWindow);
            FolderConvert = Make_NoParamCmd(_imageMassService.FolderConvertWindow);
            FolderRename = Make_NoParamCmd(_imageMassService.FolderRenameWindow);
            Duplicate = Make_NoParamCmd(_imageMassService.DuplicateWindow);
            Similar = Make_NoParamCmd(_imageMassService.SimilarWindow);
            FolderSearch = Make_NoParamCmd(_imageMassService.FolderSearch);
            ResizerWindow = Make_NoParamCmd(_imageMassService.ResizerWindow);
            AnalyzerWindow = Make_NoParamCmd(_imageMassService.AnalyzerWindow);
            GifWindow = Make_NoParamCmd(_imageMassService.GifWindow);

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
            Delete = Make_ObjParamCmd(_fileService.Delete);
            Move = Make_ObjParamCmd(_fileService.Move);
            MoveAll = Make_ObjParamCmd(_fileService.MoveAll);

            // Rename is asynchronous in your original; pass the owner in the lambda to call the service method
            Rename = Make_AsyncObjCmd(obj => _fileService.Rename(owner, obj));

            ConvertCif = Make_ObjParamCmd(_fileService.ConvertCif);

            // CleanTempFolder expects bool? (original: CleanTempAction(bool? obj) => _fileService.CleanTempFolder(obj))
            CleanTempFolder = Make_BoolParamCmd(_fileService.CleanTempFolder);

            Save = Make_ObjParamCmd(_fileService.Save);
        }
    }
}
