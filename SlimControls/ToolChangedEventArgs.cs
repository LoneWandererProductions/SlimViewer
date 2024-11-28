using CommonControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SlimControls
{
    public class ToolChangedEventArgs : RoutedEventArgs
    {
        public ImageZoomTools SelectedTool { get; }

        public ToolChangedEventArgs(RoutedEvent routedEvent, object source, ImageZoomTools selectedTool)
            : base(routedEvent, source)
        {
            SelectedTool = selectedTool;
        }
    }

}
