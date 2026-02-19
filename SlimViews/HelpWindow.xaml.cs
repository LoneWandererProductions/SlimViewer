/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        HelpWindow.xaml.cs
 * PURPOSE:     Simple help window for the SlimViewer.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows;

namespace SlimViews
{
    /// <summary>
    /// Simple help window for the SlimViewer.
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
