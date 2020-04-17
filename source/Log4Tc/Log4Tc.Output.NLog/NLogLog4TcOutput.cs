using Log4Tc.Model;
using Log4Tc.Receiver;
using Mbc.Common.Interface;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Log4TcLevel = Log4Tc.Model.LogLevel;
using NLogLevel = NLog.LogLevel;

namespace Log4Tc.Output.NLog
{
    public class NLogLog4TcOutput : IServiceStartable
    {
        private static readonly Logger DispatchLogger = LogManager.GetLogger("TwinCat");

        private readonly BufferBlock<IEnumerable<LogEntry>> _logEntryBuffer = new BufferBlock<IEnumerable<LogEntry>>();
        private readonly ILogDispatcher _dispatcher;

        public NLogLog4TcOutput(ILogDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Start()
        {
            _logEntryBuffer.LinkTo(new ActionBlock<IEnumerable<LogEntry>>(ProcessLogEntries));
            _dispatcher.DispatchLogEntry += OnLogDispatch;
        }

        public void Stop()
        {
            _dispatcher.DispatchLogEntry -= OnLogDispatch;
        }

        private void OnLogDispatch(object sender, LogEntryEventArgs e)
        {
            _logEntryBuffer.Post(e.LogEntries);
        }

        private void ProcessLogEntries(IEnumerable<LogEntry> logEntries)
        {
            foreach (var logEntry in logEntries)
            {
                ProcesLogEntry(logEntry);
            }
        }

        private void ProcesLogEntry(LogEntry logEntry)
        {
            var logEvent = new LogEventInfo
            {
                Level = ConvertToNLogLevel(logEntry.Level),
                LoggerName = logEntry.Logger,
                Message = logEntry.Message,
                Parameters = ConvertToNLogParameters(logEntry.Arguments),
                TimeStamp = logEntry.PlcTimestamp,
            };
            logEvent.StackTrace.

            foreach (var ctxProp in logEntry.Context)
            {
                logEvent.Properties.Add(ctxProp.Key, ctxProp.Value);
            }

            logEvent.Properties.Add("_TcTaskIdx_", logEntry.TaskIndex);
            logEvent.Properties.Add("_TcTaskName_", logEntry.TaskName);
            logEvent.Properties.Add("_TcTaskCycleCounter_", logEntry.TaskCycleCounter);
            logEvent.Properties.Add("_TcAppName_", logEntry.AppName);
            logEvent.Properties.Add("_TcProjectName_", logEntry.ProjectName);
            logEvent.Properties.Add("_TcOnlineChangeCount_", logEntry.OnlineChangeCount);

            DispatchLogger.Log(logEvent);
        }

        private object[] ConvertToNLogParameters(IDictionary<int, object> arguments)
        {
            var count = arguments.Keys.Max();
            var parameters = new object[count];

            for (int i = 0; i < count; ++i)
            {
                if (arguments.TryGetValue(i + 1, out object value))
                {
                    parameters[i] = value;
                }
            }

            return parameters;
        }

        private NLogLevel ConvertToNLogLevel(Log4TcLevel level)
        {
            switch (level)
            {
                case Log4TcLevel.Trace:
                    return NLogLevel.Trace;
                case Log4TcLevel.Debug:
                    return NLogLevel.Debug;
                case Log4TcLevel.Info:
                    return NLogLevel.Info;
                case Log4TcLevel.Warn:
                    return NLogLevel.Warn;
                case Log4TcLevel.Error:
                    return NLogLevel.Error;
                case Log4TcLevel.Fatal:
                    return NLogLevel.Fatal;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
