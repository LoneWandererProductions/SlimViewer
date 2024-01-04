/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViewer/SlimViewerRegister.cs
 * PURPOSE:     SlimViewer Register
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace SlimViewer
{
    internal static class SlimViewerRegister
    {
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

        internal static bool GifCleanUp { get; set; }

        internal static string GifOutPutPath { get; set; }

        internal static bool MainCleanUp { get; set; }

        internal static int MainSimilarity { get; set; }

        internal static bool MainAutoPlayGif { get; set; }

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

        internal static void SetRegister(Config obj)
        {
            GifCleanUp = obj.GifCleanUp;
            GifOutPutPath = obj.GifOutPutPath;
            MainAutoPlayGif = obj.MainAutoPlayGif;
            MainCleanUp = obj.MainCleanUp;
            MainSimilarity = obj.MainSimilarity;
        }

        internal static Config GetRegister()
        {
            return new()
            {
                GifCleanUp = GifCleanUp,
                GifOutPutPath = GifOutPutPath,
                MainAutoPlayGif = MainAutoPlayGif,
                MainCleanUp = MainCleanUp,
                MainSimilarity = MainSimilarity
            };
        }
    }
}