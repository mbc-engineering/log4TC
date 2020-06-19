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
        public static IServiceCollection AddOutputs(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Config Parameters
            var outputs = configuration
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
                .Configure<List<OutputConfiguration>>(config => config.AddRange(outputs));

            RegisterOutputHandler(services, outputs);

            return services;
        }

        private static void RegisterOutputHandler(IServiceCollection services, IEnumerable<OutputConfiguration> outputConfigs)
        {
            foreach (var outputConfig in outputConfigs)
            {
                if (OutputAliases.KnownOutputAliases.TryGetValue(outputConfig.Type, out Type outputFactoryType))
                {
                    var outputFactory = (OutputFactoryBase)Activator.CreateInstance(outputFactoryType);

                    // Configure DI
                    outputFactory.ConfigureServices(outputConfig.ConfigSection, services);

                    services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IOutputHandler), outputFactory.OutputType));
                }
                else
                {
                    // ToDo: Logging!!!
                    // _logger.LogWarning("The configured output type {outputType} was not found in the availlable output Plugins.", outputConfig.Type);
                }
            }
        }
    }
}
