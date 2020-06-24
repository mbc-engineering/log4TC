using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mbc.Log4Tc.Output.NLog
{
    public class NLogLog4TcOutputFactory : IOutputFactory
    {
        public string ShortTypeName => "nlog";

        public IOutputHandler Create(IServiceProvider serviceProvider, IConfigurationSection outputConfiguration)
        {
            var config = new NLogLog4TcOutputConfiguration();
            outputConfiguration.Bind(config);
            return new NLogLog4TcOutput(config);
        }
    }
}
