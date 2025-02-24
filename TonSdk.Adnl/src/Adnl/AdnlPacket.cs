using System;
using System.Linq;
using System.Security.Cryptography;
using TonSdk.Core.Boc;

namespace TonSdk.Adnl.Adnl;

internal class AdnlPacket
{
    internal const byte PacketMinSize = 68; // 4 (size) + 32 (nonce) + 32 (hash)

    internal AdnlPacket(byte[] payload, byte[]? nonce = null)
    {
        Nonce = nonce ?? AdnlKeys.GenerateRandomBytes(32);
        Payload = payload;
    }

    internal byte[] Payload { get; }

    private byte[] Nonce { get; }

    private byte[] Hash
    {
        get
        {
            using var sha256 = SHA256.Create();
            var sha256Hash = sha256.ComputeHash(Nonce.Concat(Payload).ToArray());
            return sha256Hash;
        }
    }

    private byte[] Size
    {
        get
        {
            var size = (uint)(32 + 32 + Payload.Length);
            var builder = new BitsBuilder().StoreUInt32LE(size).Build();
            return builder.ToBytes();
        }
    }

    internal byte[] Data => Size.Concat(Nonce).Concat(Payload).Concat(Hash).ToArray();

    internal int Length => PacketMinSize + Payload.Length;

    internal static AdnlPacket? Parse(byte[] data)
    {
        if (data.Length < 4) return null;
        var cursor = 0;

        var slice = new Bits(data).Parse();

        var size = (uint)slice.LoadUInt32LE();
        cursor += 4;

        if (data.Length - 4 < size) return null;

        var nonce = new byte[32];
        Array.Copy(data, cursor, nonce, 0, 32);
        cursor += 32;

        var payload = new byte[size - (32 + 32)];
        Array.Copy(data, cursor, payload, 0, size - (32 + 32));
        cursor += (int)size - (32 + 32);

        var hash = new byte[32];
        Array.Copy(data, cursor, hash, 0, 32);

        using var sha256 = SHA256.Create();
        var target = sha256.ComputeHash(nonce.Concat(payload).ToArray());

        if (!hash.SequenceEqual(target)) throw new Exception("ADNLPacket: Bad packet hash.");

        return new AdnlPacket(payload, nonce);
    }
}