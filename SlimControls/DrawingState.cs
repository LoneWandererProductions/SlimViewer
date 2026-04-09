/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        DrawingState.cs
 * PURPOSE:     Manages the state of the drawing tools and modes
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        private bool _areAreaModesEnabled;


        /// <summary>
        /// The brush color
        /// </summary>
        private string _brushColor = "#000000";

        /// <summary>
        /// The brush opacity
        /// </summary>
        private double _brushOpacity = 1.0;

        /// <summary>
        /// Gets or sets a value indicating whether [are area modes enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [are area modes enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool AreAreaModesEnabled
        {
            get => _areAreaModesEnabled;
            set
            {
                _areAreaModesEnabled = value;
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
                UpdateSubStates();
                OnPropertyChanged();
                OnToolOrModeChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected shape.
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

                // FIX: When user picks a shape, automatically switch the Active Tool
                ActiveTool = DrawTool.Shape;

                OnPropertyChanged();
                OnToolOrModeChanged();
            }
        }

        /// <summary>
        /// Gets or sets the size of the brush.
        /// </summary>
        /// <value>
        /// The size of the brush.
        /// </value>
        public double BrushSize
        {
            get => _brushSize;
            set
            {
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
        /// Gets or sets the brush opacity.
        /// </summary>
        /// <value>
        /// The brush opacity.
        /// </value>
        public double BrushOpacity
        {
            get => _brushOpacity;
            set
            {
                if (Math.Abs(_brushOpacity - value) < 0.01) return;
                _brushOpacity = value;
                OnPropertyChanged();
            }
        }

        // Sub-states (These hold the data for the specific modes)
        /// <summary>
        /// Gets the fill.
        /// </summary>
        /// <value>
        /// The fill.
        /// </value>
        public FillSettings Fill { get; } = new();

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public TextureSettings Texture { get; } = new();

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public FilterSettings Filter { get; } = new();

        /// <summary>
        /// Gets the erase.
        /// </summary>
        /// <value>
        /// The erase.
        /// </value>
        public EraseSettings Erase { get; } = new();


        /// <summary>
        /// LOGIC HUB: This determines what is active based on the current selection
        /// Updates the sub states.
        /// </summary>
        private void UpdateSubStates()
        {
            // 1. Check if the Active Tool is "Simple"
            bool isSimpleTool = ActiveTool == DrawTool.Pencil ||
                                ActiveTool == DrawTool.Eraser ||
                                ActiveTool == DrawTool.Move ||
                                ActiveTool == DrawTool.ColorPicker;

            // 2. Main Switch: Enable/Disable the entire Mode group
            AreAreaModesEnabled = !isSimpleTool;

            // 3. Sub-Switches: Handle specific option panels
            if (isSimpleTool)
            {
                // Simple tools have no sub-options
                Fill.Enabled = false;
                Texture.Enabled = false;
                Filter.Enabled = false;
                Erase.Enabled = false;
            }
            else
            {
                // Complex tools show the panel matching the current Mode
                Fill.Enabled = ActiveAreaMode == AreaMode.Fill;
                Texture.Enabled = ActiveAreaMode == AreaMode.Texture;
                Filter.Enabled = ActiveAreaMode == AreaMode.Filter;
                Erase.Enabled = ActiveAreaMode == AreaMode.Erase;
            }

            // 4. Notify UI to refresh visibility
            OnPropertyChanged(nameof(Fill));
            OnPropertyChanged(nameof(Texture));
            OnPropertyChanged(nameof(Filter));
            OnPropertyChanged(nameof(Erase));
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