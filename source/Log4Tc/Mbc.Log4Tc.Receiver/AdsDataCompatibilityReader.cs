using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.TypeSystem;

namespace Mbc.Log4Tc.Receiver
{
    /// <summary>
    /// helps for a simple compatibility to the obsolete AdsStream
    /// It helps to read a data blob read forward
    /// </summary>
    public class AdsDataCompatibilityReader
    {
        private readonly PrimitiveTypeMarshaler _converter;

        public AdsDataCompatibilityReader(ReadOnlyMemory<byte> data)
        {
            ForwardReadIdx = 0;
            Data = data;
            _converter = new PrimitiveTypeMarshaler(StringMarshaler.DefaultEncoding);
        }

        public ReadOnlyMemory<byte> Data { get; }

        /// <summary>
        /// Increment each time read some data
        /// </summary>
        public int ForwardReadIdx { get; private set; }

        public bool Eof => Data.Length >= ForwardReadIdx;

        public byte ReadByte()
        {
            return Data.Span[ForwardReadIdx++];
        }

        public string ReadString()
        {
            int len = ReadByte();
            int startidx = ForwardReadIdx;
            ForwardReadIdx += len;
            return Encoding.GetEncoding(1252).GetString(Data.Span.Slice(startidx, len));
        }

        public string ReadWString()
        {
            int len = ReadByte() * 2;
            int startidx = ForwardReadIdx;
            ForwardReadIdx += len;
            return Encoding.Unicode.GetString(Data.Span.Slice(startidx, len));
        }

        public ushort ReadUInt16()
        {
            int len = 2;
            _converter.Unmarshal(Data.Span.Slice(ForwardReadIdx, len), out ushort value);
            ForwardReadIdx += len;
            return value;
        }

        public short ReadInt16()
        {
            int len = 2;
            _converter.Unmarshal(Data.Span.Slice(ForwardReadIdx, len), out short value);
            ForwardReadIdx += len;
            return value;
        }

        public ulong ReadUInt64()
        {
            int len = 8;
            _converter.Unmarshal(Data.Span.Slice(ForwardReadIdx, len), out ulong value);
            ForwardReadIdx += len;
            return value;
        }

        public long ReadInt64()
        {
            int len = 8;
            _converter.Unmarshal(Data.Span.Slice(ForwardReadIdx, len), out long value);
            ForwardReadIdx += len;
            return value;
        }

        public int ReadInt32()
        {
            int len = 4;
            _converter.Unmarshal(Data.Span.Slice(ForwardReadIdx, len), out int value);
            ForwardReadIdx += len;
            return value;
        }

        public uint ReadUInt32()
        {
            int len = 4;
            _converter.Unmarshal(Data.Span.Slice(ForwardReadIdx, len), out uint value);
            ForwardReadIdx += len;
            return value;
        }

        public float ReadSingle()
        {
            int len = 4;
            _converter.Unmarshal(Data.Span.Slice(ForwardReadIdx, len), out float value);
            ForwardReadIdx += len;
            return value;
        }

        public double ReadDouble()
        {
            int len = 8;
            _converter.Unmarshal(Data.Span.Slice(ForwardReadIdx, len), out double value);
            ForwardReadIdx += len;
            return value;
        }

        public sbyte ReadSByte()
        {
            _converter.Unmarshal(Data.Span.Slice(ForwardReadIdx++, 1), out sbyte value);
            return value;
        }
    }
}
