using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;
using TonSdk.Adnl.Utils;
using TonSdk.Core;

namespace TonSdk.Adnl.LiteClient.Queries;

public class FetchAccountStateQuery(BlockIdExtended block, Address account, string query)
    : LiteClientComplexQuery<AccountStateResult>
{
    public override uint Code { get; } = Crc32.Code(query);

    public override AccountStateResult Decode(TLReadBuffer buffer)
    {
        // id:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        // shardblk:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        var result = new AccountStateResult
        {
            ShardProof = buffer.ReadBuffer(),
            Proof = buffer.ReadBuffer(),
            State = buffer.ReadBuffer()
        };
        return result;
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        FillBlockPart(writer, block);
        writer.WriteInt32(account.GetWorkchain());
        writer.WriteBytes(account.GetHash(), 32);
    }
}