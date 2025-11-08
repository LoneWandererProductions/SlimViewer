using FileHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static System.Net.WebRequestMethods;

namespace SlimViews.Contexts
{
    internal sealed class FileContext
    {
        /// <summary>
        ///     Gets or sets the root.
        /// </summary>
        /// <value>
        ///     The root.
        /// </value>
        public string Root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        ///     Gets or sets the status image.
        /// </summary>
        /// <value>
        ///     The status image.
        /// </value>
        public string StatusImage { get; set; }

        /// <summary>
        /// Gets the green icon path.
        /// </summary>
        /// <value>
        /// The green icon path.
        /// </value>
        public string GreenIconPath => Path.Combine(Root, ViewResources.IconPathGreen);

        /// <summary>
        /// Gets the red icon path.
        /// </summary>
        /// <value>
        /// The red icon path.
        /// </value>
        public string RedIconPath => Path.Combine(Root, ViewResources.IconPathRed);

        public Dictionary<int, string> Observer { get; set; }

        internal int Count { get; set; }

        internal List<string> Files { get; set; } =  [];

        internal int CurrentId { get; set; }

        internal string CurrentName { get; set; }

        internal string CurrentPath { get; set; }

        internal string GifPath { get; set; }

        internal bool IsFilesEmpty => Files.Count == 0;

        internal List<string> FilesSorted => Files.PathSort();

        internal void Clear()
        {
            Count = 0;
            GifPath = null;
            Observer = null;
        }
    }
}