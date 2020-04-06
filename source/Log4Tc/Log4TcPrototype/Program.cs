using System;
using System.Collections.Generic;
using System.Text;
using TwinCAT.Ads;
using TwinCAT.Ads.Server;
using TwinCAT.TypeSystem;

namespace Log4TcPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new LogServer();
            server.Connect();

            Console.WriteLine("Server is ready.");
            Console.ReadKey();

            server.Disconnect();
        }
    }

    internal class LogServer : TcAdsServer
    {
        public LogServer() : base(16150, "Log4TC")
        {
        }

        public override void AdsWriteInd(AmsAddress rAddr, uint invokeId, uint indexGroup, uint indexOffset, uint cbLength, byte[] data)
        {
            Console.WriteLine($"Request from {rAddr}");

            try
            {
                var buffer = new AdsBinaryReader(new AdsStream(data));

                while (buffer.BaseStream.Length > buffer.BaseStream.Position)
                {
                    var version = buffer.ReadByte();
                    byte[] buf = new byte[256];
                    byte ch;
                    int i = 0;
                    while ((ch = buffer.ReadByte()) != 0)
                    {
                        buf[i++] = ch;
                    }
                    var message = Encoding.Default.GetString(buf, 0, i);

                    i = 0;
                    while ((ch = buffer.ReadByte()) != 0)
                    {
                        buf[i++] = ch;
                    }
                    var logger = Encoding.Default.GetString(buf, 0, i);

                    var level = buffer.ReadUInt16();
                    var timestampPlc = DateTime.FromFileTime(buffer.ReadInt64());
                    var timestampClock = DateTime.FromFileTime(buffer.ReadInt64());

                    var args = new List<(int, object)>();
                    byte type;
                    while ((type = buffer.ReadByte()) != 255)
                    {
                        if (type == 1)
                        {
                            // Argument
                            var argNo = buffer.ReadByte();
                            var argType = buffer.ReadInt16();
                            object argValue = null;
                            switch (argType)
                            {
                                case 4: // REAL
                                    argValue = buffer.ReadSingle();
                                    break;

                                case 7: // INT
                                    argValue = buffer.ReadInt16();
                                    break;

                                case 12: // STRING
                                    i = 0;
                                    while ((ch = buffer.ReadByte()) != 0)
                                    {
                                        buf[i++] = ch;
                                    }
                                    argValue = Encoding.Default.GetString(buf, 0, i);
                                    break;
                            }

                            args.Add((argNo, argValue));
                        }
                    }

                    Console.WriteLine($"Log-Entry: version={version} message={message} logger={logger} level={level} timestamp={timestampPlc}.{timestampPlc.Millisecond} args=[{string.Join(",", args)}]");
                }

                AdsWriteRes(rAddr, invokeId, AdsErrorCode.NoError);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e}");
                AdsWriteRes(rAddr, invokeId, AdsErrorCode.DeviceError);
            }
        }
    }
}
