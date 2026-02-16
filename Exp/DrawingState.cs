/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Exp
 * FILE:        DrawingState.cs
 * PURPOSE:     Manages the state of the drawing tools and modes
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Exp
{
    public class DrawingState : INotifyPropertyChanged
    {
        private DrawTool _activeTool;
        private AreaMode _activeAreaMode;
        private ShapeType _selectedShape; // Backing field for Shape


        public event EventHandler? ToolOrModeChanged;

        private bool _areAreaModesEnabled;
        public bool AreAreaModesEnabled
        {
            get => _areAreaModesEnabled;
            set { _areAreaModesEnabled = value; OnPropertyChanged(); }
        }

        private void OnToolOrModeChanged()
            => ToolOrModeChanged?.Invoke(this, EventArgs.Empty);

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

        // 2. Added SelectedShape Property
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

        // Brush Properties (added full implementation for notification)
        private double _brushSize = 5;
        public double BrushSize
        {
            get => _brushSize;
            set { _brushSize = value; OnPropertyChanged(); }
        }

        private string _brushColor = "#000000";
        public string BrushColor
        {
            get => _brushColor;
            set { _brushColor = value; OnPropertyChanged(); }
        }

        public double BrushOpacity { get; set; } = 1.0;

        // Sub-states (These hold the data for the specific modes)
        public FillSettings Fill { get; } = new();
        public TextureSettings Texture { get; } = new();
        public FilterSettings Filter { get; } = new();
        public EraseSettings Erase { get; } = new();


        /// <summary>
        /// LOGIC HUB: This determines what is active based on the current selection
        /// Updates the sub states.
        /// </summary>
        private void UpdateSubStates()
        {
            // 1. Check if the Active Tool is "Simple"
            bool isSimpleTool = ActiveTool == DrawTool.Pencil || ActiveTool == DrawTool.Eraser;

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

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}