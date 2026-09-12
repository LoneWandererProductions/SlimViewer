using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Imaging
{
    public sealed class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
            : base(
                name: $"Plugin:{Path.GetFileNameWithoutExtension(pluginPath)}",
                isCollectible: false)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

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

        protected override IntPtr LoadUnmanagedDll(
            string unmanagedDllName)
        {
            var libraryPath =
                _resolver.ResolveUnmanagedDllToPath(
                    unmanagedDllName);

            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
