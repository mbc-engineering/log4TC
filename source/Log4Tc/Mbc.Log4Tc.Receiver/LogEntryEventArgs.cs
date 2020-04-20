using Mbc.Log4Tc.Model;
using System;
using System.Collections.Generic;

namespace Mbc.Log4Tc.Receiver
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
