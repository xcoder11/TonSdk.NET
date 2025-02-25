using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetTimeQuery : LiteClientSimpleQuery<int>
{
    public static GetTimeQuery Instance { get; } = new();
    public override uint Code => Codes.GetTime;

    public override int Decode(TLReadBuffer buffer)
    {
        // now:int
        var time = buffer.ReadInt32();
        return time;
    }
}