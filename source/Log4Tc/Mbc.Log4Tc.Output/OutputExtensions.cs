using Mbc.Log4Tc.Output;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mbc.Log4Tc.Service
{
    public static class OutputExtensions
    {
        public static IServiceCollection AddOutputs(this IServiceCollection services, string pluginFolder, IConfiguration configuration)
        {
            // Check Valid Parameters
            if (!Uri.IsWellFormedUriString(pluginFolder, UriKind.RelativeOrAbsolute))
            {
                throw new ApplicationException($"The plugin folder {pluginFolder} is not well formed.");
            }

            // Load the Plugin Assemblies and Create Plugin instance
            var outputPlugins = OutputPluginLoader.LoadOutputPlugins(pluginFolder).ToList();

            // Aktivate all outputs present in the application config
            ActivateConfiguredOutputs(services, outputPlugins, configuration);

            return services;
        }

        private static void ActivateConfiguredOutputs(IServiceCollection services, IEnumerable<IOutputPlugin> outputPlugins, IConfiguration configuration)
        {
            foreach (var outputPlugin in outputPlugins)
            {
                outputPlugin.ConfigureServices(services, configuration);
            }
        }
    }
}
