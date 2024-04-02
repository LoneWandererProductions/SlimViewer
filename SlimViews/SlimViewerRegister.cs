/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/SlimViewerRegister.cs
 * PURPOSE:     SlimViewer Register
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace SlimViews
{
    /// <summary>
    /// Register for the generic and commonly used Variables
    /// </summary>
    public static class SlimViewerRegister
    {
        /// <summary>
        /// Gets or sets the current folder.
        /// </summary>
        /// <value>
        /// The current folder.
        /// </value>
        internal static string CurrentFolder { get; set; }

        /// <summary>
        ///     Gets or sets the scaling.
        /// </summary>
        /// <value>
        ///     The scaling.
        /// </value>
        internal static float Scaling { get; set; }

        /// <summary>
        ///     Gets or sets the degree of the Image.
        /// </summary>
        /// <value>
        ///     The degree.
        /// </value>
        internal static int Degree { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Rename" /> is changed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if changed; otherwise, <c>false</c>.
        /// </value>
        internal static bool Changed { get; set; }

        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        /// <value>
        ///     The source.
        /// </value>
        internal static string Source { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        /// <value>
        ///     The target.
        /// </value>
        internal static string Target { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [GIF clean up].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [GIF clean up]; otherwise, <c>false</c>.
        /// </value>
        internal static bool GifCleanUp { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [main clean up].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [main clean up]; otherwise, <c>false</c>.
        /// </value>
        internal static bool MainAutoClean { get; set; }

        /// <summary>
        ///     Gets or sets the main similarity.
        /// </summary>
        /// <value>
        ///     The main similarity.
        /// </value>
        internal static int MainSimilarity { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [main automatic play GIF].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [main automatic play GIF]; otherwise, <c>false</c>.
        /// </value>
        private static bool MainAutoPlayGif { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [main sub folders].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [main sub folders]; otherwise, <c>false</c>.
        /// </value>
        internal static bool MainSubFolders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [main compress cif].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [main compress cif]; otherwise, <c>false</c>.
        /// </value>
        internal static bool MainCompressCif { get; set; }

        /// <summary>
        ///     Resets the scaling.
        /// </summary>
        internal static void ResetScaling()
        {
            Scaling = 1;
            Degree = 0;
        }

        /// <summary>
        ///     Resets the renaming.
        /// </summary>
        internal static void ResetRenaming()
        {
            Changed = false;
        }

        /// <summary>
        ///     Converts this instance.
        /// </summary>
        internal static void ResetConvert()
        {
            Source = string.Empty;
            Target = string.Empty;
        }

        /// <summary>
        ///     Sets the register.
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void SetRegister(Config obj)
        {
            GifCleanUp = obj.GifCleanUp;
            MainAutoPlayGif = obj.MainAutoPlayGif;
            MainAutoClean = obj.MainAutoClean;
            MainSimilarity = obj.MainSimilarity;
            MainSubFolders = obj.MainSubFolders;
            MainCompressCif = MainCompressCif;
        }

        /// <summary>
        ///     Gets the register.
        /// </summary>
        /// <returns>The config object</returns>
        internal static Config GetRegister()
        {
            return new Config
            {
                GifCleanUp = GifCleanUp,
                MainAutoPlayGif = MainAutoPlayGif,
                MainAutoClean = MainAutoClean,
                MainSimilarity = MainSimilarity,
                MainSubFolders = MainSubFolders,
                MainCompressCif = MainCompressCif
            };
        }
    }
}