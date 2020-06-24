using Microsoft.Extensions.Configuration;
using System;

namespace Mbc.Log4Tc.Output
{
    /// <summary>
    /// A interface for factories which creates <see cref="IOutputHandler"/> instances
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
        IOutputHandler Create(IServiceProvider serviceProvider, IConfigurationSection outputConfiguration);
    }
}
