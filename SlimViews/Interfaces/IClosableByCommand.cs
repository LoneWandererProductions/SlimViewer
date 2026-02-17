/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews.Interfaces
 * FILE:        IClosableByCommand.cs
 * PURPOSE:     Interface defining the contract for views that can be closed via a command, allowing view models to trigger view closure through a bound action.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;

namespace SlimViews.Interfaces
{
    /// <summary>
    /// Contract needed for a view to be closable by command, i.e. to have a close command that can be bound to an action in the view model.
    /// </summary>
    public interface IClosableByCommand
    {
        /// <summary>
        /// Gets or sets the request close action.
        /// </summary>
        /// <value>
        /// The request close action.
        /// </value>
        Action? RequestCloseAction { get; set; }
    }
}