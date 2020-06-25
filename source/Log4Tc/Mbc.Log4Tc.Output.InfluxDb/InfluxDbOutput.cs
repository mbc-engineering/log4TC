using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Mbc.Log4Tc.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
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
        private InfluxDBClient _client;
        private WriteApi _writeApi;

        public InfluxDbOutput(InfluxDbOutputSettings settings, ILogger<InfluxDbOutput> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public void Dispose()
        {
            _writeApi?.Dispose();
            _writeApi = null;

            _client?.Dispose();
            _client = null;
        }

        public Task ProcesLogEntry(LogEntry logEntry)
        {
            if (logEntry.Arguments.Count == 0)
                return Task.CompletedTask;

            Initialize();

            var point = PointData.Measurement(logEntry.Logger);

            point = point.Timestamp(logEntry.PlcTimestamp.ToUniversalTime(), WritePrecision.Us);

            // add tags from context
            foreach (var ctxProp in logEntry.Context)
            {
                point.Tag(ctxProp.Key, Convert.ToString(ctxProp.Value, CultureInfo.InvariantCulture));
            }

            // add standard tags
            point = point
                .Tag("level", logEntry.Level.ToString())
                .Tag("source", logEntry.Source);

            // add fields from all arguments
            var argParser = new MessageArgumentParser(logEntry.Message);
            var index = 1;
            foreach (var argName in argParser.ParseArguments())
            {
                if (index > logEntry.Arguments.Count)
                    break;

                point = WriteField(point, argName, logEntry.Arguments[index]);
                index++;
            }

            _writeApi.WritePoint(point);

            return Task.CompletedTask;
        }

        private PointData WriteField(PointData point, string name, object value)
        {
            if (value == null)
                return point;

            switch (value)
            {
                case bool boolValue:
                    return point.Field(name, boolValue);
                case byte byteValue:
                    return point.Field(name, byteValue);
                case sbyte sbyteValue:
                    return point.Field(name, sbyteValue);
                case short shortValue:
                    return point.Field(name, shortValue);
                case ushort ushortValue:
                    return point.Field(name, ushortValue);
                case int intValue:
                    return point.Field(name, intValue);
                case uint uintValue:
                    return point.Field(name, uintValue);
                case long longValue:
                    return point.Field(name, longValue);
                case ulong ulongValue:
                    return point.Field(name, ulongValue);
                case float floatValue:
                    return point.Field(name, floatValue);
                case double doubleValue:
                    return point.Field(name, doubleValue);
                case string stringValue:
                    return point.Field(name, stringValue);
                default:
                    return point;
            }
        }

        private void Initialize()
        {
            try
            {
                if (_client == null)
                {
                    _client = InfluxDBClientFactory.CreateV1(_settings.Url, _settings.Username, _settings.Password, _settings.Database, _settings.RetentionPolicy);
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
