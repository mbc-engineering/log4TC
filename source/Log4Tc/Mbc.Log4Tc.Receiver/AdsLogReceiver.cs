using Mbc.Log4Tc.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TwinCAT.Ads;
using TwinCAT.Ads.Server;

using LogLevel = Mbc.Log4Tc.Model.LogLevel;

namespace Mbc.Log4Tc.Receiver
{
    public class AdsLogReceiver : TcAdsServer, ILogReceiver
    {
        private readonly ILogger<AdsLogReceiver> _logger;

        public event EventHandler<LogEntryEventArgs> LogsReceived;

        public AdsLogReceiver(ILogger<AdsLogReceiver> logger)
            : base(16150, "Log4Tc")
        {
            _logger = logger;
        }

        public override void AdsWriteInd(AmsAddress rAddr, uint invokeId, uint indexGroup, uint indexOffset, uint cbLength, byte[] data)
        {
            // send response as soon as possible
            AdsWriteRes(rAddr, invokeId, AdsErrorCode.NoError);

            try
            {
                var entries = new List<LogEntry>();
                var reader = new AdsBinaryReader(new AdsStream(data));

                while (reader.BaseStream.Length > reader.BaseStream.Position)
                {
                    LogEntry logEntry;

                    var version = reader.ReadByte();
                    if (version == 1)
                    {
                        logEntry = ReadLogEntryV1(reader);
                    }
                    else
                    {
                        throw new NotImplementedException($"Version {version}");
                    }

                    logEntry.Source = rAddr.ToString();

                    entries.Add(logEntry);
                }

                LogsReceived?.Invoke(this, new LogEntryEventArgs(entries));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing log message from plc.");
            }
        }

        private LogEntry ReadLogEntryV1(AdsBinaryReader reader)
        {
            var logEntry = new LogEntry
            {
                Message = ReadString(reader),
                Logger = ReadString(reader),
                Level = ReadLogLevel(reader),
                PlcTimestamp = ReadFiletime(reader),
                ClockTimestamp = ReadFiletime(reader),
                TaskIndex = reader.ReadInt32(),
                TaskName = ReadString(reader),
                TaskCycleCounter = reader.ReadUInt32(),
                AppName = ReadString(reader),
                ProjectName = ReadString(reader),
                OnlineChangeCount = reader.ReadUInt32(),
            };

            var end = false;
            while (!end)
            {
                var type = reader.ReadByte();
                switch (type)
                {
                    case 1:
                        var argIdx = (int)reader.ReadByte();
                        var argValue = ReadObject(reader);
                        logEntry.Arguments.Add(argIdx, argValue);
                        break;
                    case 2:
                        // context
                        var ctxScope = reader.ReadByte(); // TODO
                        var ctxName = ReadString(reader);
                        var ctxValue = ReadObject(reader);
                        logEntry.Context[ctxName] = ctxValue;
                        break;
                    case 255:
                        end = true;
                        break;
                    default:
                        throw new NotImplementedException($"Type {type}");
                }
            }

            return logEntry;
        }

        private DateTime ReadFiletime(AdsBinaryReader reader)
        {
            var filetime = reader.ReadInt64();
            try
            {
                return DateTime.FromFileTime(filetime);
            }
            catch (ArgumentException)
            {
                return new DateTime(0);
            }
        }

        private object ReadObject(AdsBinaryReader reader)
        {
            var type = reader.ReadInt16();
            object value;
            switch (type)
            {
                case 0:
                    value = null;
                    break;
                case 1: // BYTE
                    value = reader.ReadByte();
                    break;
                case 2: // WORD
                    value = reader.ReadUInt16();
                    break;
                case 3: // DWORD
                    value = reader.ReadUInt32();
                    break;
                case 4: // REAL
                    value = reader.ReadSingle();
                    break;
                case 5: // LREAL
                    value = reader.ReadSingle();
                    break;
                case 6: // SINT
                    value = reader.ReadSByte();
                    break;
                case 7: // INT
                    value = reader.ReadInt16();
                    break;
                case 8: // DINT
                    value = reader.ReadInt32();
                    break;
                case 9: // USINT
                    value = reader.ReadByte();
                    break;
                case 10: // UINT
                    value = reader.ReadUInt16();
                    break;
                case 11: // UDINT
                    value = reader.ReadUInt32();
                    break;
                case 12: // String
                    value = ReadString(reader);
                    break;
                case 13: // BOOL
                    value = reader.ReadByte() != 0;
                    break;
                case 15: // ULARGE
                    value = reader.ReadUInt64();
                    break;
                case 17: // LARGE
                    value = reader.ReadInt64();
                    break;
                default:
                    throw new NotImplementedException($"type {type}");
            }

            return value;
        }

        private string ReadString(AdsBinaryReader reader)
        {
            var len = reader.ReadByte();
            var data = reader.ReadBytes(len);
            return Encoding.GetEncoding(1252).GetString(data);
        }

        private LogLevel ReadLogLevel(AdsBinaryReader reader)
        {
            var value = reader.ReadUInt16();
            return (LogLevel)Enum.ToObject(typeof(LogLevel), value);
        }
    }
}
