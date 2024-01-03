namespace SlimViewer
{
    public class Config
    {
        public bool GifCleanUp {get;set;}

        public string GifOutPutPath {get;set;}

        /// <summary>
        /// TODO implement flag
        /// Gets or sets a value indicating whether [main clean up].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [main clean up]; otherwise, <c>false</c>.
        /// </value>
        public bool MainCleanUp {get;set;}

        /// <summary>
        /// Gets or sets the main similarity.
        /// </summary>
        /// <value>
        /// The main similarity.
        /// </value>
        public int MainSimilarity {get;set;}

        public bool MainAutoPlayGif {get;set;}
    }
}