using InfluxDB.Client.Writes;
using Mbc.Log4Tc.Model;

namespace Mbc.Log4Tc.Output.InfluxDb
{
    internal interface IInfluxPointFactory
    {
        PointData CreatePoint(LogEntry logEntry);
    }
}
