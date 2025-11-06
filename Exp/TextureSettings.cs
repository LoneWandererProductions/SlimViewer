/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Exp
 * FILE:        TextureSettings.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.ComponentModel;

namespace Exp
{
    public class TextureSettings : INotifyPropertyChanged
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

        public string TextureName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}