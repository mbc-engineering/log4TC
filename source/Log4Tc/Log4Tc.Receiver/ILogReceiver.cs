using System;

namespace Log4Tc.Receiver
{
    public interface ILogReceiver
    {
        event EventHandler<LogEntryEventArgs> LogsReceived;
    }
}
