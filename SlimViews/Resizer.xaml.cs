/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/Resizer.xaml.cs
 * PURPOSE:     Window for the Resizer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows;
using System.Windows.Markup;

namespace SlimViews
{
    /// <summary>
    /// Window for mass converting Images in a Folder
    /// </summary>
    /// <seealso cref="Window" />
    /// <seealso cref="IComponentConnector" />
    internal sealed partial class Resizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Resizer"/> class.
        /// </summary>
        public Resizer()
        {
            InitializeComponent();
        }

    }
}
