using Mbc.Log4Tc.Model;

namespace Mbc.Log4Tc.Dispatcher.DispatchExpression
{
    public sealed class DispatchAllLogsToOutput : IDispatchExpression
    {
        private readonly string _outputName;

        public DispatchAllLogsToOutput(string outputName)
        {
            _outputName = outputName;
        }

        public bool ShouldDispatch(string name, LogEntry logEntry) => name == _outputName;
    }
}
