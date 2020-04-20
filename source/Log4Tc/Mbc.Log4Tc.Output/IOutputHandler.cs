using Mbc.Log4Tc.Model;

namespace Mbc.Log4Tc.Output
{
    public interface IOutputHandler
    {
        string Name { get; }

        void ProcesLogEntry(LogEntry logEntry);
    }
}
