﻿using System.ComponentModel.DataAnnotations;
using NRedisStack;

namespace Mbc.Log4Tc.Output.Redis
{
    public class RedisOutputSettings
    {
        // TODO: Check which attributes should be here.
        [Required]
        [Url]
        public string Url { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [Required]
        public string Database { get; set; }

        public string RetentionPolicy { get; set; }

        public int WriteBatchSize { get; set; } = 20;

        public int WriteFlushIntervalMillis { get; set; } = 1000;

        public InfluxFormat Format { get; set; } = InfluxFormat.Arguments;

        public int SyslogFacilityCode { get; set; } = 16;

        public enum InfluxFormat
        {
            Arguments,
            Syslog,
        }
    }
}
