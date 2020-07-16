using InfluxDB.Client.Writes;
using Mbc.Log4Tc.Model;
using System;
using System.Globalization;

namespace Mbc.Log4Tc.Output.InfluxDb
{
    internal abstract class BasePointFactory : IInfluxPointFactory
    {
        public abstract PointData CreatePoint(LogEntry logEntry);

        protected virtual PointData WriteTag(PointData point, string name, object value)
        {
            return point.Tag(name, Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        protected PointData WriteContextToTags(PointData point, LogEntry logEntry)
        {
            foreach (var ctxProp in logEntry.Context)
            {
                point = WriteTag(point, ctxProp.Key, ctxProp.Value);
            }

            return point;
        }

        protected virtual PointData WriteField(PointData point, string name, object value)
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
    }
}
