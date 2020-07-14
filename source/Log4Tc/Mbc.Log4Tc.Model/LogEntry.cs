using System;
using System.Collections.Generic;
using System.Linq;

namespace Mbc.Log4Tc.Model
{
    public class LogEntry
    {
        private readonly Lazy<object[]> _argValues;
        private readonly Lazy<string> _formattedMessage;
        private readonly Lazy<string[]> _argLabels;
        private readonly Lazy<List<(string, object)>> _argPairs;

        public LogEntry()
        {
            _argValues = new Lazy<object[]>(CreateArgumentList);
            _formattedMessage = new Lazy<string>(CreateFormattedMessage);
            _argLabels = new Lazy<string[]>(CreateArgIndex);
            _argPairs = new Lazy<List<(string, object)>>(CreateArgumentPairs);
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
        public IEnumerable<object> ArgumentValues => _argValues.Value;

        /// <summary>
        /// Returns the label (which might be the name or the numeric position) of
        /// the arguments in order of the message.
        /// </summary>
        public IEnumerable<string> ArgumentLabels => _argLabels.Value;

        public IEnumerable<(string, object)> ArgumentPairs => _argPairs.Value;

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

        private List<(string, object)> CreateArgumentPairs()
        {
            var labels = ArgumentLabels.ToList();
            var values = ArgumentValues.ToList();

            if (labels.All(x => int.TryParse(x, out int _)))
            {
                // all labels are numeric => use numeric semantic
                var pairs = new List<(string, object)>(labels.Count);
                foreach (var label in labels)
                {
                    var idx = int.Parse(label);
                    if (idx >= 0 && idx < values.Count)
                    {
                        pairs.Add((label, values[idx]));
                    }
                    else
                    {
                        pairs.Add((label, "?"));
                    }
                }

                return pairs;
            }
            else
            {
                // structured semantic
                return labels.Zip(values, (label, value) => (label, value)).ToList();
            }
        }

        private string CreateFormattedMessage()
        {
            var parser = new MessageArgumentParser(Message);
            return parser.FormatMessage(ArgumentValues);
        }

        private string[] CreateArgIndex()
        {
            var parser = new MessageArgumentParser(Message);
            return parser.ParseArguments().ToArray();
        }
    }
}
