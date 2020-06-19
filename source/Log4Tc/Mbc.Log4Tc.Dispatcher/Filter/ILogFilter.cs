using Mbc.Log4Tc.Model;

namespace Mbc.Log4Tc.Dispatcher.Filter
{
    /// <summary>
    /// Filter implementation for <see cref="LogEntry"/>.
    /// </summary>
    public interface ILogFilter
    {
        /// <summary>
        /// Checks if this filter matches the given <paramref name="logEntry"/>.
        /// </summary>
        /// <param name="logEntry">The log entry for which the filter should be applied.</param>
        /// <returns><c>true</c> if this filter matches the <paramref name="logEntry"/>, otherweise <c>false</c></returns>
        bool Matches(LogEntry logEntry);
    }
}
