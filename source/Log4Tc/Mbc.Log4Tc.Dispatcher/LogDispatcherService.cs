using Mbc.Log4Tc.Model;
using Mbc.Log4Tc.Receiver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Mbc.Log4Tc.Dispatcher
{
    public class LogDispatcherService : IHostedService, IDisposable
    {
        private readonly List<ILogReceiver> _receiver;
        private readonly BufferBlock<IEnumerable<LogEntry>> _logEntryBuffer = new BufferBlock<IEnumerable<LogEntry>>();
        private readonly ILogger<LogDispatcherService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _outputsConfiguration;
        private readonly List<OutputDispatch> _outputs = new List<OutputDispatch>();
        private bool _outputInitialized;
        private IChangeToken _outputChangeToken;

        public LogDispatcherService(ILogger<LogDispatcherService> logger, IEnumerable<ILogReceiver> receiver, IConfiguration outputConfiguration, IServiceProvider serviceProvider)
        {
            _receiver = receiver.ToList();
            _logger = logger;
            _serviceProvider = serviceProvider;
            _outputsConfiguration = outputConfiguration.GetSection("Outputs");
        }

        public void Dispose()
        {
            foreach (var disposableOutput in _outputs.OfType<IDisposable>())
            {
                disposableOutput.Dispose();
            }

            _outputs.Clear();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logEntryBuffer.LinkTo(new ActionBlock<IEnumerable<LogEntry>>(ProcessLogEntries));
            foreach (var receiver in _receiver)
            {
                receiver.LogsReceived += OnLogDispatch;
            }

            _logger.LogInformation("Log dispatcher started.");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var receiver in _receiver)
            {
                receiver.LogsReceived += OnLogDispatch;
            }

            _logger.LogInformation("Log dispatcher stopped.");

            return Task.CompletedTask;
        }

        private void OnLogDispatch(object sender, LogEntryEventArgs e)
        {
            _logEntryBuffer.Post(e.LogEntries);
        }

        private async Task ProcessLogEntries(IEnumerable<LogEntry> logEntries)
        {
            InitializeOutputs();

            foreach (var logEntry in logEntries)
            {
                try
                {
                    await DispatchToOutput(logEntry).ConfigureAwait(false);

                }
                catch (Exception e)
                {
                    // TODO logging
                    Console.WriteLine(e);
                }
            }
        }

        private Task DispatchToOutput(LogEntry logEntry)
        {
            return Task.WhenAll(_outputs.Select(x => x.Dispatch(logEntry)));
        }

        private void InitializeOutputs()
        {
            if (_outputInitialized && _outputChangeToken != null && !_outputChangeToken.HasChanged)
                return;

            _logger.LogInformation("Loading output configuration.");

            _outputInitialized = true;

            _outputChangeToken = _outputsConfiguration.GetReloadToken();

            _outputs.AddRange(_outputsConfiguration
                .GetChildren()
                .Select(x =>
                {
                    try
                    {
                        return ActivatorUtilities.CreateInstance<OutputDispatch>(_serviceProvider, x);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error loading output.");
                        return null;
                    }
                })
                .Where(x => x != null)
                .ToList());
        }
    }
}
