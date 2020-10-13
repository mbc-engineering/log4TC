using Mbc.Log4Tc.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output
{
    /// <summary>
    /// Basisklasse für alle Outputs. Je nach Art der Verarbeitung muss die ableitende Klasse
    /// <see cref="ProcesLogEntryAsync(LogEntry)"/> oder <see cref="ProcesLogEntriesAsync(IEnumerable{LogEntry})"/>
    /// überschreiben.
    /// </summary>
    public abstract class OutputHandlerBase
    {
        public async virtual Task ProcesLogEntriesAsync(IEnumerable<LogEntry> logEntries)
        {
            foreach (var logEntry in logEntries)
            {
                await ProcesLogEntryAsync(logEntry);
            }
        }

        protected virtual Task ProcesLogEntryAsync(LogEntry logEntry)
        {
            throw new NotImplementedException();
        }
    }
}
