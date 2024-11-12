using CommonControls;
using System;
using System.Windows;

namespace SlimViews
{
    internal static class Translator
    {

        private static readonly ResourceDictionary _resourceDictionary;

        static Translator()
        {
            _resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/SlimViews;component/Templates/ToolOptionsTemplates.xaml", UriKind.RelativeOrAbsolute)
            };
        }


        internal static SelectionTools GetToolsFromString(string command)
        {
            switch (command)
            {
                case "Move":
                    return SelectionTools.Move;
                case "Rectangle":
                    return SelectionTools.Rectangle;
                case "Ellipse":
                    return SelectionTools.Ellipse;
                case "Free Form":
                    return SelectionTools.FreeForm;
                default:
                    return SelectionTools.Move;
            }
        }
    }
}