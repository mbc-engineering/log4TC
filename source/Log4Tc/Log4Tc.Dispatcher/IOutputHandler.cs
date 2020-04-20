using Log4Tc.Model;

namespace Log4Tc.Dispatcher
{
    public interface IOutputHandler
    {
        string Name { get; }

        void ProcesLogEntry(LogEntry logEntry);
    }
}
