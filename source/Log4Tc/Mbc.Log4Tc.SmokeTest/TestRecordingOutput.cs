using Mbc.Log4Tc.Model;
using Mbc.Log4Tc.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.SmokeTest
{
    public class TestRecordingOutput : OutputHandlerBase
    {
        private readonly List<LogEntry> loggedEntries = new List<LogEntry>();

        public string Name => "TestOutput";

        public List<LogEntry> LoggedEntries => loggedEntries;

        protected override Task ProcesLogEntryAsync(LogEntry logEntry)
        {
            loggedEntries.Add(logEntry);
            return Task.CompletedTask;
        }
    }
}
