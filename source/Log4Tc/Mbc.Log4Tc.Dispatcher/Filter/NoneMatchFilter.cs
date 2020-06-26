using Mbc.Log4Tc.Model;

namespace Mbc.Log4Tc.Dispatcher.Filter
{
    /// <summary>
    /// A <see cref="ILogFilter"/> which matches no <see cref="LogEntry"/>.
    /// </summary>
    public class NoneMatchFilter : ILogFilter
    {
        public static readonly NoneMatchFilter Default = new NoneMatchFilter();

        public bool Matches(LogEntry logEntry) => false;

        public override string ToString() => "Filter(none)";
    }
}
