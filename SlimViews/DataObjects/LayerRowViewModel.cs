/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews.DataObjects
 * FILE:        LayerRowViewModel.cs
 * PURPOSE:     Bindable, per-row wrapper around a single Imaging.Objects.Documents.Layer, for use by
 *              LayersPanel. Layer/Document are deliberately plain data (see their own remarks) with no
 *              INotifyPropertyChanged of their own, so this is the thin adapter that makes one row of the
 *              layer stack bindable: every setter here goes through LayerDocumentController.SetLayerProperties
 *              or SetActiveLayer (never mutates the Layer directly), so every edit stays undoable and the
 *              panel never needs its own change-tracking.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using Imaging;
using Imaging.Objects.Documents;
using SlimViews;
using ViewModel;

namespace SlimViews.DataObjects
{
    /// <summary>
    ///     One row of a <see cref="LayersPanel" />'s layer list. Owns no state of its own beyond a cached
    ///     thumbnail - <see cref="LayersPanel" /> is responsible for calling <see cref="RefreshFromLayer" />/
    ///     <see cref="RefreshThumbnail" />/<see cref="RefreshIsActive" /> in response to the controller's
    ///     <c>DocumentChanged</c>/<c>ActiveLayerChanged</c> events, and for rebuilding the row list entirely
    ///     on structural changes (a layer added/removed/reordered replaces the Layer instance's position, not
    ///     its identity, so existing rows for untouched layers do not need to be recreated).
    /// </summary>
    public sealed class LayerRowViewModel : ViewModelBase
    {
        /// <summary>Thumbnail edge length in device-independent pixels. Cheap to regenerate; not cached to disk.</summary>
        private const int ThumbnailSize = 48;

        private readonly LayerDocumentController _controller;

        private readonly Layer _layer;

        /// <summary>Initializes a new instance wrapping <paramref name="layer" />; renders the initial thumbnail.</summary>
        public LayerRowViewModel(LayerDocumentController controller, Layer layer)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _layer = layer ?? throw new ArgumentNullException(nameof(layer));
            RefreshThumbnail();
        }

        /// <summary>Gets the stable identity of the wrapped layer.</summary>
        public Guid Id => _layer.Id;

        /// <summary>Gets a short, human-readable label for the layer's kind (used for an icon/tooltip).</summary>
        public string Kind => _layer is RasterLayer ? "🖼️ Raster" : "🔺 Shapes";

        /// <summary>Gets a value indicating whether the wrapped layer can take pencil/eraser strokes.</summary>
        public bool IsRaster => _layer is RasterLayer;

        /// <summary>Gets or sets the display name. Setting commits an undoable <see cref="SetLayerPropertiesCommand" />.</summary>
        public string Name
        {
            get => _layer.Name;
            set
            {
                if (_layer.Name == value) return;
                _controller.SetLayerProperties(Id, _layer.Properties with { Name = value });
            }
        }

        /// <summary>Gets or sets whether the layer takes part in rendering.</summary>
        public bool IsVisible
        {
            get => _layer.Visible;
            set
            {
                if (_layer.Visible == value) return;
                _controller.SetLayerProperties(Id, _layer.Properties with { Visible = value });
            }
        }

        /// <summary>
        ///     Gets or sets the opacity (0..1). The panel's slider commits this on drag-completed, not on every
        ///     tick - see <see cref="LayerDocumentController.SetLayerProperties" />'s own remarks on why a slider
        ///     should execute one command per drag, not one per mouse move.
        /// </summary>
        public double Opacity
        {
            get => _layer.Opacity;
            set
            {
                if (Math.Abs(_layer.Opacity - value) < 0.001) return;
                _controller.SetLayerProperties(Id, _layer.Properties with { Opacity = value });
            }
        }

        /// <summary>
        ///     Gets or sets the blend mode. Only <see cref="BlendMode.Normal" /> exists today - the picker is
        ///     wired up so nothing further needs touching in this file once more modes land.
        /// </summary>
        public BlendMode Blend
        {
            get => _layer.Blend;
            set
            {
                if (_layer.Blend == value) return;
                _controller.SetLayerProperties(Id, _layer.Properties with { Blend = value });
            }
        }

        /// <summary>
        ///     Gets or sets whether this is the layer new strokes/shapes go to. Setting <c>true</c> calls
        ///     <see cref="LayerDocumentController.SetActiveLayer" />; setting <c>false</c> directly is a no-op -
        ///     a layer stack always has exactly one active layer (or none), so "deactivate" only happens by
        ///     activating something else.
        /// </summary>
        public bool IsActive
        {
            get => _controller.ActiveLayerId == Id;
            set
            {
                if (value) _controller.SetActiveLayer(Id);
            }
        }

        /// <summary>Gets the small preview image shown in the row, or null while none is available (see remarks).</summary>
        public BitmapSource? Thumbnail { get; private set; }

        /// <summary>
        ///     Re-raises PropertyChanged for every bindable property and refreshes the thumbnail. Call after a
        ///     <see cref="DocumentChangeKind.LayerPropertiesChanged" /> for this row's layer id.
        /// </summary>
        public void RefreshFromLayer()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(IsVisible));
            OnPropertyChanged(nameof(Opacity));
            OnPropertyChanged(nameof(Blend));
            RefreshThumbnail();
        }

        /// <summary>Re-raises PropertyChanged for <see cref="IsActive" />. Call from <c>ActiveLayerChanged</c>.</summary>
        public void RefreshIsActive() => OnPropertyChanged(nameof(IsActive));

        /// <summary>
        ///     Regenerates <see cref="Thumbnail" /> from the layer's current pixels. Call after a
        ///     <see cref="DocumentChangeKind.PixelsChanged" />/<see cref="DocumentChangeKind.ShapesChanged" />
        ///     for this row's layer id (or after any properties change, since <see cref="RefreshFromLayer" />
        ///     calls it too - opacity/visibility do not currently bake into the thumbnail itself, only the
        ///     flattened canvas, but that is a reasonable place to extend if a dimmed/hidden look is wanted).
        /// </summary>
        /// <remarks>
        ///     Runs synchronously on the calling thread (expected to be the UI thread, via
        ///     <c>Dispatcher.BeginInvoke</c> in <see cref="LayersPanel" />). For a raster layer this makes one
        ///     full-size copy of its pixels plus one scaled-down copy - fine at interactive rates for ordinary
        ///     image sizes, but a candidate to move to a background thread/debounce if used on very large
        ///     documents or during fast, continuous strokes. Shape layers have no single-layer rasterizer yet
        ///     (<see cref="DocumentRenderer" /> only flattens the whole document), so they show no preview for
        ///     now - see the class remarks on <see cref="Kind" /> for the icon fallback used instead.
        /// </remarks>
        public void RefreshThumbnail()
        {
            try
            {
                if (_layer is RasterLayer { IsDisposed: false } raster)
                {
                    using var full = raster.Pixels.ToBitmap();
                    using var small = new Bitmap(full, new Size(ThumbnailSize, ThumbnailSize));
                    Thumbnail = small.ToBitmapSource();
                }
                else
                {
                    Thumbnail = null;
                }
            }
            catch (Exception)
            {
                // Best-effort preview only; a failed thumbnail must never take the whole panel down.
                Thumbnail = null;
            }

            OnPropertyChanged(nameof(Thumbnail));
        }
    }
}
