using System.Numerics;
using TonSdk.Adnl.Adnl;
using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public abstract class LiteClientBaseQuery<TResponse> : ILiteClientQuery<TResponse>
{
    public abstract uint Code { get; }
    public abstract (byte[] id, byte[] data) Encode();
    public abstract TResponse Decode(TLReadBuffer buffer);

    protected void FillBlockPart(TLWriteBuffer writer, BlockIdExtended block)
    {
        writer.WriteInt32(block.Workchain);
        writer.WriteInt64(block.Shard);
        writer.WriteInt32(block.Seqno);
        writer.WriteBytes(block.RootHash, 32);
        writer.WriteBytes(block.FileHash, 32);
    }

    protected static (byte[], byte[]) EncodeBase(TLWriteBuffer methodWriter)
    {
        var queryId = AdnlKeys.GenerateRandomBytes(32);
        var writer = new TLWriteBuffer();

        var liteQueryWriter = new TLWriteBuffer();
        liteQueryWriter.WriteUInt32(Codes.Query);
        liteQueryWriter.WriteBuffer(methodWriter.Build());

        writer.WriteUInt32(Codes.QueryId);
        writer.WriteInt256(new BigInteger(queryId));
        writer.WriteBuffer(liteQueryWriter.Build());

        return (queryId, writer.Build());
    }
}