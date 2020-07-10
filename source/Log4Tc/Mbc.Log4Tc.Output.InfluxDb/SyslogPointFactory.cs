using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Mbc.Log4Tc.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Log4TcLevel = Mbc.Log4Tc.Model.LogLevel;

namespace Mbc.Log4Tc.Output.InfluxDb
{
    internal class SyslogPointFactory : BasePointFactory
    {
        private static readonly IReadOnlyList<string> ReservedTags = new List<string>()
        {
            "appname", "facility", "host", "hostname", "severity",
        }.AsReadOnly();

        private static readonly IReadOnlyList<string> ReservedFields = new List<string>()
        {
            "facility_code", "message", "procid", "severity_code", "timestamp", "version",
        }.AsReadOnly();

        private readonly InfluxDbOutputSettings _settings;

        public SyslogPointFactory(InfluxDbOutputSettings settings)
        {
            _settings = settings;
        }

        protected override PointData WriteTag(PointData point, string name, object value)
        {
            if (ReservedTags.Contains(name))
            {
                name = "_" + name;
            }

            return base.WriteTag(point, name, value);
        }

        protected override PointData WriteField(PointData point, string name, object value)
        {
            if (ReservedFields.Contains(name))
            {
                name = "_" + name;
            }

            return base.WriteField(point, name, value);
        }

        public override PointData CreatePoint(LogEntry logEntry)
        {
            var point = PointData.Measurement("syslog");

            point = point.Timestamp(logEntry.PlcTimestamp.ToUniversalTime(), WritePrecision.Us);

            // add tags from context
            point = WriteContextToTags(point, logEntry);

            // add log4tc tags
            point = point
                .Tag("level", logEntry.Level.ToString())
                .Tag("source", logEntry.Source)
                .Tag("taskName", logEntry.TaskName)
                .Tag("taskIndex", Convert.ToString(logEntry.TaskIndex, CultureInfo.InvariantCulture));

            // Syslog Tags
            point = point
                .Tag("appname", logEntry.ProjectName)
                .Tag("facility", logEntry.AppName)
                .Tag("host", logEntry.Source)
                .Tag("hostname", logEntry.Hostname)
                .Tag("severity", LevelToSyslogSeverity(logEntry.Level));

            // add fields from all arguments
            point = WriteArgumentsToFields(point, logEntry);

            // Syslog Fields
            point = point
                .Field("facility_code", _settings.SyslogFacilityCode)
                .Field("message", logEntry.FormattedMessage)
                .Field("procid", logEntry.OnlineChangeCount.ToString())
                .Field("severity_code", LevelToSyslogSeverityCode(logEntry.Level))
                .Field("timestamp", new DateTimeOffset(logEntry.PlcTimestamp).ToUnixTimeMilliseconds() * 1000000)
                .Field("version", 1);

            return point;
        }

        private string LevelToSyslogSeverity(Log4TcLevel level) => level switch
        {
            Log4TcLevel.Trace => "debug",
            Log4TcLevel.Debug => "debug",
            Log4TcLevel.Info => "info",
            Log4TcLevel.Warn => "warning",
            Log4TcLevel.Error => "err",
            Log4TcLevel.Fatal => "emergency",
            _ => "emerg",
        };

        private int LevelToSyslogSeverityCode(Log4TcLevel level) => level switch
        {
            Log4TcLevel.Trace => 7,
            Log4TcLevel.Debug => 7,
            Log4TcLevel.Info => 6,
            Log4TcLevel.Warn => 4,
            Log4TcLevel.Error => 3,
            Log4TcLevel.Fatal => 0,
            _ => 0,
        };
    }
}
