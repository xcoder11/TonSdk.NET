using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetBlockQuery(BlockIdExtended block) : LiteClientComplexQuery<byte[]>
{
    public override uint Code => Codes.BlockData;

    public override byte[] Decode(TLReadBuffer buffer)
    {
        // id:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        var data = buffer.ReadBuffer();
        return data;
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        FillBlockPart(writer, block);
    }
}