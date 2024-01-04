using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SlimViewer
{
    public class Config
    {
        private readonly string _path = Path.Combine(Directory.GetCurrentDirectory(), nameof(Config));

        public bool GifCleanUp { get; set; } = true;

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

        public bool MainAutoPlayGif { get; set; } = false;

        internal void SetConfig()
        {
            try
            {
                var serializer = new XmlSerializer(GetType());

                using var tr = new StreamWriter(_path);
                serializer.Serialize(tr, this);
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

        internal Config GetConfig()
        {
            if (!File.Exists(_path)) return new Config();

            try
            {
                var deserializer = new XmlSerializer(typeof(Config));
                using TextReader reader = new StreamReader(_path);
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