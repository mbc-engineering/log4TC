using Mbc.Log4Tc.Model;
using NRedisStack;
using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Mbc.Log4Tc.Output.Redis
{
    /// <summary>
    /// Outputs log entries to Redis.
    /// </summary>
    public class RedisOutput : OutputHandlerBase, IDisposable
    {
        private readonly ConfigurationOptions _configuration; // TODO: check if this should be configurationOptions
        private IDatabase _redisClient;

        public RedisOutput(ConfigurationOptions configuration)
        {
            ConfigurationOptions configuration;
            InitializeRedisClient();
        }

        public void Dispose()
        {
            _redisClient?.Dispose();
            _redisClient = null;
        }

        protected override async Task ProcesLogEntriesAsync(IEnumerable<LogEntry> logEntries)
        {
            var key = $"log:{logEntry.Logger}:{logEntry.Level}";
            var timestamp = new TimeStamp(logEntry.PlcTimestamp.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond);

            var labels = new TimeSeriesLabel[]
            {
            new TimeSeriesLabel("Source", logEntry.Source),
            new TimeSeriesLabel("Hostname", logEntry.Hostname),
                // ... other labels like TaskName, AppName, etc.
            };

            var value = logEntry.FormattedMessage; // or any other numerical value relevant to your log

            await timeSeriesClient.AddAsync(key, timestamp, value, labels, duplicatePolicy: TsDuplicatePolicy.LAST);
        }
    }

    private void InitializeRedisClient()
    {
        _redisClient = ConnectionMultiplexer.Connect(_configuration);
    }
}
