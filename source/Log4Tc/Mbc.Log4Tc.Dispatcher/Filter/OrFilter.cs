using Mbc.Log4Tc.Model;
using System.Collections.Generic;
using System.Linq;

namespace Mbc.Log4Tc.Dispatcher.Filter
{
    /// <summary>
    /// Implements a <see cref="ILogFilter"/> which combines
    /// multiple <see cref="ILogFilter"/> with a or operation.
    /// </summary>
    public class OrFilter : ILogFilter
    {
        private readonly IReadOnlyList<ILogFilter> _filter;

        internal IEnumerable<ILogFilter> Filter => _filter;

        public OrFilter(IEnumerable<ILogFilter> filter)
        {
            _filter = new List<ILogFilter>(filter).AsReadOnly();
        }

        public bool Matches(LogEntry logEntry)
        {
            return _filter.All(x => x.Matches(logEntry));
        }

        public override string ToString()
        {
            return $"Or({string.Join(",", _filter.Select(x => x.ToString()))})";
        }
    }
}
