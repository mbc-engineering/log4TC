using Log4Tc.Model;

namespace Log4Tc.Dispatcher.DispatchExpression
{
    public interface IDispatchExpression
    {
        bool ShouldDispatch(string name, LogEntry logEntry);
    }
}
