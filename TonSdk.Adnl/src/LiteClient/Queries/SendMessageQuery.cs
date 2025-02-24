using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class SendMessageQuery(byte[] body) : LiteClientComplexQuery<int>
{
    public override uint Code => Codes.SendMsgStatus;

    public override int Decode(TLReadBuffer buffer)
    {
        return buffer.ReadInt32();
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        writer.WriteBuffer(body);
    }
}