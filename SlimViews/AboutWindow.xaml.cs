/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        AboutWindow.xaml.cs
 * PURPOSE:     Simple about window for the SlimViewer.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows;

namespace SlimViews
{
    /// <summary>
    /// Simple about window for the SlimViewer.
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
