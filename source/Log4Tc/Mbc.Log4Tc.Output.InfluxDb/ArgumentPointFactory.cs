using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Mbc.Log4Tc.Model;
using System;
using System.Globalization;
using System.Linq;

namespace Mbc.Log4Tc.Output.InfluxDb
{
    internal class ArgumentPointFactory : BasePointFactory
    {
        public override PointData CreatePoint(LogEntry logEntry)
        {
            // only log if arguments are present and named arguments are used.
            if (logEntry.Arguments.Count == 0 || logEntry.MessageFormatter.PositionalArguments.HasValue)
                return null;

            var point = PointData.Measurement(logEntry.Logger);

            point = point.Timestamp(logEntry.PlcTimestamp.ToUniversalTime(), WritePrecision.Us);

            // add tags from context
            point = WriteContextToTags(point, logEntry);

            // add standard tags
            point = point
                .Tag("level", logEntry.Level.ToString())
                .Tag("source", logEntry.Source)
                .Tag("hostname", logEntry.Hostname)
                .Tag("taskName", logEntry.TaskName)
                .Tag("taskIndex", Convert.ToString(logEntry.TaskIndex, CultureInfo.InvariantCulture))
                .Tag("appName", logEntry.AppName)
                .Tag("projectName", logEntry.ProjectName);

            // add fields from all arguments if named arguments are used
            foreach ((string name, object value) in logEntry.MessageFormatter.Arguments.Zip(logEntry.ArgumentValues, (x, y) => (x, y)))
            {
                point = WriteField(point, name, value);
            }

            return point;
        }
    }
}
