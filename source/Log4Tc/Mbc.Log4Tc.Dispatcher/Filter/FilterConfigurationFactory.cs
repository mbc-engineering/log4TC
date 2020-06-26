using Mbc.Log4Tc.Model;
using Microsoft.Extensions.Configuration;
using Optional;
using System;
using System.Linq;

namespace Mbc.Log4Tc.Dispatcher.Filter
{
    /// <summary>
    /// Creates <see cref="ILogFilter"/> from configuration.
    /// </summary>
    public static class FilterConfigurationFactory
    {
        /// <summary>
        /// Creates <see cref="ILogFilter"/> from the given <see cref="IConfigurationSection"/>. The
        /// <paramref name="defaultFilter"/> is returned if no filter was configured.
        /// </summary>
        public static ILogFilter Create(IConfigurationSection configuration, ILogFilter defaultFilter)
        {
            // simple string?
            var filterStr = configuration.Value;
            if (filterStr != null)
            {
                // not yet supported
                return defaultFilter;
            }

            if (configuration.Exists())
            {
                if (configuration.GetChildren().Count() > 1)
                {
                    return new OrFilter(configuration.GetChildren().Select(x => Create(x, defaultFilter)));
                }
                else
                {
                    var logger = configuration.GetValue<string>("Logger");
                    var level = configuration.GetValue<string>("Level");
                    return new SimpleLogFilter(
                        level.SomeNotNull().Map(x => (LogLevel)Enum.Parse(typeof(LogLevel), x)),
                        logger.SomeNotNull());
                }
            }

            return defaultFilter;
        }
    }
}
