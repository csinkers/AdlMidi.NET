using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SerdesNet
{
    public class GenericBinaryWriter : ISerializer
    {
        readonly Action<string> _assertionFailed;
        readonly Func<string, byte[]> _stringToBytes;
        readonly Stack<int> _versionStack = new Stack<int>();
        readonly BinaryWriter _bw;
        long _offset;

        public GenericBinaryWriter(BinaryWriter bw, Func<string, byte[]> stringToBytes, Action<string> assertionFailed = null)
        {
            _bw = bw ?? throw new ArgumentNullException(nameof(bw));
            _stringToBytes = stringToBytes ?? throw new ArgumentNullException(nameof(stringToBytes));
            _assertionFailed = assertionFailed;
        }

        public SerializerMode Mode => SerializerMode.Writing;
        public void PushVersion(int version) => _versionStack.Push(version);
        public int PopVersion() => _versionStack.Count == 0 ? 0 : _versionStack.Pop();
        public void Comment(string msg) { }
        public void Indent() { }
        public void Unindent() { }
        public void NewLine() { }
        public long Offset
        {
            get
            {
                Assert(_offset == _bw.BaseStream.Position);
                return _offset;
            }
        }

        public void Seek(long newOffset)
        {
            _bw.Seek((int)newOffset, SeekOrigin.Begin);
            _offset = newOffset;
        }

        public sbyte Int8(string name, sbyte existing)     { _bw.Write(existing); _offset += 1L; return existing; }
        public short Int16(string name, short existing)    { _bw.Write(existing); _offset += 2L; return existing; }
        public int Int32(string name, int existing)        { _bw.Write(existing); _offset += 4L; return existing; }
        public long Int64(string name, long existing)      { _bw.Write(existing); _offset += 8L; return existing; }
        public byte UInt8(string name, byte existing)      { _bw.Write(existing); _offset += 1L; return existing; }
        public ushort UInt16(string name, ushort existing) { _bw.Write(existing); _offset += 2L; return existing; }
        public uint UInt32(string name, uint existing)     { _bw.Write(existing); _offset += 4L; return existing; }
        public ulong UInt64(string name, ulong existing)   { _bw.Write(existing); _offset += 8L; return existing; }
        public T EnumU8<T>(string name, T existing) where T : struct, Enum
        {
            _bw.Write((byte)(object)existing);
            _offset += 1L;
            return existing;
        }

        public T EnumU16<T>(string name, T existing) where T : struct, Enum
        {
            _bw.Write((ushort)(object)existing);
            _offset += 2L;
            return existing;
        }

        public T EnumU32<T>(string name, T existing) where T : struct, Enum
        {
            _bw.Write((uint)(object)existing);
            _offset += 4L;
            return existing;
        }

        public Guid Guid(string name, Guid existing)
        {
            var v = existing;
            _bw.Write(v.ToByteArray());
            _offset += 16L;
            return existing;
        }

        public byte[] ByteArray(string name, byte[] existing, int n)
        {
            var v = existing;
            _bw.Write(v);
            _offset += v.Length;
            return existing;
        }
        public byte[] ByteArray2(string name, byte[] existing, int n, string comment)
        {
            var v = existing;
            _bw.Write(v);
            _offset += v.Length;
            return existing;
        }
        public byte[] ByteArrayHex(string name, byte[] existing, int n)
        {
            var v = existing;
            _bw.Write(v);
            _offset += v.Length;
            return existing;
        }

        public string NullTerminatedString(string name, string existing)
        {
            var v = existing;
            var bytes = _stringToBytes(v);
            _bw.Write(bytes);
            _bw.Write((byte)0);
            _offset += bytes.Length + 1; // add 2 bytes for the null terminator
            return existing;
        }

        public string FixedLengthString(string name, string existing, int length)
        {
            var bytes = _stringToBytes(existing ?? "");
            if (bytes.Length > length + 1) throw new InvalidOperationException("Tried to write overlength string");
            _bw.Write(bytes);
            _bw.Write(Enumerable.Repeat((byte)0, length - bytes.Length).ToArray());
            _offset += length; // Pad out to the full length
            return existing;
        }

        public void RepeatU8(string name, byte v, int length)
        {
            _bw.Write(Enumerable.Repeat(v, length).ToArray());
            _offset += length;
        }

        public TMemory Transform<TPersistent, TMemory>(string name, TMemory existing, Func<string, TPersistent, TPersistent> serializer, IConverter<TPersistent, TMemory> converter) =>
            converter.ToMemory(serializer(name, converter.ToPersistent(existing)));

        public void Meta(string name, Action<ISerializer> serializer, Action<ISerializer> deserializer) => serializer(this);
        public T Meta<T>(string name, T existing, Func<int, T, ISerializer, T> serdes) => serdes(0, existing, this);

        public void Check() { }
        public bool IsComplete() => false;

        public void List<TTarget>(IList<TTarget> list, int count, Func<int, TTarget, ISerializer, TTarget> serializer) where TTarget : class
        {
            for (int i = 0; i < count; i++)
                serializer(i, list[i], this);
        }

        public void List<TTarget>(IList<TTarget> list, int count, int offset, Func<int, TTarget, ISerializer, TTarget> serializer) where TTarget : class
        {
            for (int i = offset; i < count + offset; i++)
                serializer(i, list[i], this);
        }

        void Assert(bool result, string message = null, [CallerMemberName] string function = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (result)
                return;

            var formatted = $"Assertion failed: {message} at {function} in {file}:{line}";
            _assertionFailed?.Invoke(formatted);
        }
    }
}
