using System.Collections.Generic;

namespace Mbc.Log4Tc.Output.InfluxDb
{
    public class InfluxDbOutputSettings
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public char[] Password { get; set; }
        public string Database { get; set; }
        public string RetentionPolicy { get; set; }
    }
}
