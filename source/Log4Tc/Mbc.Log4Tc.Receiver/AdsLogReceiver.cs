using Mbc.Log4Tc.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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

        // Der Port sollte zwischen TwinCAT.Ads.AmsPortRange.CUSTOMERPRIVATE_FIRST and TwinCAT.Ads.AmsPortRange.CUSTOMERPRIVATE_LAST
        // also First usable port for private networks (0x6590) and Last usable port for private networks (0x6977)
        public AdsLogReceiver(ILoggerFactory loggerFactory, AdsHostnameService adsHostnameService)
            : base(16150, "Log4Tc", loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AdsLogReceiver>();
            _adsHostnameService = adsHostnameService;
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            _logger.LogInformation($"Log4Tc AdsServer with name={ServerName}; Address={ServerAddress}:{ServerPort}; Version={ServerVersion} is connected!");
        }

        protected override bool OnDisconnect()
        {
            var res = base.OnDisconnect();
            _logger.LogInformation($"Log4Tc AdsServer with name={ServerName}; Address={ServerAddress}:{ServerPort}; Version={ServerVersion} is disconnected!");
            return res;
        }

        protected override Task<ResultWrite> OnWriteAsync(AmsAddress target, uint invokeId, uint indexGroup, uint indexOffset, ReadOnlyMemory<byte> writeData, CancellationToken cancel)
        {
            ResultWrite result = ResultWrite.CreateError(AdsErrorCode.DeviceServiceNotSupported);

            try
            {
                var entries = new List<LogEntry>();
                var reader = new BinaryReader(new MemoryStream(writeData.ToArray()));

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

                    logEntry.Source = target.ToString();
                    logEntry.Hostname = _adsHostnameService.GetHostname(target.NetId).ValueOr(string.Empty);

                    entries.Add(logEntry);
                }

                LogsReceived?.Invoke(this, new LogEntryEventArgs(entries));
                result = ResultWrite.CreateSuccess(invokeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing log message from plc.");
                result = ResultWrite.CreateError(AdsErrorCode.DeviceError, invokeId);
            }

            return Task.FromResult(result);
        }

        private LogEntry ReadLogEntryV1(BinaryReader reader)
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

        private DateTime ReadFiletime(BinaryReader reader)
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

        private object ReadObject(BinaryReader reader)
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
                    value = reader.ReadDouble();
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
                case 20000: // Custom Type TIME
                    value = TimeSpan.FromMilliseconds(reader.ReadUInt32());
                    break;
                case 20001: // Custom Type LTIME
                    // TimeSpan might loose nanoseconds precision
                    value = TimeSpan.FromTicks((long)(reader.ReadUInt64() / (1000000 / TimeSpan.TicksPerMillisecond)));
                    break;
                case 20002: // Custom Type DATE
                case 20003: // Custom Type DATE_AND_TIME
                    value = DateTimeOffset.FromUnixTimeSeconds(reader.ReadUInt32());
                    break;
                case 20004: // Custom Type TIME_OF_DAY
                    // C# has no time only type
                    value = TimeSpan.FromMilliseconds(reader.ReadUInt32());
                    break;
                case 20005: // Custom Type ENUM
                    // enum values contains its integer value
                    value = ReadObject(reader);
                    break;
                case 20006: // Custom Type WSTRING
                    value = ReadWString(reader);
                    break;
                default:
                    throw new NotImplementedException($"type {type}");
            }

            return value;
        }

        private string ReadString(BinaryReader reader)
        {
            var len = reader.ReadByte();
            var data = reader.ReadBytes(len);
            return Encoding.GetEncoding(1252).GetString(data);
        }

        private string ReadWString(BinaryReader reader)
        {
            var len = reader.ReadByte();
            var data = reader.ReadBytes(len * 2);
            return Encoding.Unicode.GetString(data);
        }

        private LogLevel ReadLogLevel(BinaryReader reader)
        {
            var value = reader.ReadUInt16();
            return (LogLevel)Enum.ToObject(typeof(LogLevel), value);
        }
    }
}
