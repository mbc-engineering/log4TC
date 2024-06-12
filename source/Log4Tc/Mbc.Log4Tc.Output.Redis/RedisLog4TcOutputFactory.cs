using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.DataAnnotations;

namespace Mbc.Log4Tc.Output.Redis
{
    public class RedisLog4TcOutputFactory : IOutputFactory
    {
        public string ShortTypeName => "redis";

        public OutputHandlerBase Create(IServiceProvider serviceProvider, IConfigurationSection outputConfiguration)
        {
            var config = new RedisOutputSettings();

            outputConfiguration.Bind(config);
            Validator.ValidateObject(config, new ValidationContext(config));

            return ActivatorUtilities.CreateInstance<RedisOutput>(serviceProvider, config);
        }
    }
}
