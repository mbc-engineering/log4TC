using Mbc.Log4Tc.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mbc.Log4Tc.Output
{
    public static class PluginLoader
    {
        public static IEnumerable<IPlugin> LoadPlugins(string pluginFolder)
        {
            var assemblyFilePaths = GetPluginPaths(pluginFolder);

            return assemblyFilePaths.SelectMany(assemblyFilePath =>
            {
                var assembly = Assembly.LoadFrom(assemblyFilePath);
                return CreatePlugins(assembly);
            });
        }

        private static IEnumerable<string> GetPluginPaths(string pluginFolder)
        {
            string absolutePath = pluginFolder;
            if (!Path.IsPathRooted(absolutePath))
            {
                string root = Path.GetDirectoryName(typeof(PluginLoader).Assembly.Location);
                absolutePath = Path.GetFullPath(Path.Combine(root, pluginFolder.Replace('\\', Path.DirectorySeparatorChar)));
            }

            // Find all dll in this folder
            return Directory.GetFiles(absolutePath, "*.dll", SearchOption.AllDirectories);
        }

        private static IEnumerable<IPlugin> CreatePlugins(Assembly assembly)
        {
            var exportedTypes = Enumerable.Empty<Type>();
            try
            {
                // try to get all exported types, ignore dependencies load error
                exportedTypes = assembly.GetExportedTypes();
            }
            catch (FileNotFoundException fnfex)
            {
                Trace.WriteLine($"Dependencies error: {fnfex.Message}");
            }

            foreach (Type type in exportedTypes.Where(x => typeof(IPlugin).IsAssignableFrom(x)))
            {
                IPlugin result = Activator.CreateInstance(type) as IPlugin;
                if (result != null)
                {
                    yield return result;
                }
            }
        }
    }
}
