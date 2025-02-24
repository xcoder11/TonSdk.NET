using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public abstract class LiteClientComplexQuery<TResponse> : LiteClientBaseQuery<TResponse>
{
    public override (byte[] id, byte[] data) Encode()
    {
        var writer = new TLWriteBuffer();
        writer.WriteUInt32(Code);
        EncodeInternal(writer);
        return EncodeBase(writer);
    }

    protected abstract void EncodeInternal(TLWriteBuffer writer);
}