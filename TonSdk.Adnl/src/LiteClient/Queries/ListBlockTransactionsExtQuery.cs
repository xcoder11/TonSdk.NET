using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class ListBlockTransactionsExtQuery(
    BlockIdExtended block,
    uint limit,
    ITransactionId? after = null,
    bool? reverseOrder = null,
    bool? wantProof = null) : LiteClientComplexQuery<ListBlockTransactionsExtendedResult>
{
    public override uint Code => Codes.BlockTransactionsExt;

    public override ListBlockTransactionsExtendedResult Decode(TLReadBuffer buffer)
    {
        // id:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        buffer.ReadUInt32();
        var inComplete = buffer.ReadBool();
        var transactions = buffer.ReadBuffer();
        var proof = buffer.ReadBuffer();

        return new ListBlockTransactionsExtendedResult(inComplete, transactions, proof);
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        writer.WriteInt32(block.Workchain);
        writer.WriteInt64(block.Shard);
        writer.WriteInt32(block.Seqno);
        writer.WriteBytes(block.RootHash, 32);
        writer.WriteBytes(block.FileHash, 32);

        if (after == null)
        {
            writer.WriteUInt32(7);
            writer.WriteUInt32(limit);
        }
        else
        {
            writer.WriteUInt32(7 + 128);
            writer.WriteUInt32(limit);
            switch (after)
            {
                case TransactionId id:
                    writer.WriteBytes(id.Hash, 32);
                    writer.WriteInt64(id.Lt);
                    break;
                case TransactionId3 id3:
                    writer.WriteBytes(id3.Account.GetHash(), 32);
                    writer.WriteInt64(id3.Lt);
                    break;
            }
        }
    }
}