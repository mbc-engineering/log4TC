using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mbc.Log4Tc.Output
{
    public static class OutputAliases
    {
        private static Dictionary<string, Type> _knownOutputAliases = new Dictionary<string, Type>();

        public static ReadOnlyDictionary<string, Type> KnownOutputAliases => new ReadOnlyDictionary<string, Type>(_knownOutputAliases);

        public static void AddKnownOutputAlias(string alias, Type fqnOutputFactory)
        {
            if (_knownOutputAliases.ContainsKey(alias))
            {
                _knownOutputAliases[alias] = fqnOutputFactory;
            }
            else
            {
            _knownOutputAliases.Add(alias, fqnOutputFactory);
            }
        }
    }
}
