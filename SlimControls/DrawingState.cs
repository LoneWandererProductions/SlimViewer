/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        DrawingState.cs
 * PURPOSE:     Manages the state of the drawing tools and modes
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Imaging.Enums;

namespace SlimControls
{
    /// <summary>
    /// Manages the state of the drawing tools and modes. This class is responsible for tracking the active tool, area mode, selected shape, and related settings.
    /// It implements INotifyPropertyChanged to allow the UI to react to changes in the state.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class DrawingState : INotifyPropertyChanged
    {
        /// <summary>
        /// The active tool
        /// </summary>
        private DrawTool _activeTool;

        /// <summary>
        /// The active area mode
        /// </summary>
        private AreaMode _activeAreaMode;

        /// <summary>
        /// The selected shape
        /// Backing field for Shape
        /// </summary>
        private ShapeType _selectedShape;

        /// <summary>
        /// Occurs when [tool or mode changed].
        /// </summary>
        public event EventHandler ToolOrModeChanged;

        /// <summary>
        /// The brush size
        /// </summary>
        private double _brushSize = 5;

        /// <summary>
        /// The are area modes enabled
        /// </summary>
        private bool _isShapeToolActive;

        /// <summary>
        /// The brush color
        /// </summary>
        private string _brushColor = "#000000";

        /// <summary>
        /// The brush opacity
        /// </summary>
        private double _brushOpacity = 1.0;

        /// <summary>
        /// The is active layer raster
        /// </summary>
        private bool _isActiveLayerRaster = true;

        /// <summary>
        /// The shape filled
        /// </summary>
        private bool _shapeFilled = true;

        /// <summary>
        /// Gets or sets a value indicating whether the Shape tool is the active tool. Gates the visibility
        /// of both the shape-type picker (Rect/Ellipse/Freeform/Polygon) and the area-mode picker (Fill/
        /// Texture/Filter/Clear) in the toolbar - both only mean anything while drawing a shape. Named for
        /// what it means rather than "AreAreaModesEnabled": it now does double duty for the shape-type
        /// picker too, and a name describing only the older of its two uses would be actively misleading.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the active tool is <see cref="DrawTool.Shape" />; otherwise, <c>false</c>.
        /// </value>
        public bool IsShapeToolActive
        {
            get => _isShapeToolActive;
            set
            {
                _isShapeToolActive = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Called when [tool or mode changed].
        /// </summary>
        private void OnToolOrModeChanged()
            => ToolOrModeChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Gets or sets the active tool.
        /// </summary>
        /// <value>
        /// The active tool.
        /// </value>
        public DrawTool ActiveTool
        {
            get => _activeTool;
            set
            {
                if (_activeTool == value) return;

                _activeTool = value;
                OnPropertyChanged();

                // CRITICAL: Tool change forces a re-evaluation of what is allowed
                UpdateSubStates();
                OnToolOrModeChanged();
            }
        }


        /// <summary>
        /// Gets or sets the active area mode.
        /// </summary>
        /// <value>
        /// The active area mode.
        /// </value>
        public AreaMode ActiveAreaMode
        {
            get => _activeAreaMode;
            set
            {
                if (_activeAreaMode == value) return;

                _activeAreaMode = value;
                OnPropertyChanged();
                OnToolOrModeChanged();
            }
        }

        /// <summary>
        /// Gets or sets which shape a "🔺 Shape" tool stroke draws (Rectangle/Ellipse/Freeform/Polygon).
        /// Independent of <see cref="ActiveTool" /> - selecting a shape type no longer implicitly switches
        /// the active tool to <see cref="DrawTool.Shape" /> (it used to; see the type's own remarks in
        /// source control history for why that was a problem). The shape-type picker in the toolbar is only
        /// shown while <see cref="ActiveTool" /> is already <see cref="DrawTool.Shape" /> - gated by
        /// <see cref="IsShapeToolActive" />, the same flag that gates the area-mode picker - exactly
        /// mirroring how <see cref="ActiveAreaMode" /> is already independent of <see cref="ActiveTool" />.
        /// </summary>
        /// <value>
        /// The selected shape.
        /// </value>
        public ShapeType SelectedShape
        {
            get => _selectedShape;
            set
            {
                if (_selectedShape == value) return;

                _selectedShape = value;
                OnPropertyChanged();
                OnToolOrModeChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether a newly-drawn shape (Rect/Ellipse/Polygon) is filled. When <c>false</c>, the
        /// shape is stroke-only (outline) regardless of <see cref="ActiveAreaMode" />/<see cref="Fill" />/
        /// <see cref="Texture" />/<see cref="Filter" /> - those choose *what* fills a shape, this chooses
        /// *whether* it is filled at all, which is why it is a separate, independent flag rather than a
        /// fifth <see cref="AreaMode" /> value. Ignored by open shapes (line/freeform), which were never
        /// fillable in the first place. Defaults to <c>true</c> so existing behavior (every new shape is
        /// filled per the current area mode) is unchanged until the toolbar's "Fill" checkbox is unchecked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if new shapes are filled; <c>false</c> for outline-only.
        /// </value>
        public bool ShapeFilled
        {
            get => _shapeFilled;
            set
            {
                if (_shapeFilled == value) return;

                _shapeFilled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the size of the brush. Clamped to [1, 50] - the same range the toolbar's slider
        /// already enforces by construction; clamping here too means typing a value directly (see
        /// DrawingToolBarControl's numeric entry box) can't bypass it.
        /// </summary>
        /// <value>
        /// The size of the brush.
        /// </value>
        public double BrushSize
        {
            get => _brushSize;
            set
            {
                value = Math.Clamp(value, 1, 50);
                if (Math.Abs(_brushSize - value) < 0.001) return;

                _brushSize = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the color of the brush.
        /// </summary>
        /// <value>
        /// The color of the brush.
        /// </value>
        public string BrushColor
        {
            get => _brushColor;
            set
            {
                _brushColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the brush opacity. Clamped to [0.1, 1.0], matching the toolbar slider's own range -
        /// see <see cref="BrushSize" />'s remarks on why clamping happens here too, not just in the slider.
        /// </summary>
        /// <value>
        /// The brush opacity.
        /// </value>
        public double BrushOpacity
        {
            get => _brushOpacity;
            set
            {
                value = Math.Clamp(value, 0.1, 1.0);
                if (Math.Abs(_brushOpacity - value) < 0.01) return;

                _brushOpacity = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the currently active layer can take pixel edits (a raster
        /// layer, or no layered document at all - see remarks). Pencil, Eraser and the "Clear" area mode all
        /// end up calling <c>LayerDocumentController.ApplyToActiveRasterLayer</c>/<c>DrawStrokeAsync</c>,
        /// which are no-ops or throw when the active layer is a shape layer; the toolbar binds this to those
        /// tools' <c>IsEnabled</c> so that mismatch is visible and unclickable instead of silently doing
        /// nothing (Pencil/Eraser) or being caught and swallowed (Clear) further down.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the active layer accepts pixel edits, or there is no layered document (single-
        ///   bitmap editing, where this restriction does not apply); otherwise <c>false</c>.
        /// </value>
        public bool IsActiveLayerRaster
        {
            get => _isActiveLayerRaster;
            set
            {
                if (_isActiveLayerRaster == value) return;

                _isActiveLayerRaster = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the collection of available textures for the UI dropdown.
        /// </summary>
        /// <value>
        /// The textures.
        /// </value>
        public IEnumerable<TextureType> Textures => Enum.GetValues(typeof(TextureType)).Cast<TextureType>();

        /// <summary>
        /// Gets the collection of available filters for the UI dropdown.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public IEnumerable<FiltersType> Filters => Enum.GetValues(typeof(FiltersType)).Cast<FiltersType>();

        // Sub-states (These hold the data for the specific modes)

        /// <summary>
        /// Gets the fill.
        /// </summary>
        /// <value>
        /// The fill.
        /// </value>
        public FillSettings Fill { get; }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public TextureSettings Texture { get; }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public FilterSettings Filter { get; }

        /// <summary>
        /// Gets the erase.
        /// </summary>
        /// <value>
        /// The erase.
        /// </value>
        public EraseSettings Erase { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingState" /> class. Constructs the four area-mode
        /// settings objects with a back-reference to this instance, so each can compute its own
        /// <see cref="AreaModeSettings.Enabled" /> from <see cref="ActiveAreaMode" /> directly (see
        /// AreaModeSettings' own remarks) instead of <see cref="UpdateSubStates" /> having to set all four
        /// by hand.
        /// </summary>
        public DrawingState()
        {
            Fill = new FillSettings(this);
            Texture = new TextureSettings(this);
            Filter = new FilterSettings(this);
            Erase = new EraseSettings(this);
        }

        /// <summary>
        /// Re-derives <see cref="IsShapeToolActive" /> from <see cref="ActiveTool" />. Called whenever the
        /// active tool changes; the four area-mode settings objects re-derive their own
        /// <see cref="AreaModeSettings.Enabled" /> independently (see that class), so there is nothing else
        /// left for this method to do.
        /// </summary>
        private void UpdateSubStates()
        {
            IsShapeToolActive = ActiveTool == DrawTool.Shape;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}