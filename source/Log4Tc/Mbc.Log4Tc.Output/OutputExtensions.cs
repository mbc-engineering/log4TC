using Microsoft.Extensions.Configuration;

namespace Mbc.Log4Tc.Service
{
    public static class OutputExtensions
    {
        public static PluginBuilder AddOutputs(this PluginBuilder pluginBuilder, IConfiguration configuration)
        {
            // Aktivate all outputs present in the application config
            pluginBuilder.ActivateConfiguredOutputs(configuration);

            return pluginBuilder;
        }
    }
}
