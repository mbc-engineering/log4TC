using Mbc.Log4Tc.Output;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

            // Add Config Parameters
            var outputConfigs = configuration
                .GetSection("Outputs")
                .GetChildren()
                .ToList()
                .Select(o =>
                    new OutputConfiguration()
                    {
                        Type = o.GetValue<string>("Type"),
                        Filter = o.GetValue<object>("Filter"),
                        ConfigSection = o.GetSection("Config"),
                    }).ToList();

            services
                .Configure<List<OutputConfiguration>>(config => config.AddRange(outputConfigs));

            // Load the Plugin Assemblies and Create Plugin instance
            var outputPlugins = OutputPluginLoader.LoadOutputPlugins(pluginFolder);

            // Register all Plugin instances
            services.TryAddEnumerable(outputPlugins.Select(p => ServiceDescriptor.Singleton(p)));

            // Aktivate all outputs present in the application config
            ActivateConfiguredOutputs(services, outputPlugins, outputConfigs);

            return services;
        }

        private static void ActivateConfiguredOutputs(IServiceCollection services, IEnumerable<IOutputPlugin> outputPlugins, IEnumerable<OutputConfiguration> outputConfigs)
        {
            // Load the Plugin Assemblies
            foreach (var outputConfig in outputConfigs)
            {
                var pluginForConfig = outputPlugins.FirstOrDefault(p => string.Equals(p.ConfigTypeAlias, outputConfig.Type, StringComparison.OrdinalIgnoreCase));
                if (pluginForConfig != null)
                {
                    // Configure DI for Plugin
                    pluginForConfig.ConfigureServices(outputConfig.ConfigSection, services);

                    services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IOutputHandler), pluginForConfig.OutputType));
                }
                else
                {
                    throw new ApplicationException($"The configured output type {outputConfig.Type} was not found in the availlable output Plugins.");
                }
            }
        }
    }
}
