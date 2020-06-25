using Mbc.Log4Tc.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Mbc.Log4Tc.Service
{
    public class PluginBuilder
    {
        private IServiceCollection _services;

        public PluginBuilder(IServiceCollection services, IEnumerable<IPlugin> plugins)
        {
            _services = services;
            Plugins = plugins;
        }

        public IEnumerable<IPlugin> Plugins { get; set; }

        public void ActivateConfiguredOutputs(IConfiguration configuration)
        {
            foreach (var plugin in Plugins)
            {
                plugin.ConfigureServices(_services, configuration);
            }
        }
    }
}
