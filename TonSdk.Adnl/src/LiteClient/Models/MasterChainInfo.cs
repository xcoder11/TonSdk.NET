using System.Numerics;

namespace TonSdk.Adnl.LiteClient.Models;

public class MasterChainInfo
{
    public MasterChainInfo(BlockIdExtended lastBlockId, BlockIdExtended initBlockId, BigInteger stateRootHash)
    {
        LastBlockId = lastBlockId;
        InitBlockId = initBlockId;
        StateRootHash = stateRootHash;
    }

    public BlockIdExtended LastBlockId { get; set; }
    public BlockIdExtended InitBlockId { get; set; }
    public BigInteger StateRootHash { get; set; }
}