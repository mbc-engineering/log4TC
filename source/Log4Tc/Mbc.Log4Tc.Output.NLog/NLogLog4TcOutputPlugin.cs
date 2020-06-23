using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mbc.Log4Tc.Output.NLog
{
    public class NLogLog4TcOutputPlugin : OutputPluginBase
    {
        public override string ConfigTypeAlias => "nlog";

        public override Type OutputType => typeof(NLogLog4TcOutput);

        public override IServiceCollection ConfigureServices(IConfigurationSection configSection, IServiceCollection services)
        {
            services.AddLog4TcNLogOutputSettings(GetConfiguration(configSection));

            return base.ConfigureServices(configSection, services);
        }
    }
}
