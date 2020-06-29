using System.Collections.Generic;
using System.Text.Json;

namespace Mbc.Log4Tc.Output.Graylog
{
    /// <summary>
    /// Represent GELF data and transform it into its JSON representation.
    /// </summary>
    internal class GelfData
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public GelfData()
        {
            _data.Add("version", "1.1");
        }

        public string Host
        {
            get => (string)_data["host"];
            set => _data["host"] = value;
        }

        public string ShortMessage
        {
            get => (string)_data["short_message"];
            set => _data["short_message"] = value;
        }

        public string FullMessage
        {
            get => (string)_data["full_message"];
            set => _data["full_message"] = value;
        }

        public decimal Timestamp
        {
            get => (decimal)_data["timestamp"];
            set => _data["timestamp"] = value;
        }

        public int Level
        {
            get => (int)_data["level"];
            set => _data["level"] = value;
        }

        public void Add(string name, object value)
        {
            if (!name.StartsWith("_"))
                name = "_" + name;

            _data[name] = value;
        }

        public string ToJson() => JsonSerializer.Serialize(_data);
    }
}
