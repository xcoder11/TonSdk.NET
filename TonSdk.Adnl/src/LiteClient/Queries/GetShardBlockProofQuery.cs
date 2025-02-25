using System.Collections.Generic;
using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetShardBlockProofQuery(BlockIdExtended block) : LiteClientComplexQuery<ShardBlockProof>
{
    public override uint Code => Codes.ShardBlockProof;

    public override ShardBlockProof Decode(TLReadBuffer buffer)
    {
        // masterchain_id:tonNode.blockIdExt
        var w = buffer.ReadInt32();
        var shard = buffer.ReadInt64();
        var seqno = buffer.ReadInt32();
        var rootHash = buffer.ReadBytes(32);
        var fileHash = buffer.ReadBytes(32);
        var masterChainId = new BlockIdExtended(w, rootHash, fileHash, shard, seqno);

        var count = buffer.ReadUInt32();

        var links = new List<ShardBlockLink>();
        for (var i = 0; i < count; i++)
        {
            // from:tonNode.blockIdExt
            var linkFromW = buffer.ReadInt32();
            var linkFromShard = buffer.ReadInt64();
            var linkFromSeqno = buffer.ReadInt32();
            var linkFromRootHash = buffer.ReadBytes(32);
            var linkFromFileHash = buffer.ReadBytes(32);
            var linkFrom = new BlockIdExtended(linkFromW, linkFromRootHash, linkFromFileHash, linkFromShard,
                linkFromSeqno);

            var proof = buffer.ReadBuffer();
            links.Add(new ShardBlockLink
            {
                BlockIdExtended = linkFrom,
                Proof = proof
            });
        }

        return new ShardBlockProof
        {
            MasterChainId = masterChainId,
            Links = links.ToArray()
        };
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        FillBlockPart(writer, block);
    }
}