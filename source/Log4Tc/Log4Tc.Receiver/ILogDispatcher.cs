using System;

namespace Log4Tc.Receiver
{
    public interface ILogDispatcher
    {
        event EventHandler<LogEntryEventArgs> DispatchLogEntry;
    }
}
