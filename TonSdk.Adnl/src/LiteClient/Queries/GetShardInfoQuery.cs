using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetShardInfoQuery(
    BlockIdExtended block,
    int workchain,
    long shard,
    bool exact = false) : LiteClientComplexQuery<ShardInfo>
{
    public override uint Code => Codes.ShardInfo;

    public override ShardInfo Decode(TLReadBuffer buffer)
    {
        // id:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        // shardblk:tonNode.blockIdExt
        var workchain1 = buffer.ReadInt32();
        var shard1 = buffer.ReadInt64();
        var seqno = buffer.ReadInt32();
        var rootHash = buffer.ReadInt256();
        var fileHash = buffer.ReadInt256();
        var shardBlock = new BlockIdExtended(workchain1, rootHash, fileHash, shard1, seqno);

        var shardProof = buffer.ReadBuffer();
        var shardDescr = buffer.ReadBuffer();

        return new ShardInfo(shardProof, shardDescr, shardBlock);
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        FillBlockPart(writer, block);
        writer.WriteInt32(workchain);
        writer.WriteInt64(shard);
        writer.WriteBool(exact);
    }
}