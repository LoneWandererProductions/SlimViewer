/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews.Contexts
 * FILE:        FileContext.cs
 * PURPOSE:     File-related state and operations for <see cref="ImageView"/>; tracks file lists, current file, and related metadata.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using FileHandler;
using System.Collections.Generic;
using System.Linq;

namespace SlimViews.Contexts
{
    /// <summary>
    /// File context holds all file-related state and operations for the ImageView, including file lists, current file tracking, and related metadata.
    /// </summary>
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

        /// <summary>
        /// Gets or sets the current path.
        /// </summary>
        /// <value>
        /// The current path.
        /// </value>
        internal string CurrentPath { get; set; }

        /// <summary>
        /// Gets or sets the GIF path.
        /// </summary>
        /// <value>
        /// The GIF path.
        /// </value>
        internal string GifPath { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is files empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is files empty; otherwise, <c>false</c>.
        /// </value>
        internal bool IsFilesEmpty => Files.Count == 0;

        /// <summary>
        /// Gets the files sorted.
        /// </summary>
        /// <value>
        /// The files sorted.
        /// </value>
        internal List<string> FilesSorted => Files.PathSort();

        /// <summary>
        /// Determines whether [is key in observer] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if [is key in observer] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsKeyInObserver(int key) => Observer.ContainsKey(key);

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        internal string FilePath { get; set; }

        /// <summary>
        /// Currents the identifier get identifier by file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Id of Image</returns>
        internal int CurrentIdGetIdByFilePath(string filePath) => Observer.FirstOrDefault(x => x.Value == filePath).Key;

        /// <summary>
        /// Clears this instance.
        /// </summary>
        internal void Clear()
        {
            Count = 0;
            Observer = null;
            GifPath = null;
        }
    }
}