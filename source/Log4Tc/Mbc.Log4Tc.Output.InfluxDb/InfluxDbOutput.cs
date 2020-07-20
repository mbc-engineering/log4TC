using InfluxDB.Client;
using InfluxDB.Client.Writes;
using Mbc.Log4Tc.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output.InfluxDb
{
    /// <summary>
    /// Outputs message arguments to InfluxDB (>= 1.8).
    /// </summary>
    public class InfluxDbOutput : IOutputHandler, IDisposable
    {
        private readonly InfluxDbOutputSettings _settings;
        private readonly ILogger<InfluxDbOutput> _logger;
        private readonly IInfluxPointFactory _pointFactory;
        private InfluxDBClient _client;
        private WriteApi _writeApi;

        public InfluxDbOutput(InfluxDbOutputSettings settings, ILogger<InfluxDbOutput> logger)
        {
            _settings = settings;
            _logger = logger;

            _pointFactory = _settings.Format switch
            {
                InfluxDbOutputSettings.InfluxFormat.Arguments => new ArgumentPointFactory(),
                InfluxDbOutputSettings.InfluxFormat.Syslog => new SyslogPointFactory(_settings),
                _ => throw new ArgumentOutOfRangeException(nameof(settings)),
            };
        }

        public void Dispose()
        {
            _writeApi?.Flush();
            _writeApi?.Dispose();
            _writeApi = null;

            _client?.Dispose();
            _client = null;
        }

        public Task ProcesLogEntry(LogEntry logEntry)
        {
            Initialize();

            PointData point = _pointFactory.CreatePoint(logEntry);
            if (point != null)
            {
                _writeApi.WritePoint(point);
            }

            return Task.CompletedTask;
        }

        private void Initialize()
        {
            try
            {
                if (_client == null)
                {
                    char[] password = string.IsNullOrWhiteSpace(_settings.Password) ? null : _settings.Password.ToCharArray();
                    _client = InfluxDBClientFactory.CreateV1(_settings.Url, _settings.Username, password, _settings.Database, _settings.RetentionPolicy);
                }

                if (_writeApi == null)
                {
                    _writeApi = _client.GetWriteApi(new WriteOptions.Builder().BatchSize(_settings.WriteBatchSize).FlushInterval(_settings.WriteFlushIntervalMillis).Build());
                    _writeApi.EventHandler += OnWriteApiEvent;
                }
            }
            catch (Exception)
            {
                _writeApi?.Dispose();
                _writeApi = null;

                _client?.Dispose();
                _client = null;
                throw;
            }
        }

        private void OnWriteApiEvent(object sender, EventArgs e)
        {
            if (e is WriteErrorEvent writeError)
            {
                _logger.LogError(writeError.Exception, "Error writing to influxdb.");
            }
        }
    }
}
