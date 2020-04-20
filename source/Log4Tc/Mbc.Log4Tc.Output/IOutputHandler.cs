using Log4Tc.Model;

namespace Log4Tc.Output
{
    public interface IOutputHandler
    {
        string Name { get; }

        void ProcesLogEntry(LogEntry logEntry);
    }
}
