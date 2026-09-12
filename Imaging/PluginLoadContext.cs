/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging
 * FILE:        PluginLoadContext.cs
 * PURPOSE:     Plugin Loader for advanced plugins.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Imaging
{
    /// <inheritdoc />
    /// <summary>
    /// A loader for Plugins.
    /// </summary>
    /// <seealso cref="System.Runtime.Loader.AssemblyLoadContext" />
    public sealed class PluginLoadContext : AssemblyLoadContext
    {
        private readonly string _pluginPath;
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
            : base(
                name: $"Plugin:{Path.GetFileNameWithoutExtension(pluginPath)}",
                isCollectible: false)
        {
            _pluginPath = pluginPath;
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        /// <inheritdoc />
        protected override Assembly? Load(
            AssemblyName assemblyName)
        {
            // These assemblies define the types shared between
            // the host and the plugin. They must come from the
            // default context.

            if (assemblyName.Name is
                "Imaging.Plugins.Interface" or
                "System.Drawing.Common")
            {
                return AssemblyLoadContext.Default
                    .LoadFromAssemblyName(assemblyName);
            }

            var assemblyPath =
                _resolver.ResolveAssemblyToPath(assemblyName);

            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        /// <inheritdoc />
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            Trace.WriteLine(
                $"[PluginLoadContext] Native dependency requested: {unmanagedDllName}");

            Trace.WriteLine(
                $"[PluginLoadContext] Plugin path: {_pluginPath}");

            var libraryPath =
                _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            Trace.WriteLine(
                $"[PluginLoadContext] Native dependency resolved to: {libraryPath ?? "<not found>"}");

            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
