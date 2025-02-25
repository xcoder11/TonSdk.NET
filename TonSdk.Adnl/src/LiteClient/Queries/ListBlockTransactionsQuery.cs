using System.Collections.Generic;
using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class ListBlockTransactionsQuery(
    BlockIdExtended block,
    uint limit,
    ITransactionId? after = null,
    bool? reverseOrder = null,
    bool? wantProof = null) : LiteClientComplexQuery<ListBlockTransactionsResult>
{
    public override uint Code => Codes.BlockTransactions;

    public override ListBlockTransactionsResult Decode(TLReadBuffer buffer)
    {
        // id:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        buffer.ReadUInt32();
        var inComplete = buffer.ReadBool();

        var count = buffer.ReadUInt32();

        var ids = new List<TransactionId>();
        for (var i = 0; i < count; i++)
        {
            var id = new TransactionId();
            buffer.ReadUInt32();
            id.Account = buffer.ReadInt256();
            id.Lt = buffer.ReadInt64();
            id.Hash = buffer.ReadInt256();
            ids.Add(id);
        }

        var proof = buffer.ReadBuffer();

        return new ListBlockTransactionsResult(inComplete, ids.ToArray(), proof);
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