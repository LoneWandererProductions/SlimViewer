using FileHandler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SlimViews.Contexts
{
    internal sealed class FileContext
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        internal int Count { get; set; }

        /// <summary>
        /// Gets or sets the observer.
        /// </summary>
        /// <value>
        /// The observer.
        /// </value>
        public Dictionary<int, string> Observer { get; set; } = new();

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        internal string FileName { get; set; }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        internal List<string> Files { get; set; } =  [];

        /// <summary>
        /// Gets or sets the current identifier.
        /// </summary>
        /// <value>
        /// The current identifier.
        /// </value>
        internal int CurrentId { get; set; }

        internal string CurrentName { get; set; }

        internal string CurrentPath { get; set; }

        internal string GifPath { get; set; }

        internal bool IsFilesEmpty => Files.Count == 0;

        internal List<string> FilesSorted => Files.PathSort();

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        internal string FilePath { get; set; }


        internal void Clear()
        {
            Count = 0;
            GifPath = null;
            Observer = null;
        }
    }
}