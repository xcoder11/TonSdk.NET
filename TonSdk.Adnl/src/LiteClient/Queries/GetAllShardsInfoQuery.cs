using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetAllShardsInfoQuery(BlockIdExtended block) : LiteClientComplexQuery<byte[]>
{
    public override uint Code => Codes.AllShardsInfo;

    public override byte[] Decode(TLReadBuffer buffer)
    {
        // id:tonNode.blockIdExt
        var workchain = buffer.ReadInt32();
        var shard = buffer.ReadInt64();
        var seqno = buffer.ReadInt32();
        var rootHash = buffer.ReadInt256();
        var fileHash = buffer.ReadInt256();
        _ = new BlockIdExtended(workchain, rootHash, fileHash, shard, seqno);

        buffer.ReadBuffer();
        var data = buffer.ReadBuffer();
        return data;
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        FillBlockPart(writer, block);
    }
}