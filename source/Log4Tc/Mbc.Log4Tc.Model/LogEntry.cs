using System;
using System.Collections.Generic;
using System.Linq;

namespace Mbc.Log4Tc.Model
{
    public class LogEntry
    {
        private readonly Lazy<object[]> _argList;
        private readonly Lazy<string> _formattedMessage;
        private readonly Lazy<string[]> _argIndex;

        public LogEntry()
        {
            _argList = new Lazy<object[]>(CreateArgumentList);
            _formattedMessage = new Lazy<string>(CreateFormattedMessage);
            _argIndex = new Lazy<string[]>(CreateArgIndex);
        }

        public string Source { get; set; }

        public string Hostname { get; set; }

        public string Message { get; set; }

        public string Logger { get; set; }

        public LogLevel Level { get; set; }

        public DateTime PlcTimestamp { get; set; }

        public DateTime ClockTimestamp { get; set; }

        public int TaskIndex { get; set; }

        public string TaskName { get; set; }

        public uint TaskCycleCounter { get; set; }

        public string AppName { get; set; }

        public string ProjectName { get; set; }

        public uint OnlineChangeCount { get; set; }

        public IDictionary<int, object> Arguments { get; } = new Dictionary<int, object>();

        public IDictionary<string, object> Context { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Returns the <see cref="Message"/> formatted with the <see cref="Arguments"/>.
        /// </summary>
        public string FormattedMessage => _formattedMessage.Value;

        /// <summary>
        /// Returns all arguments in order and filled with <c>null</c> values.
        /// </summary>
        public IEnumerable<object> ArgumentEnumerable => _argList.Value;

        /// <summary>
        /// Returns the index (which might be the name or the numeric position) of
        /// the arguments.
        /// </summary>
        public IEnumerable<string> ArgumentIndex => _argIndex.Value;

        private object[] CreateArgumentList()
        {
            if (Arguments.Count == 0)
            {
                return new object[0];
            }

            var count = Arguments.Keys.Max();
            var argList = new object[count];

            for (int i = 0; i < count; ++i)
            {
                if (Arguments.TryGetValue(i + 1, out object value))
                {
                    argList[i] = value;
                }
            }

            return argList;
        }

        private string CreateFormattedMessage()
        {
            var parser = new MessageArgumentParser(Message);
            return parser.FormatMessage(ArgumentEnumerable);
        }

        private string[] CreateArgIndex()
        {
            var parser = new MessageArgumentParser(Message);
            return parser.ParseArguments().ToArray();
        }
    }
}
