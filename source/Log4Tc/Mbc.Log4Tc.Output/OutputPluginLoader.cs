using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mbc.Log4Tc.Output
{
    public static class OutputPluginLoader
    {
        public static IEnumerable<IOutputPlugin> LoadOutputPlugins(string pluginFolder)
        {
            var assemblyFilePaths = GetOutputPuginPaths(pluginFolder);

            return assemblyFilePaths.SelectMany(assemblyFilePath =>
            {
                var assembly = Assembly.LoadFrom(assemblyFilePath);

                return CreateOutputPlugins(assembly);
            });
        }

        private static IEnumerable<string> GetOutputPuginPaths(string pluginFolder)
        {
            string absolutePath = pluginFolder;
            if (!Path.IsPathRooted(absolutePath))
            {
                string root = Path.GetDirectoryName(typeof(OutputPluginLoader).Assembly.Location);
                absolutePath = Path.GetFullPath(Path.Combine(root, pluginFolder.Replace('\\', Path.DirectorySeparatorChar)));
            }

            // Find all dll in this folder
            return Directory.GetFiles(absolutePath, "*.dll", SearchOption.AllDirectories);
        }

        private static IEnumerable<IOutputPlugin> CreateOutputPlugins(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes().Where(x => typeof(IOutputPlugin).IsAssignableFrom(x)))
            {
                IOutputPlugin result = Activator.CreateInstance(type) as IOutputPlugin;
                if (result != null)
                {
                    yield return result;
                }
            }
        }
    }
}
