using Mbc.Log4Tc.Model;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Log4TcLevel = Mbc.Log4Tc.Model.LogLevel;
using NLogLevel = NLog.LogLevel;

namespace Mbc.Log4Tc.Output.NLog
{
    public class NLogLog4TcOutput : IOutputHandler
    {
        static NLogLog4TcOutput()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "log4tc", "config", "NLog.config");
            LogManager.Configuration = new XmlLoggingConfiguration(path);
        }

        private readonly Logger _dispatchLogger = LogManager.GetLogger("TwinCat");

        public NLogLog4TcOutput(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void ProcesLogEntry(LogEntry logEntry)
        {
            var logEvent = new LogEventInfo
            {
                Level = ConvertToNLogLevel(logEntry.Level),
                LoggerName = logEntry.Logger,
                Message = logEntry.Message,
                Parameters = ConvertToNLogParameters(logEntry.Arguments),
                TimeStamp = logEntry.PlcTimestamp,
            };

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
            logEvent.Properties.Add("_TcLogSource_", logEntry.Source);

            _dispatchLogger.Log(logEvent);
        }

        private object[] ConvertToNLogParameters(IDictionary<int, object> arguments)
        {
            if (arguments.Count == 0)
            {
                return new object[0];
            }

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
