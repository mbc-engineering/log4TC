using Optional;
using Optional.Collections;
using Optional.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mbc.Log4Tc.Model.Message
{
    /// <summary>
    /// Parses a message template with either positional or named placeholder between {/} brackets.
    /// </summary>
    public class MessageFormatter
    {
        private readonly List<MessageTemplateToken> _token = new List<MessageTemplateToken>();
        private readonly string _messageFormat;

        public MessageFormatter(string messageFormat)
        {
            _messageFormat = messageFormat;
            Parse();

            Arguments = _token.OfType<MessageHoleToken>().Select(x => x.Label).ToList();
            if (_token.OfType<MessageHoleToken>().All(x => x.Index.HasValue))
            {
                PositionalArguments = _token.OfType<MessageHoleToken>().Select(x => x.Index).Values().ToList().Some<IEnumerable<int>>();

            }
            else
            {
                PositionalArguments = Option.None<IEnumerable<int>>();
            }
        }

        private void Parse()
        {
            int idx = 0;
            StringBuilder text = new StringBuilder();

            while (idx < _messageFormat.Length)
            {
                char c = _messageFormat[idx];

                if (c == '{')
                {
                    if (PeekChar(idx) != '{')
                    {
                        // a hole starts -> save text before and parse hole
                        if (text.Length > 0)
                        {
                            _token.Add(new MessageTextToken(text.ToString()));
                            text.Clear();
                        }

                        MessageHoleToken hole = ParseHole(ref idx);
                        _token.Add(hole);
                    }
                    else
                    {
                        // escaped {
                        text.Append(c);
                        idx += 2;
                    }
                }
                else if (c == '}' && PeekChar(idx) == '}')
                {
                    // escaped }
                    text.Append(c);
                    idx += 2;
                }
                else
                {
                    text.Append(c);
                    idx++;
                }
            }

            if (text.Length > 0)
            {
                _token.Add(new MessageTextToken(text.ToString()));
            }
        }

        private char PeekChar(int idx) => (idx + 1) < _messageFormat.Length ? _messageFormat[idx + 1] : char.MinValue;

        private MessageHoleToken ParseHole(ref int idx)
        {
            if (_messageFormat[idx] != '{')
                throw new InvalidOperationException();

            idx++;

            string hole;
            var end = _messageFormat.IndexOf('}', idx);
            if (end > 0)
            {
                hole = _messageFormat.Substring(idx, end - idx);
                idx = end + 1;
            }
            else
            {
                hole = _messageFormat.Substring(idx);
                idx = _messageFormat.Length;
            }

            int startOfAlignment = hole.IndexOf(',');
            int startOfFormat = hole.IndexOf(':');
            int endOfLabel = hole.Length;

            string format = string.Empty;
            if (startOfFormat > 0)
            {
                format = hole.Substring(startOfFormat + 1);
                endOfLabel = startOfFormat;
            }

            int alignment = 0;
            if (startOfAlignment > 0)
            {
                if (!int.TryParse(hole.Substring(startOfAlignment + 1, endOfLabel - startOfAlignment - 1), out alignment))
                {
                    alignment = 0;
                }

                endOfLabel = startOfAlignment;
            }

            string label = hole.Substring(0, endOfLabel);

            return new MessageHoleToken(label, alignment, format);
        }

        /// <summary>
        /// Returns the arguments in order of the template.
        /// </summary>
        public IEnumerable<string> Arguments { get; }

        /// <summary>
        /// Returns the indicies of the arguments if the message template is positional,
        /// <c>Option.None</c> otherweise.
        /// </summary>
        public Option<IEnumerable<int>> PositionalArguments { get; }

        public string Format(IEnumerable<object> argumentValues)
        {
            if (PositionalArguments.HasValue)
            {
                return FormatPositional(argumentValues.ToList());
            }
            else
            {
                return FormatNamed(argumentValues);
            }
        }

        private string FormatNamed(IEnumerable<object> values)
        {
            var format = new StringBuilder(_messageFormat.Length + 128);

            IEnumerator<object> valuesEnumerator = values.GetEnumerator();
            foreach (var token in _token)
            {
                var text = token switch
                {
                    MessageTextToken textToken => textToken.Text,
                    MessageHoleToken holeToken => FormatHole(holeToken, valuesEnumerator.MoveNext() ? valuesEnumerator.Current : "?"),
                    _ => throw new NotImplementedException(),
                };

                format.Append(text);
            }

            return format.ToString();
        }

        private string FormatPositional(List<object> values)
        {
            var format = new StringBuilder(_messageFormat.Length + 128);

            foreach (var token in _token)
            {
                var text = token switch
                {
                    MessageTextToken textToken => textToken.Text,
                    MessageHoleToken holeToken => FormatHole(holeToken, holeToken.Index.Filter(x => values.Count > x).Match(x => values[x], () => "?")),
                    _ => throw new NotImplementedException(),
                };

                format.Append(text);
            }

            return format.ToString();
        }

        private string FormatHole(MessageHoleToken holeToken, object value)
        {
            return string.Format($"{{0,{holeToken.Alignment}:{holeToken.Format}}}", value);
        }
    }
}
