using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public interface ILiteClientQuery<out TResponse>
{
    (byte[] id, byte[] data) Encode();
    TResponse Decode(TLReadBuffer buffer);
}