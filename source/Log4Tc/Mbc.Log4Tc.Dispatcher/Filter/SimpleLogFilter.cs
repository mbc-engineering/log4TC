using Mbc.Log4Tc.Model;
using Optional;
using System;
using System.Text;

namespace Mbc.Log4Tc.Dispatcher.Filter
{
    /// <summary>
    /// A <see cref="ILogFilter"/> implementation which provides simple filtering on
    /// common <see cref="LogEntry"/> properties.
    /// </summary>
    public class SimpleLogFilter : ILogFilter
    {
        private readonly LogLevel _minLevel;
        private readonly Func<LogEntry, bool> _loggerMatcher;

        public SimpleLogFilter(Option<LogLevel> level = default, Option<string> logger = default)
        {
            Level = level;
            Logger = logger;

            _minLevel = level.ValueOr(LogLevel.Trace);
            _loggerMatcher = logger.Match(
                some: x =>
                {
                    if (x.StartsWith("*") && x.EndsWith("*"))
                    {
                        return (LogEntry log) => log.Logger.Contains(x.Substring(1, x.Length - 2));
                    }
                    else if (x.EndsWith("*"))
                    {
                        return (LogEntry log) => log.Logger.StartsWith(x.Substring(0, x.Length - 1));
                    }
                    else if (x.StartsWith("*"))
                    {
                        return (LogEntry log) => log.Logger.EndsWith(x.Substring(1));
                    }
                    else
                    {
                        return (Func<LogEntry, bool>)((LogEntry log) => log.Logger == x);
                    }
                },
                none: () => { return (LogEntry log) => true; });
        }

        public Option<LogLevel> Level { get; }

        public Option<string> Logger { get; }

        public bool Matches(LogEntry logEntry)
        {
            return logEntry.Level >= _minLevel
                && _loggerMatcher(logEntry);
        }

        public override string ToString()
        {
            var str = new StringBuilder("Filter(");
            Level.MatchSome(x => str.Append($"Level >= {x}"));

            if (Level.HasValue && Logger.HasValue)
                str.Append(" && ");

            Logger.MatchSome(x => str.Append($"Logger == {x}"));

            str.Append(")");
            return str.ToString();
        }
    }
}
