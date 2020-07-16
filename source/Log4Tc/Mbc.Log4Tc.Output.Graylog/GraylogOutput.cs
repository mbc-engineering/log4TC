using Mbc.Log4Tc.Model;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output.Graylog
{
    /// <summary>
    /// Writes LogEntry to Graylog via UDP/Gelf.
    /// </summary>
    internal class GraylogOutput : IOutputHandler, IDisposable
    {
        private const int MaxCachedStreamSize = 8192;

        private readonly byte[] _buffer = new byte[MaxCachedStreamSize];
        private readonly GraylogOutputSettings _settings;
        private UdpClient _udpClient;

        public GraylogOutput(GraylogOutputSettings settings)
        {
            _settings = settings;
        }

        public void Dispose()
        {
            _udpClient?.Dispose();
            _udpClient = null;
        }

        public async Task ProcesLogEntry(LogEntry logEntry)
        {
            var gelf = CreateGelf(logEntry);

            Initialize();

            var json = gelf.ToJson();
            var length = Encoding.UTF8.GetBytes(json, 0, json.Length, _buffer, 0);

            if (_settings.GelfCompression == GraylogOutputSettings.GelfCompressionType.Gzip)
            {
                var source = new MemoryStream(_buffer, 0, length);
                var dest = new MemoryStream(length);
                using (var gzipStream = new GZipStream(dest, CompressionLevel.Optimal, true))
                {
                    source.CopyTo(gzipStream);
                }

                await _udpClient.SendAsync(dest.GetBuffer(), (int)dest.Length);
            }
            else
            {
                // uncompressed
                await _udpClient.SendAsync(_buffer, length);
            }
        }

        private GelfData CreateGelf(LogEntry logEntry)
        {
            var gelf = new GelfData
            {
                Host = logEntry.Source,
                ShortMessage = logEntry.FormattedMessage,
                FullMessage = logEntry.Message,
                Level = ToGelfLevel(logEntry.Level),
                Timestamp = ToGelfTimeStamp(logEntry.PlcTimestamp),
            };

            // add context
            gelf.Add("taskName", logEntry.TaskName);
            gelf.Add("taskIndex", logEntry.TaskIndex);
            gelf.Add("taskCycleCounter", logEntry.TaskCycleCounter);
            gelf.Add("appName", logEntry.AppName);
            gelf.Add("projectName", logEntry.ProjectName);
            gelf.Add("logger", logEntry.Logger);
            gelf.Add("clockTimestamp", logEntry.ClockTimestamp.ToString("o"));
            gelf.Add("onlineChangeCount", logEntry.OnlineChangeCount);

            foreach (var ctx in logEntry.Context)
            {
                gelf.Add(ctx.Key, ctx.Value);
            }

            // add arguments
            foreach ((string name, object value) in logEntry.MessageFormatter.Arguments.Zip(logEntry.ArgumentValues, (x, y) => (x, y)))
            {
                gelf.Add(name, value);
            }

            return gelf;
        }

        private decimal ToGelfTimeStamp(DateTime plcTimestamp) => new decimal(new DateTimeOffset(plcTimestamp.ToUniversalTime()).ToUnixTimeMilliseconds()) / new decimal(1000);

        private int ToGelfLevel(LogLevel level) => level switch
        {
            LogLevel.Trace => 7,
            LogLevel.Debug => 7,
            LogLevel.Info => 6,
            LogLevel.Warn => 4,
            LogLevel.Error => 3,
            LogLevel.Fatal => 2,
            _ => 0,
        };

        private void Initialize()
        {
            try
            {
                if (_udpClient == null)
                {
                    _udpClient = new UdpClient(_settings.GraylogHostname, _settings.GraylogPort);
                }
            }
            catch (Exception)
            {
                _udpClient?.Dispose();
                _udpClient = null;
                throw;
            }
        }
    }
}
