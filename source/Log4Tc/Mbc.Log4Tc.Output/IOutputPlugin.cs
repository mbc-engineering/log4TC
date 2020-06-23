using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mbc.Log4Tc.Output
{
    public interface IOutputPlugin
    {
        string ConfigTypeAlias { get; }

        Type OutputType { get; }

        IServiceCollection ConfigureServices(IConfigurationSection configSection, IServiceCollection services);
    }
}
