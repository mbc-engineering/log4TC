using Mbc.Log4Tc.Model;
using System.Collections.Generic;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.DataTypes;
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
        private readonly ConfigurationOptions _configuration;
        private ConnectionMultiplexer? _redisConnection;
        private IDatabase? _redisDb;
        private TimeSeriesCommands _timeSeriesCommands;

        public RedisOutput(ConfigurationOptions configuration)
        {
            _configuration = configuration;
            InitializeRedisClient();
        }

        public void Dispose()
        {
            _redisConnection?.Close();
            _redisConnection = null;
        }

        public override async Task ProcesLogEntriesAsync(IEnumerable<LogEntry> logEntries)
        {
            foreach (var logEntry in logEntries)
            {
                // TODO: process log entry and write to redis time series.
            }
        }

        private void InitializeRedisClient()
        {
            _redisConnection = ConnectionMultiplexer.Connect(_configuration);
            _redisDb = _redisConnection.GetDatabase();
            _timeSeriesCommands = _redisDb.TS();
        }
    }
}
