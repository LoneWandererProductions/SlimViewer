﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViewer/Converter.xaml.cs
 * PURPOSE:     Basic Converter Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows;

namespace SlimViewer
{
    /// <summary>
    ///     Window for converting all Files
    /// </summary>
    /// <seealso cref="Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    /// <inheritdoc cref="Window" />
    internal sealed partial class Converter
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SlimViewer.Converter" /> class.
        /// </summary>
        public Converter()
        {
            InitializeComponent();
        }
    }
}