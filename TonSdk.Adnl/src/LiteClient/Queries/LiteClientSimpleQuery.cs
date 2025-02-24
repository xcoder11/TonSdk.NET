using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public abstract class LiteClientSimpleQuery<TResponse> : LiteClientBaseQuery<TResponse>
{
    public override (byte[] id, byte[] data) Encode()
    {
        var writer = new TLWriteBuffer();
        writer.WriteUInt32(Code);
        return EncodeBase(writer);
    }
}