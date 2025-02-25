using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetConfigAllQuery(BlockIdExtended block) : LiteClientComplexQuery<ConfigInfo>
{
    public override uint Code => Codes.ConfigInfo;

    public override ConfigInfo Decode(TLReadBuffer buffer)
    {
        // mode:#
        buffer.ReadUInt32();

        // id:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        var stateProof = buffer.ReadBuffer();
        var configProof = buffer.ReadBuffer();

        return new ConfigInfo
        {
            StateProof = stateProof,
            ConfigProof = configProof
        };
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        writer.WriteUInt32(1);
        FillBlockPart(writer, block);
    }
}