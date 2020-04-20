using Log4Tc.Dispatcher.DispatchExpression;
using Log4Tc.Model;
using Log4Tc.Output;
using Log4Tc.Receiver;
using Mbc.Common.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace Log4Tc.Dispatcher
{
    public class LogDispatcher : IServiceStartable
    {
        private readonly List<ILogReceiver> _receiver;
        private readonly Dictionary<string, IOutputHandler> _outputs;
        private readonly List<IDispatchExpression> _dispatchExpressions;
        private readonly BufferBlock<IEnumerable<LogEntry>> _logEntryBuffer = new BufferBlock<IEnumerable<LogEntry>>();

        public LogDispatcher(IEnumerable<ILogReceiver> receiver, IEnumerable<IOutputHandler> outputs, IEnumerable<IDispatchExpression> dispatchExpressions)
        {
            _receiver = receiver.ToList();
            _outputs = outputs.ToDictionary(x => x.Name);
            _dispatchExpressions = dispatchExpressions.ToList();
        }

        public void Start()
        {
            _logEntryBuffer.LinkTo(new ActionBlock<IEnumerable<LogEntry>>(ProcessLogEntries));
            foreach (var receiver in _receiver)
            {
                receiver.LogsReceived += OnLogDispatch;
            }
        }

        public void Stop()
        {
            foreach (var receiver in _receiver)
            {
                receiver.LogsReceived += OnLogDispatch;
            }
        }

        private void OnLogDispatch(object sender, LogEntryEventArgs e)
        {
            _logEntryBuffer.Post(e.LogEntries);
        }

        private void ProcessLogEntries(IEnumerable<LogEntry> logEntries)
        {
            foreach (var logEntry in logEntries)
            {
                DispatchToOutput(logEntry);
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
