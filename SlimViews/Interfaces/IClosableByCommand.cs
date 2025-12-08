using System;

namespace SlimViews.Interfaces
{
    public interface IClosableByCommand
    {
        Action? RequestCloseAction { get; set; }
    }
}