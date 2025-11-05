/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        ToolOptionsViewModel.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ViewModel;

namespace SlimControls
{
    public class ToolOptionsViewModel : INotifyPropertyChanged
    {
        private ShapeTool? _selectedShape;
        private AreaOperation? _selectedOperation;

        public ShapeTool? SelectedShape
        {
            get => _selectedShape;
            set { _selectedShape = value; OnPropertyChanged(); }
        }

        public AreaOperation? SelectedOperation
        {
            get => _selectedOperation;
            set { _selectedOperation = value; OnPropertyChanged(); }
        }

        // Command to execute an operation
        public ICommand ExecuteOperationCommand { get; }

        public ToolOptionsViewModel()
        {
            FillCommand = new RelayCommand(() => ExecuteOperation(AreaOperation.Fill));
            TextureCommand = new RelayCommand(() => ExecuteOperation(AreaOperation.Texture));
            FilterCommand = new RelayCommand(() => ExecuteOperation(AreaOperation.Filter));
        }

        private void ExecuteOperation(AreaOperation op)
        {
            Debug.WriteLine($"Executing {op} on {SelectedShape}");
            // Call AreaControl logic here
        }

        public RelayCommand FillCommand { get; }
        public RelayCommand TextureCommand { get; }
        public RelayCommand FilterCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}