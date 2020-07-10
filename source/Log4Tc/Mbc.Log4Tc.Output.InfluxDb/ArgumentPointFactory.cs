using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Mbc.Log4Tc.Model;
using System;
using System.Globalization;

namespace Mbc.Log4Tc.Output.InfluxDb
{
    internal class ArgumentPointFactory : BasePointFactory
    {
        public override PointData CreatePoint(LogEntry logEntry)
        {
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

            // add fields from all arguments
            point = WriteArgumentsToFields(point, logEntry);

            return point;
        }
    }
}
