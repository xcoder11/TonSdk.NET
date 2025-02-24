using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetVersionQuery : LiteClientSimpleQuery<ChainVersion>
{
    public static GetVersionQuery Instance { get; } = new();
    public override uint Code => Codes.Version;

    public override ChainVersion Decode(TLReadBuffer buffer)
    {
        // mode:#
        buffer.ReadInt32();
        // version:int
        var version = buffer.ReadInt32();
        // capabilities:long
        var capabilities = buffer.ReadInt64();
        // now:int
        var time = buffer.ReadInt32();

        return new ChainVersion(version, capabilities, time);
    }
}