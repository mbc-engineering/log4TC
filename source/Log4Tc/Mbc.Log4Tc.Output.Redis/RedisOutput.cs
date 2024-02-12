using Mbc.Log4Tc.Model;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.DataTypes;


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
            // _redisConnection?.Close();
            _redisConnection = null;
        }

        public override async Task ProcesLogEntriesAsync(IEnumerable<LogEntry> logEntries)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();
            var ts = db.TS();
            // ts.Create(logEntries.First().Logger);
            TimeSeriesLabel[] labels;
            foreach (var logEntry in logEntries)
            {
                labels =
                new TimeSeriesLabel[]
                {
                    new TimeSeriesLabel("source", logEntry.Source ?? string.Empty),
                    new TimeSeriesLabel("hostname", logEntry.Hostname ?? string.Empty),
                    new TimeSeriesLabel("logger", logEntry.Logger ?? string.Empty),
                    new TimeSeriesLabel("level", logEntry.Level.ToString()),
                    new TimeSeriesLabel("appName", logEntry.AppName ?? string.Empty),
                    new TimeSeriesLabel("projectName", logEntry.ProjectName ?? string.Empty),
                    new TimeSeriesLabel("taskName", logEntry.TaskName ?? string.Empty),
                    new TimeSeriesLabel("taskIndex", logEntry.TaskIndex.ToString()),
                    new TimeSeriesLabel("taskCycleCounter", logEntry.TaskCycleCounter.ToString()),
                    new TimeSeriesLabel("onlineChangeCount", logEntry.OnlineChangeCount.ToString()),
                };
                await ts.AddAsync(logEntry.Logger, logEntry.PlcTimestamp, 1.1, labels: labels);
            }
        }

        private void InitializeRedisClient()
        {
            this._redisConnection = ConnectionMultiplexer.Connect(this._configuration);
            this._redisDb = this._redisConnection.GetDatabase();
            this._timeSeriesCommands = this._redisDb.TS();
        }
    }
}
