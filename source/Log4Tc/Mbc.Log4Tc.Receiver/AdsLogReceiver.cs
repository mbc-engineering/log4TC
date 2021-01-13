using Mbc.Log4Tc.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.Ads.Server;

using LogLevel = Mbc.Log4Tc.Model.LogLevel;

namespace Mbc.Log4Tc.Receiver
{
    public class AdsLogReceiver : AdsServer, ILogReceiver
    {
        private readonly ILogger<AdsLogReceiver> _logger;
        private readonly AdsHostnameService _adsHostnameService;

        public event EventHandler<LogEntryEventArgs> LogsReceived;

        public AdsLogReceiver(ILogger<AdsLogReceiver> logger, AdsHostnameService adsHostnameService)
            : base(16150, "Log4Tc")
        {
            _logger = logger;
            _adsHostnameService = adsHostnameService;
        }

        protected override Task<AdsErrorCode> WriteConfirmationAsync(AmsAddress sender, uint invokeId, AdsErrorCode result, CancellationToken cancel)
        {
            return base.WriteConfirmationAsync(sender, invokeId, result, cancel);
        }

        protected override async Task<AdsErrorCode> WriteIndicationAsync(AmsAddress target, uint invokeId, uint indexGroup, uint indexOffset, ReadOnlyMemory<byte> writeData, CancellationToken cancel)
        {
            // send response as soon as possible
            var result = await base.WriteIndicationAsync(target, invokeId, indexGroup, indexOffset, writeData, cancel);

            try
            {
                var entries = new List<LogEntry>();
                var dataReader = new AdsDataCompatibilityReader(writeData);

                while (!dataReader.Eof)
                {
                    LogEntry logEntry;

                    // Read the first byte in block for version
                    var version = dataReader.ReadByte();
                    if (version == 1)
                    {
                        logEntry = ReadLogEntryV1(dataReader);
                    }
                    else
                    {
                        throw new NotImplementedException($"Version {version}");
                    }

                    logEntry.Source = target.ToString();
                    logEntry.Hostname = _adsHostnameService.GetHostname(target.NetId).ValueOr(string.Empty);

                    entries.Add(logEntry);
                }

                LogsReceived?.Invoke(this, new LogEntryEventArgs(entries));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing log message from plc.");
            }

            return result;
        }

        // ToDo: Remove old version
        /*
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
                    logEntry.Hostname = _adsHostnameService.GetHostname(rAddr.NetId).ValueOr(string.Empty);

                    entries.Add(logEntry);
                }

                LogsReceived?.Invoke(this, new LogEntryEventArgs(entries));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing log message from plc.");
            }
        }
        */

        private LogEntry ReadLogEntryV1(AdsDataCompatibilityReader dataReader)
        {
            var logEntry = new LogEntry
            {
                Message = dataReader.ReadString(),
                Logger = dataReader.ReadString(),
                Level = ReadLogLevel(dataReader),
                PlcTimestamp = ReadFiletime(dataReader),
                ClockTimestamp = ReadFiletime(dataReader),
                TaskIndex = dataReader.ReadInt32(),
                TaskName = dataReader.ReadString(),
                TaskCycleCounter = dataReader.ReadUInt32(),
                AppName = dataReader.ReadString(),
                ProjectName = dataReader.ReadString(),
                OnlineChangeCount = dataReader.ReadUInt32(),
            };

            var end = false;
            while (!end)
            {
                var type = dataReader.ReadByte();
                switch (type)
                {
                    case 1:
                        var argIdx = (int)dataReader.ReadByte();
                        var argValue = ReadObject(dataReader);
                        logEntry.Arguments.Add(argIdx, argValue);
                        break;
                    case 2:
                        // context
                        var ctxScope = dataReader.ReadByte(); // TODO
                        var ctxName = dataReader.ReadString();
                        var ctxValue = ReadObject(dataReader);
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

        private DateTime ReadFiletime(AdsDataCompatibilityReader dataReader)
        {
            var filetime = dataReader.ReadInt64();
            try
            {
                return DateTime.FromFileTime(filetime);
            }
            catch (ArgumentException)
            {
                return new DateTime(0);
            }
        }

        private object ReadObject(AdsDataCompatibilityReader dataReader)
        {
            var type = dataReader.ReadInt16();
            object value;
            switch (type)
            {
                case 0:
                    value = null;
                    break;
                case 1: // BYTE
                    value = dataReader.ReadByte();
                    break;
                case 2: // WORD
                    value = dataReader.ReadUInt16();
                    break;
                case 3: // DWORD
                    value = dataReader.ReadUInt32();
                    break;
                case 4: // REAL
                    value = dataReader.ReadSingle();
                    break;
                case 5: // LREAL
                    value = dataReader.ReadDouble();
                    break;
                case 6: // SINT
                    value = dataReader.ReadSByte();
                    break;
                case 7: // INT
                    value = dataReader.ReadInt16();
                    break;
                case 8: // DINT
                    value = dataReader.ReadInt32();
                    break;
                case 9: // USINT
                    value = dataReader.ReadByte();
                    break;
                case 10: // UINT
                    value = dataReader.ReadUInt16();
                    break;
                case 11: // UDINT
                    value = dataReader.ReadUInt32();
                    break;
                case 12: // String
                    value = dataReader.ReadString();
                    break;
                case 13: // BOOL
                    value = dataReader.ReadByte() != 0;
                    break;
                case 15: // ULARGE
                    value = dataReader.ReadUInt64();
                    break;
                case 17: // LARGE
                    value = dataReader.ReadInt64();
                    break;
                case 20000: // Custom Type TIME
                    value = TimeSpan.FromMilliseconds(dataReader.ReadUInt32());
                    break;
                case 20001: // Custom Type LTIME
                    // TimeSpan might loose nanoseconds precision
                    value = TimeSpan.FromTicks((long)(dataReader.ReadUInt64() / (1000000 / TimeSpan.TicksPerMillisecond)));
                    break;
                case 20002: // Custom Type DATE
                case 20003: // Custom Type DATE_AND_TIME
                    value = DateTimeOffset.FromUnixTimeSeconds(dataReader.ReadUInt32());
                    break;
                case 20004: // Custom Type TIME_OF_DAY
                    // C# has no time only type
                    value = TimeSpan.FromMilliseconds(dataReader.ReadUInt32());
                    break;
                case 20005: // Custom Type ENUM
                    // enum values contains its integer value
                    value = ReadObject(dataReader);
                    break;
                case 20006: // Custom Type WSTRING
                    value = dataReader.ReadWString();
                    break;
                default:
                    throw new NotImplementedException($"type {type}");
            }

            return value;
        }

        private LogLevel ReadLogLevel(AdsDataCompatibilityReader dataReader)
        {
            var value = dataReader.ReadUInt16();
            return (LogLevel)Enum.ToObject(typeof(LogLevel), value);
        }
    }
}
