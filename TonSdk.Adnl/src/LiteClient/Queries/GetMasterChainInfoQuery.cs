using System.Numerics;
using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetMasterChainInfoQuery : LiteClientSimpleQuery<MasterChainInfo>
{
    public static GetMasterChainInfoQuery Instance { get; } = new();
    public override uint Code => Codes.MasterchainInfo;

    public override MasterChainInfo Decode(TLReadBuffer buffer)
    {
        // last:tonNode.blockIdExt
        var workchain = buffer.ReadInt32();
        var shard = buffer.ReadInt64();
        var seqno = buffer.ReadInt32();
        var rootHash = buffer.ReadBytes(32);
        var fileHash = buffer.ReadBytes(32);

        // state_root_hash:int256
        var stateRootHash = new BigInteger(buffer.ReadInt256());

        // init:tonNode.zeroStateIdExt
        var workchainI = buffer.ReadInt32();
        var rootHashI = buffer.ReadBytes(32);
        var fileHashI = buffer.ReadBytes(32);

        var lastBlock = new BlockIdExtended(workchain, rootHash, fileHash, shard, seqno);
        var initBlock = new BlockIdExtended(workchainI, rootHashI, fileHashI, 0, 0);

        return new MasterChainInfo(lastBlock, initBlock, stateRootHash);
    }
}