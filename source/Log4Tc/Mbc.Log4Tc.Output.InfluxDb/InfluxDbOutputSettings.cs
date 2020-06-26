using System.ComponentModel.DataAnnotations;

namespace Mbc.Log4Tc.Output.InfluxDb
{
    public class InfluxDbOutputSettings
    {
        [Required]
        [Url]
        public string Url { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [Required]
        public string Database { get; set; }

        public string RetentionPolicy { get; set; }

        public int WriteBatchSize { get; set; } = 1;

        public int WriteFlushIntervalMillis { get; set; } = 1000;
    }
}
