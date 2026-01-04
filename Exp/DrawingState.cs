/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Exp
 * FILE:        DrawingState.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.ComponentModel;

namespace Exp
{
    public class DrawingState : INotifyPropertyChanged
    {
        private DrawTool _activeTool;

        public event EventHandler? ToolOrModeChanged;

        private void OnToolOrModeChanged()
            => ToolOrModeChanged?.Invoke(this, EventArgs.Empty);

        public DrawTool ActiveTool
        {
            get => _activeTool;
            set
            {
                if (_activeTool == value) return;
                _activeTool = value;
                OnPropertyChanged(nameof(ActiveTool));
                OnToolOrModeChanged(); // fire event
            }
        }

        private AreaMode _activeAreaMode;

        public AreaMode ActiveAreaMode
        {
            get => _activeAreaMode;
            set
            {
                if (_activeAreaMode == value) return;
                _activeAreaMode = value;
                OnPropertyChanged(nameof(ActiveAreaMode));
                OnToolOrModeChanged(); // fire event
            }
        }

        public double BrushSize { get; set; } = 5;
        public double BrushOpacity { get; set; } = 1.0;
        public string BrushColor { get; set; } = "#000000";

        // Sub-states
        public FillSettings Fill { get; } = new();
        public TextureSettings Texture { get; } = new();
        public FilterSettings Filter { get; } = new();
        public EraseSettings Erase { get; } = new();

        private void UpdateSubStates()
        {
            Fill.Enabled = ActiveAreaMode == AreaMode.Fill;
            Texture.Enabled = ActiveAreaMode == AreaMode.Texture;
            Filter.Enabled = ActiveAreaMode == AreaMode.Filter;
            Erase.Enabled = ActiveAreaMode == AreaMode.Erase;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}