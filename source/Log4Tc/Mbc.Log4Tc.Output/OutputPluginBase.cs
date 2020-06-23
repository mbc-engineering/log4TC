using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mbc.Log4Tc.Output
{
    public abstract class OutputPluginBase : IOutputPlugin
    {
        public virtual Type OutputType { get; }

        public virtual string ConfigTypeAlias { get; }

        /// <summary>
        /// Configure the IOC of this component
        /// </summary>
        public virtual IServiceCollection ConfigureServices(IConfigurationSection configSection, IServiceCollection services)
        {
            return services;
        }

        protected IConfiguration GetConfiguration(IConfigurationSection configSection)
        {
            if (!configSection.Path.EndsWith("Config", StringComparison.InvariantCulture))
            {
                throw new ApplicationException("The Output Channel Configuration should named with 'Config'");
            }

            return configSection;
        }
    }
}
