using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SlimViewer
{
    public sealed class Config
    {
        /// <summary>
        /// The path
        /// </summary>
        private static readonly string Path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), nameof(Config));

        /// <summary>
        /// Gets or sets a value indicating whether [GIF clean up].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [GIF clean up]; otherwise, <c>false</c>.
        /// </value>
        public bool GifCleanUp { get; set; } = true;

        /// <summary>
        /// Gets or sets the GIF out put path.
        /// </summary>
        /// <value>
        /// The GIF out put path.
        /// </value>
        public string GifOutPutPath { get; set; }

        /// <summary>
        ///     TODO implement flag
        ///     Gets or sets a value indicating whether [main clean up].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [main clean up]; otherwise, <c>false</c>.
        /// </value>
        public bool MainCleanUp { get; set; } = true;

        /// <summary>
        ///     Gets or sets the main similarity.
        /// </summary>
        /// <value>
        ///     The main similarity.
        /// </value>
        public int MainSimilarity { get; set; } = 90;

        public bool MainAutoPlayGif { get; set; }

        /// <summary>
        /// Sets the configuration.
        /// </summary>
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
                Trace.WriteLine(string.Concat(SlimViewerResources.ErrorSerializer, ex));
            }
            catch (XmlException ex)
            {
                Trace.WriteLine(string.Concat(SlimViewerResources.ErrorSerializer, ex));
            }
            catch (NullReferenceException ex)
            {
                Trace.WriteLine(string.Concat(SlimViewerResources.ErrorSerializer, ex));
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine(string.Concat(SlimViewerResources.ErrorSerializer, ex));
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(string.Concat(SlimViewerResources.ErrorSerializer, ex));
            }
            catch (IOException ex)
            {
                Trace.WriteLine(string.Concat(SlimViewerResources.ErrorSerializer, ex));
            }
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <returns>Config File</returns>
        internal static Config GetConfig()
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
                Trace.WriteLine(string.Concat(SlimViewerResources.ErrorSerializer, ex));
            }
            catch (XmlException ex)
            {
                Trace.WriteLine(string.Concat(SlimViewerResources.ErrorSerializer, ex));
            }
            catch (NullReferenceException ex)
            {
                Trace.WriteLine(string.Concat(SlimViewerResources.ErrorSerializer, ex));
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine(string.Concat(SlimViewerResources.ErrorSerializer, ex));
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(string.Concat(SlimViewerResources.ErrorSerializer, ex));
            }
            catch (IOException ex)
            {
                Trace.WriteLine(string.Concat(SlimViewerResources.ErrorSerializer, ex));
            }

            return null;
        }
    }
}