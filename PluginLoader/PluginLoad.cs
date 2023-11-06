﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Plugin
 * FILE:        Plugin/PluginLoader.cs
 * PURPOSE:     Basic Plugin Support, Load all Plugins
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 * SOURCES:     https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FileHandler;
using Plugin;

namespace PluginLoader
{
    /// <summary>
    ///     Basic Load System for the Plugins
    /// </summary>
    public static class PluginLoad
    {
        /// <summary>
        ///     Gets or sets the plugin container.
        /// </summary>
        /// <value>
        ///     The plugin container.
        /// </value>
        public static List<IPlugin> PluginContainer { get; private set; }

        /// <summary>
        ///     Loads all.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Success Status</returns>
        public static bool LoadAll(string path)
        {
            var pluginPaths = FileHandleSearch.GetFilesByExtensionFullPath(path, PluginLoaderResources.FileExt, false);

            if (pluginPaths == null) return false;

            try
            {
                PluginContainer = new List<IPlugin>();

                foreach (var pluginPath in pluginPaths)
                    try
                    {
                        var pluginAssembly = LoadPlugin(pluginPath);
                        var lst = CreateCommands(pluginAssembly).ToList();
                        PluginContainer.AddRange(lst);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
            }
            catch (FileLoadException ex)
            {
                Trace.WriteLine(ex);
            }
            catch (ApplicationException ex)
            {
                Trace.WriteLine(ex);
            }
            catch (BadImageFormatException ex)
            {
                Trace.WriteLine(ex);
            }
            catch (FileNotFoundException ex)
            {
                Trace.WriteLine(ex);
            }

            return PluginContainer.Count != 0;
        }

        /// <summary>
        ///     Loads the plugin.
        /// </summary>
        /// <param name="pluginLocation">The plugin location.</param>
        /// <returns>An Assembly</returns>
        private static Assembly LoadPlugin(string pluginLocation)
        {
            var loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        /// <summary>
        ///     Creates the commands.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Adds References to the Commands</returns>
        /// <exception cref="ApplicationException">
        ///     Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
        ///     $"Available types: {availableTypes}
        /// </exception>
        /// <exception cref="ArgumentException">Could not find the Plugin</exception>
        private static IEnumerable<IPlugin> CreateCommands(Assembly assembly)
        {
            var count = 0;

            foreach (var type in assembly.GetTypes().Where(type => typeof(IPlugin).IsAssignableFrom(type)))
            {
                if (Activator.CreateInstance(type) is not IPlugin result) continue;

                count++;
                yield return result;
            }

            if (count != 0) yield break;

            var availableTypes =
                string.Join(PluginLoaderResources.Separator, assembly.GetTypes().Select(t => t.FullName));

            var message = string.Concat(PluginLoaderResources.ErrorCouldNotFindPlugin,
                $" {assembly} from {assembly.Location}.",
                Environment.NewLine, PluginLoaderResources.MessageTypes,
                $" {availableTypes}");

            throw new ArgumentException(message);
        }
    }
}