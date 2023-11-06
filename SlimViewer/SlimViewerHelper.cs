/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViewer/SlimViewerHelper.cs
 * PURPOSE:     Helper Functions
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.IO;
using ExtendedSystemObjects;
using FileHandler;

namespace SlimViewer
{
    /// <summary>
    ///     Basic Helper functions
    /// </summary>
    internal static class SlimViewerHelper
    {
        /// <summary>
        ///     Unpacks the folder.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileNameWithoutExt">The file name without extension.</param>
        /// <returns>Target Folder</returns>
        internal static string UnpackFolder(string path, string fileNameWithoutExt)
        {
            //create Temp Folder
            var root = Path.Combine(Directory.GetCurrentDirectory(), SlimViewerResources.TempFolder);
            if (!Directory.Exists(root)) _ = Directory.CreateDirectory(root);

            root = Path.Combine(root, fileNameWithoutExt);
            if (!Directory.Exists(root))
            {
                //if the folder exists which should not happen, we clear it out
                _ = FileHandleDelete.DeleteAllContents(root, true);
                _ = Directory.CreateDirectory(root);
            }

            _ = FileHandleCompress.OpenZip(path, root, false);

            return root;
        }

        /// <summary>
        ///     Unpacks the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The first Image</returns>
        public static string UnpackFile(string path)
        {
            var lst = FileHandleSearch.GetFilesByExtensionFullPath(path, SlimViewerResources.Appendix, true);
            return lst.IsNullOrEmpty() ? null : lst[0];
        }
    }
}