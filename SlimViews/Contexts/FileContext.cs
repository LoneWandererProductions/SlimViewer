using FileHandler;
using System.Collections.Generic;

namespace SlimViews.Contexts
{
    internal sealed class FileContext
    {
        public Dictionary<int, string> Observer { get; set; }

        internal int Count { get; set;  }

        internal List<string> Files { get; set; } = [];

        internal int CurrentId { get; set; }

        internal string CurrentName { get; set; }

        internal string CurrentPath { get; set; }

        internal string GifPath { get; set; }

        internal bool IsFilesEmpty => Files.Count == 0;

        internal List<string> FilesSorted => Files.PathSort();
    }
}
