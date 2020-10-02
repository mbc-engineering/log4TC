using Mbc.Log4Tc.Dispatcher.Filter;
using Mbc.Log4Tc.Model;
using Mbc.Log4Tc.Output;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Dispatcher
{
    /// <summary>
    /// Holder for a single concrete output including its filter.
    /// </summary>
    internal class OutputDispatch : IDisposable
    {
        private readonly ILogger<OutputDispatch> _logger;
        private readonly IConfiguration _outputConfig;
        private readonly IOutputHandler _output;
        private readonly ILogFilter _logFilter;
        private readonly ILogFilter _logExcludeFilter;
        private bool _hasError;

        public OutputDispatch(ILogger<OutputDispatch> logger, IServiceProvider serviceProvider, IConfiguration outputConfig)
        {
            _logger = logger;
            _outputConfig = outputConfig;

            var type = outputConfig.GetValue<string>("Type");
            var configSection = outputConfig.GetSection("Config");

            _logFilter = FilterConfigurationFactory.Create(outputConfig.GetSection("filter"), AllMatchFilter.Default);
            _logExcludeFilter = FilterConfigurationFactory.Create(outputConfig.GetSection("excludeFilter"), NoneMatchFilter.Default);

            var outputFactory = serviceProvider.GetServices<IOutputFactory>().FirstOrDefault(x => x.ShortTypeName == type);
            if (outputFactory == null)
            {
                // TODO bessere Exception?
                throw new ArgumentException($"Unknown output type '{outputConfig.GetValue<string>("Type")}'.");
            }

            _output = outputFactory.Create(serviceProvider, configSection);
            _logger.LogInformation("Loaded output '{type}' with filter '{filter}' and exclude '{excludeFilter}'.", type, _logFilter, _logExcludeFilter);
        }

        public void Dispose()
        {
            if (_output is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public async Task Dispatch(LogEntry logEntry)
        {
            try
            {
                if (!_logExcludeFilter.Matches(logEntry) && _logFilter.Matches(logEntry))
                {
                    await _output.ProcesLogEntry(logEntry);
                    _hasError = false;
                }
            }
            catch (Exception e)
            {
                if (!_hasError)
                {
                    _logger.LogError(e, "Error writing to output.");
                }

                _hasError = true;
            }
        }
    }
}
