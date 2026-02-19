/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        FillSettings.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.ComponentModel;

namespace SlimControls
{
    public class FillSettings : INotifyPropertyChanged
    {
        private bool _enabled;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }

        public string Color { get; set; } = "#FFFFFF";

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}