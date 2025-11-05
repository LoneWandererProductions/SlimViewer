/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Exp
 * FILE:        DrawingToolState.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.ComponentModel;

namespace Exp
{
    public class DrawingToolState : INotifyPropertyChanged
    {
        private DrawTool _activeTool;
        public DrawTool ActiveTool
        {
            get => _activeTool;
            set { if (_activeTool == value) return; _activeTool = value; OnChanged(nameof(ActiveTool)); }
        }

        private ShapeType _activeShape;
        public ShapeType ActiveShape
        {
            get => _activeShape;
            set { if (_activeShape == value) return; _activeShape = value; OnChanged(nameof(ActiveShape)); }
        }


        private AreaMode _activeAreaMode;
        public AreaMode ActiveAreaMode
        {
            get => _activeAreaMode;
            set { _activeAreaMode = value; OnChanged(nameof(ActiveAreaMode)); }
        }

        public double BrushSize { get; set; } = 5;
        public double BrushOpacity { get; set; } = 1.0;
        public string BrushColor { get; set; } = "#000000";

        private bool _useFill, _useTexture, _useFilter, _useEraser;

        public bool UseFill
        {
            get => _useFill;
            set
            {
                if (_useFill == value) return;
                _useFill = value;
                if (value) SetExclusive(nameof(UseFill));
                OnChanged(nameof(UseFill));
                OnChanged(nameof(ShowTextureSelector));
                OnChanged(nameof(ShowFilterSelector));
            }
        }

        public bool UseTexture
        {
            get => _useTexture;
            set
            {
                if (_useTexture == value) return;
                _useTexture = value;
                if (value) SetExclusive(nameof(UseTexture));
                OnChanged(nameof(UseTexture));
                OnChanged(nameof(ShowTextureSelector));
                OnChanged(nameof(ShowFilterSelector));
            }
        }

        public bool UseFilter
        {
            get => _useFilter;
            set
            {
                if (_useFilter == value) return;
                _useFilter = value;
                if (value) SetExclusive(nameof(UseFilter));
                OnChanged(nameof(UseFilter));
                OnChanged(nameof(ShowTextureSelector));
                OnChanged(nameof(ShowFilterSelector));
            }
        }

        public bool UseEraser
        {
            get => _useEraser;
            set
            {
                if (_useEraser == value) return;
                _useEraser = value;
                if (value) SetExclusive(nameof(UseEraser));
                OnChanged(nameof(UseEraser));
                OnChanged(nameof(ShowTextureSelector));
                OnChanged(nameof(ShowFilterSelector));
            }
        }

        private void SetExclusive(string active)
        {
            if (active != nameof(UseFill)) _useFill = false;
            if (active != nameof(UseTexture)) _useTexture = false;
            if (active != nameof(UseFilter)) _useFilter = false;
            if (active != nameof(UseEraser)) _useEraser = false;

            // Raise events for all checkboxes to refresh UI
            OnChanged(nameof(UseFill));
            OnChanged(nameof(UseTexture));
            OnChanged(nameof(UseFilter));
            OnChanged(nameof(UseEraser));
        }

        public string TextureName { get; set; }
        public string Filter { get; set; }

        public bool ShowTextureSelector => UseTexture && !UseFilter;
        public bool ShowFilterSelector => UseFilter && !UseTexture;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
