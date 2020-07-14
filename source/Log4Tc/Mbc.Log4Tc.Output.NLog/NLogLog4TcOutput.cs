using Mbc.Log4Tc.Model;
using NLog;
using NLog.Config;
using NLog.MessageTemplates;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public NLogLog4TcOutput(NLogLog4TcOutputConfiguration configuration)
        {
        }

        public Task ProcesLogEntry(LogEntry logEntry)
        {
            var messageTemplateParameters = logEntry.ArgumentLabels.Zip(logEntry.ArgumentValues, (x, y) => new MessageTemplateParameter(x, y, null, CaptureType.Normal)).ToList();

            var logEvent = new LogEventInfo(ConvertToNLogLevel(logEntry.Level), logEntry.Logger, logEntry.Message, messageTemplateParameters)
            {
                TimeStamp = logEntry.PlcTimestamp,
                // Use the already formated message
                MessageFormatter = (entry) => logEntry.FormattedMessage,
                Parameters = logEntry.ArgumentValues.ToArray(),
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
            logEvent.Properties.Add("_TcHostname_", logEntry.Hostname);

            _dispatchLogger.Log(logEvent);

            return Task.CompletedTask;
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
