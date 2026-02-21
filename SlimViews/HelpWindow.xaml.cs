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
    public partial class HelpWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelpWindow"/> class.
        /// </summary>
        public HelpWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}