using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

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
    }
}
