using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetBlockHeaderQuery(BlockIdExtended block) : LiteClientComplexQuery<BlockHeader>
{
    public override uint Code => Codes.BlockHeader;

    public override BlockHeader Decode(TLReadBuffer buffer)
    {
        var workchain = buffer.ReadInt32();
        var shard = buffer.ReadInt64();
        var seqno = buffer.ReadInt32();
        var rootHash = buffer.ReadBytes(32);
        var fileHash = buffer.ReadBytes(32);

        var blockIdExtended = new BlockIdExtended(workchain, rootHash, fileHash, shard, seqno);
        // mode:#
        buffer.ReadUInt32();

        // header_proof:bytes
        var headerProof = buffer.ReadBuffer();
        return new BlockHeader
        {
            BlockId = blockIdExtended,
            HeaderProof = headerProof
        };
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        FillBlockPart(writer, block);
        writer.WriteUInt32(1);
    }
}