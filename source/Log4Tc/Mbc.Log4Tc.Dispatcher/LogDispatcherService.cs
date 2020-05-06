using Mbc.Log4Tc.Dispatcher.DispatchExpression;
using Mbc.Log4Tc.Model;
using Mbc.Log4Tc.Output;
using Mbc.Log4Tc.Receiver;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Mbc.Log4Tc.Dispatcher
{
    public class LogDispatcherService : IHostedService
    {
        private readonly List<ILogReceiver> _receiver;
        private readonly Dictionary<string, IOutputHandler> _outputs;
        private readonly List<IDispatchExpression> _dispatchExpressions;
        private readonly BufferBlock<IEnumerable<LogEntry>> _logEntryBuffer = new BufferBlock<IEnumerable<LogEntry>>();

        public LogDispatcherService(IEnumerable<ILogReceiver> receiver, IEnumerable<IOutputHandler> outputs, IEnumerable<IDispatchExpression> dispatchExpressions)
        {
            _receiver = receiver.ToList();
            _outputs = outputs.ToDictionary(x => x.Name);
            _dispatchExpressions = dispatchExpressions.ToList();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logEntryBuffer.LinkTo(new ActionBlock<IEnumerable<LogEntry>>(ProcessLogEntries));
            foreach (var receiver in _receiver)
            {
                receiver.LogsReceived += OnLogDispatch;
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var receiver in _receiver)
            {
                receiver.LogsReceived += OnLogDispatch;
            }

            return Task.CompletedTask;
        }

        private void OnLogDispatch(object sender, LogEntryEventArgs e)
        {
            _logEntryBuffer.Post(e.LogEntries);
        }

        private void ProcessLogEntries(IEnumerable<LogEntry> logEntries)
        {
            foreach (var logEntry in logEntries)
            {
                try
                {
                    DispatchToOutput(logEntry);

                }
                catch (Exception e)
                {
                    // TODO logging
                    Console.WriteLine(e);
                }
            }
        }

        private void DispatchToOutput(LogEntry logEntry)
        {
            foreach (var output in _outputs.Values)
            {
                if (_dispatchExpressions.Any(x => x.ShouldDispatch(output.Name, logEntry)))
                {
                    output.ProcesLogEntry(logEntry);
                }
            }
        }
    }
}
