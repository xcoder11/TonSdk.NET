using System.Numerics;

namespace TonSdk.Adnl.LiteClient.Models;

public class MasterChainInfoExtended
{
    public MasterChainInfoExtended(int version, long capabilities, int lastUTime, int now, BlockIdExtended lastBlockId,
        BlockIdExtended initBlockId, BigInteger stateRootHash)
    {
        Version = version;
        Capabilities = capabilities;
        LastUTime = lastUTime;
        Now = now;
        LastBlockId = lastBlockId;
        InitBlockId = initBlockId;
        StateRootHash = stateRootHash;
    }

    public int Version { get; set; }
    public long Capabilities { get; set; }
    public int LastUTime { get; set; }
    public int Now { get; set; }
    public BlockIdExtended LastBlockId { get; set; }
    public BlockIdExtended InitBlockId { get; set; }
    public BigInteger StateRootHash { get; set; }
}