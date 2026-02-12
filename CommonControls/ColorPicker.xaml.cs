/*
 * FILE:        ColorPicker.xaml.cs
 * PURPOSE:     Resolution-independent Color Picker (Hue Ring + SV Triangle)
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Imaging; // Uses your existing ColorHsv class

namespace CommonControls
{
    public sealed partial class ColorPicker : UserControl, INotifyPropertyChanged
    {
        // --- Internal Fields ---
        private WriteableBitmap _bitmap; // Fast pixel buffer
        private bool _isDragging;

        // Internal Color State (0.0 to 1.0 or 0 to 360)
        private double _h = 0;
        private double _s = 1;
        private double _v = 1;

        // --- Events ---
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void DelegateColor(ColorHsv colorHsv);
        public event DelegateColor ColorChanged;

        public ColorPicker()
        {
            InitializeComponent();
        }

        // --- Public Properties (Data Binding) ---

        public double Hue
        {
            get => _h;
            set { if (SetField(ref _h, value)) { RedrawAsync(); UpdateCursorPosition(); } }
        }

        public double Sat
        {
            get => _s;
            set { if (SetField(ref _s, value)) { UpdateCursorPosition(); } }
        }

        public double Val
        {
            get => _v;
            set { if (SetField(ref _v, value)) { UpdateCursorPosition(); } }
        }

        // --- Mouse Interaction ---

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            Mouse.Capture(PickerImage);
            ProcessMouse(e.GetPosition(PickerImage));
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging) ProcessMouse(e.GetPosition(PickerImage));
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            Mouse.Capture(null);
        }

        // The core logic that maps "Click Position" -> "H/S/V Value"
        private void ProcessMouse(Point p)
        {
            double size = Math.Min(ActualWidth, ActualHeight);
            if (size <= 0) return;

            // Define Geometry relative to current size
            double radius = size / 2.0;
            double innerRadius = radius * 0.85; // Triangle is 85% of the radius
            double centerX = size / 2.0;
            double centerY = size / 2.0;

            double dx = p.X - centerX;
            double dy = p.Y - centerY;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            // CASE 1: Clicked on the Hue Ring
            if (dist > innerRadius && dist <= radius)
            {
                double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
                if (angle < 0) angle += 360;

                Hue = angle; // Triggers Redraw
                TriggerColorChange();
            }
            // CASE 2: Clicked Inside (Triangle Area)
            else if (dist <= innerRadius)
            {
                // Convert Mouse X/Y to Sat/Val
                if (GetSvFromPoint(dx, dy, innerRadius, _h, out double s, out double v))
                {
                    // Update internal values directly to prevent unnecessary redraws
                    _s = s;
                    _v = v;

                    // Notify UI
                    OnPropertyChanged(nameof(Sat));
                    OnPropertyChanged(nameof(Val));

                    UpdateCursorPosition();
                    TriggerColorChange();
                }
            }
        }

        // Sends the ColorHsv object out to the world
        private void TriggerColorChange()
        {
            var color = ColorHsv.FromHsv(_h, _s, _v);
            ColorChanged?.Invoke(color);
        }

        // --- Rendering Logic (The Fast Part) ---

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0) RedrawAsync();
        }

        // Draws the Triangle and Ring pixels
        private void RedrawAsync()
        {
            int size = (int)Math.Min(ActualWidth, ActualHeight);
            if (size <= 0) return;

            // Resize Bitmap if needed
            if (_bitmap == null || _bitmap.PixelWidth != size || _bitmap.PixelHeight != size)
            {
                _bitmap = new WriteableBitmap(size, size, 96, 96, PixelFormats.Bgra32, null);
                PickerImage.Source = _bitmap;
            }

            _bitmap.Lock();

            unsafe
            {
                int* pBackBuffer = (int*)_bitmap.BackBuffer;
                int stride = _bitmap.BackBufferStride;

                double radius = size / 2.0;
                double innerRadius = radius * 0.85;
                double centerX = size / 2.0;
                double centerY = size / 2.0;

                // Loop through every pixel
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        double dx = x - centerX;
                        double dy = y - centerY;
                        double dist = Math.Sqrt(dx * dx + dy * dy);

                        int colorData = 0; // Default transparent

                        // 1. Draw Hue Ring
                        if (dist <= radius && dist >= innerRadius - 1)
                        {
                            double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
                            if (angle < 0) angle += 360;

                            // Anti-alias edge
                            int alpha = 255;
                            if (dist > radius - 1) alpha = (int)((radius - dist) * 255);
                            else if (dist < innerRadius) alpha = (int)((dist - (innerRadius - 1)) * 255);

                            colorData = HsvToInt(angle, 1, 1, alpha);
                        }
                        // 2. Draw Triangle
                        else if (dist < innerRadius)
                        {
                            // Math: Is this pixel inside the triangle?
                            if (GetSvFromPoint(dx, dy, innerRadius, _h, out double s, out double v))
                            {
                                colorData = HsvToInt(_h, s, v, 255);
                            }
                        }

                        // Write pixel
                        *(pBackBuffer + y * (stride / 4) + x) = colorData;
                    }
                }
            }

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, size, size));
            _bitmap.Unlock();

            UpdateCursorPosition();
        }

        // Moves the selection circle without redrawing the background
        private void UpdateCursorPosition()
        {
            double size = Math.Min(ActualWidth, ActualHeight);
            if (size <= 0) return;

            double innerRadius = size / 2.0 * 0.85;

            // Math: Where is the Sat/Val located on screen?
            Point p = GetPointFromSv(_h, _s, _v, innerRadius);

            // Center the cursor ellipse on that point
            double canvasX = p.X + ActualWidth / 2.0 - SelectionCursor.Width / 2.0;
            double canvasY = p.Y + ActualHeight / 2.0 - SelectionCursor.Height / 2.0;

            Canvas.SetLeft(SelectionCursor, canvasX);
            Canvas.SetTop(SelectionCursor, canvasY);
        }

        // --- MATH HELPERS (Resolution Independent) ---
        // These replace your "ColorProcessing.cs" and work at any scale.

        // Converts X,Y -> Saturation, Value
        private bool GetSvFromPoint(double x, double y, double r, double hue, out double s, out double v)
        {
            s = 0; v = 0;
            double hueRad = hue * Math.PI / 180.0;

            // Triangle Vertices (Rotated by Hue)
            Point pColor = new Point(Math.Cos(hueRad) * r, Math.Sin(hueRad) * r);
            Point pWhite = new Point(Math.Cos(hueRad + 2 * Math.PI / 3) * r, Math.Sin(hueRad + 2 * Math.PI / 3) * r);
            Point pBlack = new Point(Math.Cos(hueRad + 4 * Math.PI / 3) * r, Math.Sin(hueRad + 4 * Math.PI / 3) * r);

            // Barycentric Coordinates (Standard algorithm to check point inside triangle)
            double det = (pWhite.Y - pBlack.Y) * (pColor.X - pBlack.X) + (pBlack.X - pWhite.X) * (pColor.Y - pBlack.Y);
            double w1 = ((pWhite.Y - pBlack.Y) * (x - pBlack.X) + (pBlack.X - pWhite.X) * (y - pBlack.Y)) / det;
            double w2 = ((pBlack.Y - pColor.Y) * (x - pBlack.X) + (pColor.X - pBlack.X) * (y - pBlack.Y)) / det;
            double w3 = 1.0 - w1 - w2;

            // Outside triangle?
            if (w1 < -0.01 || w2 < -0.01 || w3 < -0.01) return false;

            // Map Weights to HSV Model
            // w1 = Weight of Color (S=1, V=1)
            // w2 = Weight of White (S=0, V=1)
            // w3 = Weight of Black (V=0)

            v = w1 + w2;
            s = v <= 0.001 ? 0 : w1 / v;

            return true;
        }

        // Converts Saturation, Value -> X,Y
        private Point GetPointFromSv(double h, double s, double v, double r)
        {
            double hueRad = h * Math.PI / 180.0;
            Point pColor = new Point(Math.Cos(hueRad) * r, Math.Sin(hueRad) * r);
            Point pWhite = new Point(Math.Cos(hueRad + 2 * Math.PI / 3) * r, Math.Sin(hueRad + 2 * Math.PI / 3) * r);
            Point pBlack = new Point(Math.Cos(hueRad + 4 * Math.PI / 3) * r, Math.Sin(hueRad + 4 * Math.PI / 3) * r);

            // Weights derived from S and V
            double w1 = s * v;       // Color
            double w2 = v * (1 - s); // White
            double w3 = 1 - v;       // Black

            return new Point(
                w1 * pColor.X + w2 * pWhite.X + w3 * pBlack.X,
                w1 * pColor.Y + w2 * pWhite.Y + w3 * pBlack.Y
            );
        }

        // Fast Integer Color Conversion (Avoids creating Color objects in loop)
        private static int HsvToInt(double h, double s, double v, int alpha)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs(h / 60.0 % 2 - 1));
            double m = v - c;
            double r = 0, g = 0, b = 0;

            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            byte R = (byte)((r + m) * 255);
            byte G = (byte)((g + m) * 255);
            byte B = (byte)((b + m) * 255);

            return alpha << 24 | R << 16 | G << 8 | B;
        }

        // Helper for Property Changed
        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}