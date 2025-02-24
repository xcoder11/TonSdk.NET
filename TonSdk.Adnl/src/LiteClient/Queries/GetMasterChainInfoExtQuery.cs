using System.Numerics;
using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetMasterChainInfoExtQuery : LiteClientComplexQuery<MasterChainInfoExtended>
{
    public static GetMasterChainInfoExtQuery Instance { get; } = new();
    public override uint Code => Codes.MasterchainInfoExt;

    public override MasterChainInfoExtended Decode(TLReadBuffer buffer)
    {
        // mode:#
        buffer.ReadUInt32();

        // version:int
        var version = buffer.ReadInt32();

        // capabilities:long
        var capabilities = buffer.ReadInt64();

        // last:tonNode.blockIdExt
        var workchain = buffer.ReadInt32();
        var shard = buffer.ReadInt64();
        var seqno = buffer.ReadInt32();
        var rootHash = buffer.ReadBytes(32);
        var fileHash = buffer.ReadBytes(32);

        // last_uTime:int
        var lastUTime = buffer.ReadInt32();
        // now:int
        var time = buffer.ReadInt32();
        // state_root_hash:int256
        var stateRootHash = new BigInteger(buffer.ReadInt256());

        // init:tonNode.zeroStateIdExt
        var workchainI = buffer.ReadInt32();
        var rootHashI = buffer.ReadBytes(32);
        var fileHashI = buffer.ReadBytes(32);

        var lastBlock = new BlockIdExtended(workchain, rootHash, fileHash, shard, seqno);
        var initBlock = new BlockIdExtended(workchainI, rootHashI, fileHashI, 0, 0);

        return new MasterChainInfoExtended(version, capabilities, lastUTime, time, lastBlock, initBlock,
            stateRootHash);
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        writer.WriteUInt32(0);
    }
}