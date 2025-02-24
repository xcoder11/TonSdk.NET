using System;
using System.IO;
using System.Text;

namespace TonSdk.Adnl.TL;

public class TLReadBuffer
{
    private readonly BinaryReader _reader;

    public TLReadBuffer(byte[] buffer)
    {
        _reader = new BinaryReader(new MemoryStream(buffer));
    }

    public int Remaining => (int)(_reader.BaseStream.Length - _reader.BaseStream.Position);

    private void EnsureSize(int needBytes)
    {
        if (_reader.BaseStream.Position + needBytes > _reader.BaseStream.Length)
            throw new Exception("Not enough bytes");
    }

    public int ReadInt32()
    {
        EnsureSize(4);
        return _reader.ReadInt32();
    }

    public uint ReadUInt32()
    {
        EnsureSize(4);
        return _reader.ReadUInt32();
    }

    public long ReadInt64()
    {
        EnsureSize(8);
        return _reader.ReadInt64();
    }

    public byte ReadUInt8()
    {
        EnsureSize(1);
        return _reader.ReadByte();
    }

    public byte[] ReadInt256()
    {
        EnsureSize(32);
        return _reader.ReadBytes(32);
    }

    public byte[] ReadBytes(int size)
    {
        EnsureSize(size);
        return _reader.ReadBytes(size);
    }


    public byte[] ReadBuffer()
    {
        int len = ReadUInt8();

        if (len == 254)
        {
            var readed = _reader.ReadBytes(3);
            len = readed[0] | (readed[1] << 8) | (readed[2] << 16);
        }

        var buffer = _reader.ReadBytes(len);

        while (_reader.BaseStream.Position % 4 != 0) _reader.ReadByte();

        return buffer;
    }

    public string ReadString()
    {
        var buffer = ReadBuffer();
        return Encoding.UTF8.GetString(buffer);
    }

    private bool CompareBytes(byte[] array, byte[] compareWith)
    {
        if (array.Length != compareWith.Length) return false;

        for (var i = 0; i < array.Length; i++)
            if (array[i] != compareWith[i])
                return false;

        return true;
    }

    public bool ReadBool()
    {
        var value = ReadBytes(4);

        byte[] falseBytes = { 0x37, 0x97, 0x79, 0xbc };
        byte[] trueBytes = { 0xb5, 0x75, 0x72, 0x99 };

        if (CompareBytes(value, falseBytes)) return false;
        if (CompareBytes(value, trueBytes)) return true;

        throw new Exception("Unknown boolean value");
    }

    public T[] ReadVector<T>(Func<TLReadBuffer, T> codec)
    {
        var count = (int)ReadUInt32();
        T[] result = new T[count];
        for (var i = 0; i < count; i++) result[i] = codec(this);
        return result;
    }

    public byte[] ReadObject()
    {
        var remainingBytes = (int)(_reader.BaseStream.Length - _reader.BaseStream.Position);
        return _reader.ReadBytes(remainingBytes);
    }
}