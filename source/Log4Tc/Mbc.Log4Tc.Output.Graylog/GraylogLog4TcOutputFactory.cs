using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.DataAnnotations;

namespace Mbc.Log4Tc.Output.Graylog
{
    internal class GraylogLog4TcOutputFactory : IOutputFactory
    {
        public string ShortTypeName => "graylog";

        public OutputHandlerBase Create(IServiceProvider serviceProvider, IConfigurationSection outputConfiguration)
        {
            var config = new GraylogOutputSettings();
            outputConfiguration.Bind(config);
            Validator.ValidateObject(config, new ValidationContext(config));
            return ActivatorUtilities.CreateInstance<GraylogOutput>(serviceProvider, config);
        }
    }
}
