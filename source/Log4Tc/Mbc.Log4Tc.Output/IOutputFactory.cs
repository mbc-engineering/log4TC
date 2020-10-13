using Microsoft.Extensions.Configuration;
using System;

namespace Mbc.Log4Tc.Output
{
    /// <summary>
    /// A interface for factories which creates <see cref="OutputHandlerBase"/> instances
    /// for specific outputs.
    /// </summary>
    public interface IOutputFactory
    {
        string ShortTypeName { get; }

        /// <summary>
        /// Creates the instance of this output.
        /// <paramref name="serviceProvider">The service provider of the DI framework to inject objects for example logging.</paramref>
        /// <paramref name="outputConfiguration">The output specific configuration section.</paramref>
        /// </summary>
        OutputHandlerBase Create(IServiceProvider serviceProvider, IConfigurationSection outputConfiguration);
    }
}
