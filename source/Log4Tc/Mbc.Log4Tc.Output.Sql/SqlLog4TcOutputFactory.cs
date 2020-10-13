using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.DataAnnotations;

namespace Mbc.Log4Tc.Output.Sql
{
    public class SqlLog4TcOutputFactory : IOutputFactory
    {
        public string ShortTypeName => "sql";

        public OutputHandlerBase Create(IServiceProvider serviceProvider, IConfigurationSection outputConfiguration)
        {
            var config = new SqlOutputSettings();
            outputConfiguration.Bind(config);
            Validator.ValidateObject(config, new ValidationContext(config));

            return ActivatorUtilities.CreateInstance<SqlOutput>(serviceProvider, config);
        }
    }
}
