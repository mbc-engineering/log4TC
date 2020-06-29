using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Text;

namespace Mbc.Log4Tc.Model
{
    /// <summary>
    /// Helper class for parsing message templates (format string).
    /// </summary>
    public class MessageArgumentParser
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

        public string ReplaceArguments(Func<string, int, string> replacement)
        {
            var formatted = new StringBuilder(_messageTemplate.Length);
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
                        if (_messageTemplate[i+1] == c)
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
                        formatted.Append(replacement(argId.ToString(), idx));
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
    }
}
