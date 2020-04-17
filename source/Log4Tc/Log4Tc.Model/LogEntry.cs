using System;
using System.Collections.Generic;

namespace Log4Tc.Model
{
    public class LogEntry
    {
        public string Source { get; set; }

        public string Message { get; set; }

        public string Logger { get; set; }

        public LogLevel Level { get; set; }

        public DateTime PlcTimestamp { get; set; }

        public DateTime ClockTimestamp { get; set; }

        public int TaskIndex { get; set; }

        public string TaskName { get; set; }

        public uint TaskCycleCounter { get; set; }

        public string AppName { get; set; }

        public string ProjectName { get; set; }

        public uint OnlineChangeCount { get; set; }

        public IDictionary<int, object> Arguments { get; } = new Dictionary<int, object>();

        public IDictionary<string, object> Context { get; } = new Dictionary<string, object>();
    }
}
