using InfluxDB.Client;
using InfluxDB.Client.Writes;
using Mbc.Log4Tc.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output.InfluxDb
{
    public class InfluxDbOutput : IOutputHandler, IHostedService
    {
        private readonly InfluxDbOutputSettings _settings;
        private readonly ILogger<InfluxDbOutput> _logger;
        private InfluxDBClient _client;
        private WriteApi _writeApi;

        public InfluxDbOutput(string name, InfluxDbOutputSettings settings, ILogger<InfluxDbOutput> logger)
        {
            Name = name;
            _settings = settings;
            _logger = logger;
        }

        public string Name { get; }

        public Task ProcesLogEntry(LogEntry logEntry)
        {
            try
            {
                if (logEntry.Arguments.Count == 0)
                    return Task.CompletedTask;

                var point = PointData.Measurement(logEntry.Logger);

                // add tags from context
                foreach (var ctxProp in logEntry.Context)
                {
                    point.Tag(ctxProp.Key, Convert.ToString(ctxProp.Value, CultureInfo.InvariantCulture));
                }

                // add standard tags
                point.Tag("level", logEntry.Level.ToString());
                point.Tag("source", logEntry.Source);

                // add fields from all arguments
                var argParser = new MessageArgumentParser(logEntry.Message);
                var index = 0;
                foreach (var argName in argParser.ParseArguments())
                {
                    if (index >= logEntry.Arguments.Count)
                        break;

                    point.Field(argName, Convert.ToString(logEntry.Arguments[index], CultureInfo.InvariantCulture));
                    index++;
                }

                _writeApi.WritePoint(point);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending data to Influx.");
            }

            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client = InfluxDBClientFactory.CreateV1(_settings.Url, _settings.Username, _settings.Password, _settings.Database, _settings.RetentionPolicy);
            _writeApi = _client.GetWriteApi();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _writeApi?.Dispose();
            _writeApi = null;
            _client?.Dispose();
            _client = null;
            return Task.CompletedTask;
        }
    }
}
