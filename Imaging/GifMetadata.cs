using System.Collections.Generic;
using System.Linq;

namespace Imaging
{
    public class GifMetadata
    {
        public string Header { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool HasGlobalColorTable { get; set; }
        public int GlobalColorTableSize { get; set; }
        public int ColorResolution { get; set; }
        public int BackgroundColorIndex { get; set; }
        public int PixelAspectRatio { get; set; }
        public int? LoopCount { get; set; } // Nullable, since not all GIFs have this
        public List<FrameInfo> Frames { get; set; } = new List<FrameInfo>();
        public double TotalDuration => Frames.Sum(f => f.DelayTime); // In seconds
    }

    public class FrameInfo
    {
        public string Description { get; set; }
        public double DelayTime { get; set; } // Delay time in seconds
    }
}