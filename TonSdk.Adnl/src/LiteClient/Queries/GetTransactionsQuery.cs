using System;
using TonSdk.Adnl.TL;
using TonSdk.Core;
using TonSdk.Core.Boc;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetTransactionsQuery(Address account, long lt, string hash, uint count)
    : LiteClientComplexQuery<byte[]>
{
    public override uint Code => Codes.TransactionList;

    public override byte[] Decode(TLReadBuffer buffer)
    {
        var count = buffer.ReadUInt32();
        for (var i = 0; i < count; i++)
        {
            // id:tonNode.blockIdExt
            buffer.ReadInt32();
            buffer.ReadInt64();
            buffer.ReadInt32();
            buffer.ReadInt256();
            buffer.ReadInt256();
        }

        return buffer.ReadBuffer();
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        byte[] hashBytes;
        if (hash.isHexString()) hashBytes = Core.Crypto.Utils.HexToBytes(hash);
        else if (hash.isBase64()) hashBytes = Convert.FromBase64String(hash);
        else throw new Exception("Not valid hash string. Set only in hex or non-url base64.");

        writer.WriteInt32((int)count);

        writer.WriteInt32(account.GetWorkchain());
        writer.WriteBytes(account.GetHash(), 32);

        writer.WriteInt64(lt);
        writer.WriteBytes(hashBytes, 32);
    }
}