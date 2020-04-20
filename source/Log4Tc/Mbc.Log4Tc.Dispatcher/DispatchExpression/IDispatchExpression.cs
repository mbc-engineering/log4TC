using Mbc.Log4Tc.Model;

namespace Mbc.Log4Tc.Dispatcher.DispatchExpression
{
    public interface IDispatchExpression
    {
        bool ShouldDispatch(string name, LogEntry logEntry);
    }
}
