/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        ToolBase.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace SlimControls
{
    public abstract record ToolBase
    {
        public ToolMode Mode { get; init; }
    }
}
