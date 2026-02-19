/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        Config.cs
 * PURPOSE:     Config object and creator
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal, can't be made internal, else the serializer will make some trouble.

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SlimViews
{
    /// <summary>
    ///     Config File
    /// </summary>
    public sealed class Config
    {
        /// <summary>
        ///     The path
        /// </summary>
        private static readonly string Path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), nameof(Config));

        /// <summary>
        ///     Gets or sets a value indicating whether [GIF clean up].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [GIF clean up]; otherwise, <c>false</c>.
        /// </value>
        public bool GifCleanUp { get; init; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether [main clean up].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [main clean up]; otherwise, <c>false</c>.
        /// </value>
        public bool MainAutoClean { get; init; } = true;

        /// <summary>
        ///     Gets or sets the main similarity.
        /// </summary>
        /// <value>
        ///     The main similarity.
        /// </value>
        public int MainSimilarity { get; init; } = 90;

        /// <summary>
        ///     Gets or sets a value indicating whether [main automatic play GIF].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [main automatic play GIF]; otherwise, <c>false</c>.
        /// </value>
        public bool MainAutoPlayGif { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [main sub folders].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [main sub folders]; otherwise, <c>false</c>.
        /// </value>
        public bool MainSubFolders { get; init; }

        /// <summary>
        ///     Gets or sets a value indicating whether [main compress cif].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [main compress cif]; otherwise, <c>false</c>.
        /// </value>
        public bool MainCompressCif { get; init; }

        /// <summary>
        ///     Sets the configuration.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal static void SetConfig(Config obj)
        {
            try
            {
                var serializer = new XmlSerializer(obj.GetType());

                using var tr = new StreamWriter(Path);
                serializer.Serialize(tr, obj);
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(string.Concat(ViewResources.ErrorSerializer, ex));
            }
            catch (XmlException ex)
            {
                Trace.WriteLine(string.Concat(ViewResources.ErrorSerializer, ex));
            }
            catch (NullReferenceException ex)
            {
                Trace.WriteLine(string.Concat(ViewResources.ErrorSerializer, ex));
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine(string.Concat(ViewResources.ErrorSerializer, ex));
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(string.Concat(ViewResources.ErrorSerializer, ex));
            }
            catch (IOException ex)
            {
                Trace.WriteLine(string.Concat(ViewResources.ErrorSerializer, ex));
            }
        }

        /// <summary>
        ///     Gets the configuration.
        /// </summary>
        /// <returns>Config File</returns>
        public static Config GetConfig()
        {
            if (!File.Exists(Path)) return new Config();

            try
            {
                var deserializer = new XmlSerializer(typeof(Config));
                using TextReader reader = new StreamReader(Path);
                //can return null but unlikely
                return deserializer.Deserialize(reader) as Config;
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(string.Concat(ViewResources.ErrorSerializer, ex));
            }
            catch (XmlException ex)
            {
                Trace.WriteLine(string.Concat(ViewResources.ErrorSerializer, ex));
            }
            catch (NullReferenceException ex)
            {
                Trace.WriteLine(string.Concat(ViewResources.ErrorSerializer, ex));
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine(string.Concat(ViewResources.ErrorSerializer, ex));
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(string.Concat(ViewResources.ErrorSerializer, ex));
            }
            catch (IOException ex)
            {
                Trace.WriteLine(string.Concat(ViewResources.ErrorSerializer, ex));
            }

            return null;
        }
    }
}