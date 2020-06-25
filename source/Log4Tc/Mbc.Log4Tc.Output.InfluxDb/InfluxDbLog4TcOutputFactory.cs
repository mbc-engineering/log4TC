using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mbc.Log4Tc.Output.InfluxDb
{
    public class InfluxDbLog4TcOutputFactory : IOutputFactory
    {
        public string ShortTypeName => "influxdb";

        public IOutputHandler Create(IServiceProvider serviceProvider, IConfigurationSection outputConfiguration)
        {
            var config = new InfluxDbOutputSettings();
            outputConfiguration.Bind(config);
            return ActivatorUtilities.CreateInstance<InfluxDbOutput>(serviceProvider, config);
        }
    }
}
