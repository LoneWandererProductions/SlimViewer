#nullable enable
using CommonControls;
using Imaging;
using Point = System.Windows.Point;
using ViewModel;
using System.Windows.Input;

namespace SlimViews
{
    public class ImageViewCommands
    {
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

        private readonly ImageMassProcessingCommands _imageMassService = new();

        /// <summary>
        /// The owner
        /// </summary>
        private readonly ImageView _owner;

        /// <summary>
        /// Scales the window action.
        /// </summary>
        /// <param name="_">The empty parameter</param>
        public void ScaleWindowAction(object? _) => _imageMassService.ScaleWindow(_owner);

        /// <summary>
        /// Folders the convert window action.
        /// </summary>
        /// <param name="_">The empty parameter</param>
        public void FolderConvertWindowAction(object? _) => _imageMassService.FolderConvertWindow(_owner);


        /// <summary>
        /// Folders the rename window action.
        /// </summary>
        /// <param name="_">The empty parameter</param>
        public void FolderRenameWindowAction(object? _) => _imageMassService.FolderRenameWindow(_owner);


        /// <summary>
        /// Duplicates the window action.
        /// </summary>
        /// <param name="_">The empty parameter</param>
        public void DuplicateWindowAction(object? _) => _imageMassService.DuplicateWindow(_owner);

        /// <summary>
        /// Similars the window action.
        /// </summary>
        /// <param name="_">The empty parameter</param>
        public void SimilarWindowAction(object? _) => _imageMassService.SimilarWindow(_owner);

        /// <summary>
        /// Folders the search action.
        /// </summary>
        /// <param name="_">The empty parameter</param>
        public void FolderSearchAction(object? _) => _imageMassService.FolderSearch(_owner);

        /// <summary>
        /// Resizers the window action.
        /// </summary>
        /// <param name="_">The empty parameter</param>
        public void ResizerWindowAction(object? _) => _imageMassService.ResizerWindow(_owner);

        /// <summary>
        /// Analyzers the window action.
        /// </summary>
        /// <param name="_">The empty parameter</param>
        public void AnalyzerWindowAction(object? _) => _imageMassService.AnalyzerWindow(_owner);

        /// <summary>
        /// GIFs the window action.
        /// </summary>
        /// <param name="_">The .</param>
        public void GifWindowAction(object? _) => _imageMassService.GifWindow(_owner);

        /// <summary>
        /// Filters the configuration window action.
        /// </summary>
        /// <param name="obj">The object.</param>
        public void FilterConfigWindowAction(string? obj) => _imageMassService.FilterConfigWindow(_owner, obj);

        /// <summary>
        /// Textures the configuration window action.
        /// </summary>
        /// <param name="obj">The object.</param>
        public void TextureConfigWindowAction(string? obj) => _imageMassService.TextureConfigWindow(_owner, obj);

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageViewCommands"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public ImageViewCommands(ImageView owner)
        {
            _owner = owner;

            // Generic helper that adapts automatically to all command types.
            bool CanRun<T>(T? arg) => owner.CanRun(arg);

            Close = new DelegateCommand<object>(owner.CloseAction, CanRun);
            OpenCbz = new DelegateCommand<object>(owner.OpenCbzAction, CanRun);
            Open = new DelegateCommand<object>(owner.OpenAction, CanRun);
            Save = new DelegateCommand<object>(owner.SaveAction, CanRun);
            Delete = new DelegateCommand<object>(owner.DeleteAction, CanRun);
            Refresh = new DelegateCommand<object>(owner.RefreshAction, CanRun);
            Pixelate = new DelegateCommand<object>(owner.PixelateAction, CanRun);
            Rename = new AsyncDelegateCommand<object>(owner.RenameAction, CanRun);

            Folder = new DelegateCommand<object>(owner.FolderAction, CanRun);
            Mirror = new DelegateCommand<object>(owner.MirrorAction, CanRun);
            RotateForward = new DelegateCommand<object>(owner.RotateForwardAction, CanRun);
            RotateBackward = new DelegateCommand<object>(owner.RotateBackwardAction, CanRun);
            Explorer = new DelegateCommand<object>(owner.ExplorerAction, CanRun);

            Clear = new DelegateCommand<object>(owner.ClearAction, CanRun);
            CleanTempFolder = new DelegateCommand<object>(owner.CleanTempAction, CanRun);
            Move = new DelegateCommand<object>(owner.MoveAction, CanRun);
            MoveAll = new DelegateCommand<object>(owner.MoveAllAction, CanRun);
            OpenCif = new DelegateCommand<object>(owner.OpenCifAction, CanRun);
            ConvertCif = new DelegateCommand<object>(owner.ConvertCifAction, CanRun);

            ExportString = new DelegateCommand<object>(owner.ExportStringAction, CanRun);


            ApplyFilter = new DelegateCommand<string>(owner.ApplyFilterAction, CanRun);
            ApplyTexture = new DelegateCommand<string>(owner.ApplyTextureAction, CanRun);

            Brighten = new DelegateCommand<string>(owner.BrightenAction, CanRun);
            Darken = new DelegateCommand<string>(owner.DarkenAction, CanRun);

            ThumbImageClicked = new DelegateCommand<ImageEventArgs>(owner.ThumbImageClickedAction, CanRun);
            ImageLoaded = new DelegateCommand<object>(owner.ImageLoadedCommandAction, CanRun);
            SelectedPoint = new DelegateCommand<Point>(owner.SelectedPointAction, CanRun);
            SelectedFrame = new DelegateCommand<SelectionFrame>(owner.SelectedFrameAction, CanRun);
            ColorChanged = new DelegateCommand<ColorHsv>(owner.ColorChangedAction, CanRun);
            Next = new DelegateCommand<object>(owner.NextAction, CanRun);
            Previous = new DelegateCommand<object>(owner.PreviousAction, CanRun);
            ToolChanged = new DelegateCommand<ImageZoomTools>(owner.ToolChangedAction, CanRun);


            // Image mass processing commands with sub Windows

            Scale = new DelegateCommand<object>(ScaleWindowAction, CanRun);
            FolderConvert = new DelegateCommand<object>(FolderConvertWindowAction, CanRun);
            FolderRename = new DelegateCommand<object>(FolderRenameWindowAction, CanRun);
            Duplicate = new DelegateCommand<object>(DuplicateWindowAction, CanRun);
            FilterConfig = new DelegateCommand<string>(FilterConfigWindowAction, CanRun);
            TextureConfig = new DelegateCommand<string>(TextureConfigWindowAction, CanRun);
            FolderSearch = new DelegateCommand<object>(FolderSearchAction, CanRun);
            ResizerWindow = new DelegateCommand<object>(ResizerWindowAction, CanRun);
            AnalyzerWindow = new DelegateCommand<object>(AnalyzerWindowAction, CanRun);
            GifWindow = new DelegateCommand<object>(GifWindowAction, CanRun);
            Similar = new DelegateCommand<object>(SimilarWindowAction, CanRun);
        }
    }
}