using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetLookUpBlockQuery(int workchain, long shard, long? seqno, ulong? lt, ulong? uTime)
    : LiteClientComplexQuery<BlockHeader>
{
    public override uint Code => Codes.LookUpBlock;

    public override BlockHeader Decode(TLReadBuffer buffer)
    {
        var workchain1 = buffer.ReadInt32();
        var shard1 = buffer.ReadInt64();
        var seqno1 = buffer.ReadInt32();
        var rootHash = buffer.ReadBytes(32);
        var fileHash = buffer.ReadBytes(32);

        var blockIdExtended = new BlockIdExtended(workchain1, rootHash, fileHash, shard1, seqno1);
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
        ;
        uint mode = 0;
        if (seqno != null) mode |= 1;
        if (lt != null) mode |= 2;
        if (uTime != null) mode |= 4;

        writer.WriteUInt32(mode);

        writer.WriteInt32(workchain);
        writer.WriteInt64(shard);

        if (seqno != null) writer.WriteUInt32((uint)seqno);
        else writer.WriteUInt32(0);
        if (lt != null) writer.WriteInt64((long)lt.Value);
        if (uTime != null) writer.WriteInt32((int)uTime.Value);
    }
}