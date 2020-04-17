using Log4Tc.Model;
using System;
using System.Collections.Generic;

namespace Log4Tc.Receiver
{
    public class LogEntryEventArgs : EventArgs
    {
        public LogEntryEventArgs(List<LogEntry> logEntries)
        {
            LogEntries = logEntries;
        }

        public List<LogEntry> LogEntries { get; }
    }
}
