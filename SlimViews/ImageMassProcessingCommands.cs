/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        ImageMassProcessingCommands.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using SlimControls;
using SlimViews.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SlimViews
{
    /// <summary>
    /// Hnadler for alle sub Windows related to Mass Image Processing
    /// </summary>
    internal class ImageMassProcessingCommands
    {
        /// <summary>
        /// Keep track of all currently open subwindows
        /// </summary>
        private readonly List<Window> _subWindows = new();

        /// <summary>
        /// Folders the convert window.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal void FolderConvertWindow(ImageView owner)
        {
            SlimViewerRegister.ResetConvert();

            var converterWindow = InitDialog<Converter>(owner, modal: true);

            if (!SlimViewerRegister.IsPathsSet)
            {
                ImageProcessor.FolderConvert(
                    SlimViewerRegister.Target,
                    SlimViewerRegister.Source,
                    owner.FileContext.Observer);
            }
        }

        /// <summary>
        /// Scales the window.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal void ScaleWindow(ImageView owner)
        {
            SlimViewerRegister.ResetScaling();
            var scaleWindow = InitDialog<Scale>(owner, modal: true);

            owner.Image.Bitmap = ImageProcessor.BitmapScaling(owner.Image.Bitmap, SlimViewerRegister.Scaling);
            owner.Image.Bitmap = ImageProcessor.RotateImage(owner.Image.Bitmap, SlimViewerRegister.Degree);
            owner.Image.Bitmap = ImageProcessor.CropImage(owner.Image.Bitmap);
            owner.Bmp = owner.Image.BitmapSource;
        }

        /// <summary>
        ///     Rename the Folder action.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal void FolderRenameWindow(ImageView owner)
        {
            SlimViewerRegister.ResetRenaming();

            var dct = new Dictionary<int, string>();

            if (owner.UiState.Thumb.IsSelectionValid)
            {
                foreach (var id in owner.UiState.Thumb.Selection.Keys.Where(id =>
                             owner.FileContext.IsKeyInObserver(id)))
                    dct.Add(id, owner.FileContext.Observer[id]);
            }
            else
            {
                dct = new Dictionary<int, string>(owner.FileContext.Observer);
            }


            var rename = new Rename(dct)
            {
                Topmost = true,
                Owner = owner.UiState.Main
            };
            _ = rename.ShowDialog();

            //refresh the Filename, no need to refresh all, we don't need to reload everything, to save time
            if (SlimViewerRegister.Changed && owner.UiState.Thumb.IsSelectionValid)
                owner.FileContext.Observer = rename.Observer;
            else
                foreach (var (key, value) in rename.Observer)
                    owner.FileContext.Observer[key] = value;
        }

        /// <summary>
        /// Duplicates the window.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal void DuplicateWindow(ImageView owner)
        {
            var compareWindow = new Compare(owner.UseSubFolders, owner.FileContext.CurrentPath, owner)
            {
                Topmost = true,
                Owner = owner.UiState.Main
            };

            compareWindow.Show();

            SlimViewerRegister.CompareView = true;
        }

        /// <summary>
        /// Filters the configuration window.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="obj">The object.</param>
        internal void FilterConfigWindow(ImageView owner, string obj)
        {
            var filterConfig = new FilterConfig
            {
                Topmost = true,
                Owner = owner.UiState.Main
            };

            if (!string.IsNullOrEmpty(obj))
            {
                var filter = Translator.GetFilterFromString(obj);

                // Reassign the TextureConfig to initialize with texture if needed
                filterConfig = new FilterConfig(filter)
                {
                    Topmost = true,
                    Owner = owner.UiState.Main
                };
            }

            filterConfig.Show();
        }

        /// <summary>
        /// Textures the configuration window.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="obj">The object.</param>
        internal void TextureConfigWindow(ImageView owner, string obj)
        {
            var textureConfig = new TextureConfig
            {
                Topmost = true,
                Owner = owner.UiState.Main
            };

            if (!string.IsNullOrEmpty(obj))
            {
                var texture = Translator.GetTextureFromString(obj);

                // Reassign the TextureConfig to initialize with texture if needed
                textureConfig = new TextureConfig(texture)
                {
                    Topmost = true,
                    Owner = owner.UiState.Main
                };
            }

            textureConfig.Show();
        }

        /// <summary>
        /// Resizers the window.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal void ResizerWindow(ImageView owner)
        {
            var resizer = new Resizer(owner.FileContext.CurrentPath)
            {
                Topmost = true,
                Owner = owner.UiState.Main
            };
            resizer.Show();
        }

        /// <summary>
        /// Similars the window.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal void SimilarWindow(ImageView owner)
        {
            var compareWindow =
                new Compare(owner.UseSubFolders, owner.FileContext.CurrentPath, owner, owner.Similarity)
                {
                    Topmost = true,
                    Owner = owner.UiState.Main
                };
            compareWindow.Show();

            SlimViewerRegister.CompareView = true;
        }

        /// <summary>
        /// Folders the search.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal void FolderSearch(ImageView owner) => InitDialog<Search>(owner);

        /// <summary>
        /// Analyzers the window.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal void AnalyzerWindow(ImageView owner) =>
            InitDialog<DetailCompare>(owner);

        /// <summary>
        /// GIFs the window.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal void GifWindow(ImageView owner) => InitDialog<Gif>(owner);

        /// <summary>
        /// Initializes the dialog.
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <param name="owner">The owner.</param>
        /// <param name="modal">if set to <c>true</c> [modal].</param>
        /// <returns>Reference to Window</returns>
        private T InitDialog<T>(ImageView owner, bool modal = false) where T : Window, new()
        {
            var window = new T
            {
                Owner = owner.UiState.Main,
                Topmost = true
            };

            // Register in subwindow list
            _subWindows.Add(window);
            window.Closed += (_, __) => _subWindows.Remove(window);

            // Attach callback so the subwindow asks THIS class to close it
            if (window is IClosableByCommand closable)
            {
                closable.RequestCloseAction = () =>
                {
                    // central close
                    if (window.IsLoaded)
                        window.Close();
                };
            }

            if (modal)
                window.ShowDialog();
            else
                window.Show();

            return window;
        }

        /// <summary>
        /// Close all registered subwindows safely.
        /// </summary>
        internal void CloseAllSubWindows()
        {
            foreach (var w in _subWindows.ToList())
            {
                if (w.IsLoaded) w.Close();
            }
            _subWindows.Clear();
        }
    }
}