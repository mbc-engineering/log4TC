using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Text;
using System.Globalization;

namespace Mbc.Log4Tc.Model
{
    /// <summary>
    /// Helper class for parsing message templates (format string).
    /// </summary>
    internal class MessageArgumentParser
    {
        private static readonly Regex ArgPattern = new Regex(@"{(?<name>\w+)}");
        private readonly string _messageTemplate;

        public MessageArgumentParser(string messageTemplate)
        {
            _messageTemplate = messageTemplate;
        }

        /// <summary>
        /// Returns the name of all arguments from the message template.
        /// </summary>
        public IEnumerable<string> ParseArguments()
        {
            // geschützte {{ und }} werden einfachhalber entfernt, da diese für die Argumente keine Rolle spielen
            var msg = _messageTemplate.Replace("{{", string.Empty).Replace("}}", string.Empty);

            var matches = ArgPattern.Matches(msg);
            return matches.Cast<Match>().Select(x => x.Groups["name"].Value);
        }

        public string FormatMessage(IEnumerable<object> args)
        {
            var argList = args.ToList();
            var formatted = new StringBuilder(_messageTemplate.Length + 128);
            var argId = new StringBuilder();

            bool inArg = false;
            int idx = 0;

            for (int i = 0; i < _messageTemplate.Length; i++)
            {
                char c = _messageTemplate[i];

                if (c == '{' || c == '}')
                {
                    if ((i + 1) < _messageTemplate.Length && !inArg)
                    {
                        if (_messageTemplate[i + 1] == c)
                        {
                            formatted.Append(c);
                            i++;
                            continue;
                        }
                    }

                    if (c == '{')
                    {
                        if (!inArg)
                        {
                            inArg = true;
                            continue;
                        }
                    }

                    if (c == '}' && inArg)
                    {
                        formatted.Append(GetArgumentString(argId.ToString(), idx, argList));

                        idx++;
                        inArg = false;
                        argId.Clear();
                        continue;
                    }
                }

                if (inArg)
                {
                    argId.Append(c);
                }
                else
                {
                    formatted.Append(c);
                }
            }

            return formatted.ToString();
        }

        private string GetArgumentString(string arg, int idx, IList<object> argumentList)
        {
            string argIndex = arg;
            int align = 0;
            string format = null;

            var pos = arg.IndexOfAny(new[] { ',', ':' });
            if (pos > 0)
            {
                // formattiertes oder ausgerichtetes Argument => Zerlegen in Einzelteile
                argIndex = arg.Substring(0, pos);

                if (arg[pos] == ',')
                {
                    var end = arg.IndexOf(':', pos);
                    if (end == -1)
                        end = arg.Length;

                    if (!int.TryParse(arg.Substring(pos + 1, end - pos - 1), out align))
                    {
                        align = 0;
                    }

                    pos = end;
                }

                if (pos < arg.Length && arg[pos] == ':')
                {
                    format = arg.Substring(pos + 1);
                }
            }

            // numeric argument?
            if (!int.TryParse(argIndex, out int indexOfArgument))
            {
                indexOfArgument = idx;
            }

            if (indexOfArgument >= 0 && indexOfArgument < argumentList.Count)
            {
                object argValue = argumentList[indexOfArgument];

                if (align == 0 && format == null)
                {
                    return Convert.ToString(argValue, CultureInfo.InvariantCulture);
                }
                else
                {
                    return string.Format($"{{0,{align}:{format}}}", argValue);
                }
            }
            else
            {
                return "?";
            }
        }
    }
}
