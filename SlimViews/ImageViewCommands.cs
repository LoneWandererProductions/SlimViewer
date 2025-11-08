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

        public ImageViewCommands(ImageView owner)
        {
            // Generic helper that adapts automatically to all command types.
            bool CanRun<T>(T? arg) => owner.CanRun(arg);

            Close = new DelegateCommand<object>(owner.CloseAction, CanRun);
            OpenCbz = new DelegateCommand<object>(owner.OpenCbzAction, CanRun);
            Open = new DelegateCommand<object>(owner.OpenAction, CanRun);
            Save = new DelegateCommand<object>(owner.SaveAction, CanRun);
            Delete = new DelegateCommand<object>(owner.DeleteAction, CanRun);
            Refresh = new DelegateCommand<object>(owner.RefreshAction, CanRun);
            Pixelate = new DelegateCommand<object>(owner.PixelateAction, CanRun);
            Similar = new DelegateCommand<object>(owner.SimilarWindowAction, CanRun);
            Duplicate = new DelegateCommand<object>(owner.DuplicateWindowAction, CanRun);
            Rename = new AsyncDelegateCommand<object>(owner.RenameAction, CanRun);
            Folder = new DelegateCommand<object>(owner.FolderAction, CanRun);
            Mirror = new DelegateCommand<object>(owner.MirrorAction, CanRun);
            RotateForward = new DelegateCommand<object>(owner.RotateForwardAction, CanRun);
            RotateBackward = new DelegateCommand<object>(owner.RotateBackwardAction, CanRun);
            Explorer = new DelegateCommand<object>(owner.ExplorerAction, CanRun);
            Scale = new DelegateCommand<object>(owner.ScaleWindowAction, CanRun);
            FolderRename = new DelegateCommand<object>(owner.FolderRenameWindowAction, CanRun);
            FolderConvert = new DelegateCommand<object>(owner.FolderConvertWindowAction, CanRun);
            Clear = new DelegateCommand<object>(owner.ClearAction, CanRun);
            CleanTempFolder = new DelegateCommand<object>(owner.CleanTempAction, CanRun);
            FolderSearch = new DelegateCommand<object>(owner.SearchWindowAction, CanRun);
            Move = new DelegateCommand<object>(owner.MoveAction, CanRun);
            MoveAll = new DelegateCommand<object>(owner.MoveAllAction, CanRun);
            OpenCif = new DelegateCommand<object>(owner.OpenCifAction, CanRun);
            ConvertCif = new DelegateCommand<object>(owner.ConvertCifAction, CanRun);
            GifWindow = new DelegateCommand<object>(owner.GifWindowAction, CanRun);
            AnalyzerWindow = new DelegateCommand<object>(owner.AnalyzerWindowAction, CanRun);
            ExportString = new DelegateCommand<object>(owner.ExportStringAction, CanRun);
            ResizerWindow = new DelegateCommand<object>(owner.ResizerWindowAction, CanRun);

            ApplyFilter = new DelegateCommand<string>(owner.ApplyFilterAction, CanRun);
            ApplyTexture = new DelegateCommand<string>(owner.ApplyTextureAction, CanRun);
            FilterConfig = new DelegateCommand<string>(owner.FilterConfigWindowAction, CanRun);
            TextureConfig = new DelegateCommand<string>(owner.TextureConfigWindowAction, CanRun);
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
        }
    }
}