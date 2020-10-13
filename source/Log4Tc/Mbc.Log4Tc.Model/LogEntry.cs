using Mbc.Log4Tc.Model.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mbc.Log4Tc.Model
{
    public class LogEntry
    {
        private readonly Lazy<MessageFormatter> _messageFormatter;
        private readonly Lazy<object[]> _argValues;
        private readonly Lazy<string> _formattedMessage;

        public LogEntry()
        {
            _messageFormatter = new Lazy<MessageFormatter>(() => new MessageFormatter(Message), isThreadSafe: true);
            _argValues = new Lazy<object[]>(CreateArgumentList, isThreadSafe: true);
            _formattedMessage = new Lazy<string>(CreateFormattedMessage, isThreadSafe: true);
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

        public MessageFormatter MessageFormatter => _messageFormatter.Value;

        /// <summary>
        /// Returns the <see cref="Message"/> formatted with the <see cref="Arguments"/>. Usage
        /// of this property is preferred to <c>MessageFormatter.Format(ArgumentValues></c> because
        /// the formatted is cached in this instance.
        /// </summary>
        public string FormattedMessage => _formattedMessage.Value;

        /// <summary>
        /// Returns all arguments in order and filled with <c>null</c> values.
        /// </summary>
        public IEnumerable<object> ArgumentValues => _argValues.Value;

        private object[] CreateArgumentList()
        {
            if (Arguments.Count == 0)
            {
                return Array.Empty<object>();
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

        private string CreateFormattedMessage() => _messageFormatter.Value.Format(ArgumentValues);
    }
}
