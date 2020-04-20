using System;

namespace Mbc.Log4Tc.Receiver
{
    public interface ILogReceiver
    {
        event EventHandler<LogEntryEventArgs> LogsReceived;
    }
}
