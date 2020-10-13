using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mbc.Log4Tc.Output.NLog
{
    public class NLogLog4TcOutputFactory : IOutputFactory
    {
        public string ShortTypeName => "nlog";

        public OutputHandlerBase Create(IServiceProvider serviceProvider, IConfigurationSection outputConfiguration)
        {
            NLogLog4TcOutputInitializer.Setup();

            var config = new NLogLog4TcOutputConfiguration();
            outputConfiguration.Bind(config);
            return ActivatorUtilities.CreateInstance<NLogLog4TcOutput>(serviceProvider, config);
        }
    }
}
