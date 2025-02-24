using System;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class PingPongQuery : ILiteClientQuery<bool>
{
    public (byte[] id, byte[] data) Encode()
    {
        var writer = new TLWriteBuffer();
        writer.WriteUInt32(Codes.Ping);

        var random = new Random();
        var firstPart = random.Next();
        var secondPart = random.Next();
        var randomInt64 = ((long)firstPart << 32) | (uint)secondPart;
        writer.WriteInt64(randomInt64);
        return ([0], writer.Build());
    }

    public bool Decode(TLReadBuffer buffer)
    {
        return true;
    }
}