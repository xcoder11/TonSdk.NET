namespace TonSdk.Adnl.LiteClient.Models;

public class ShardInfo
{
    public ShardInfo(byte[] shardProof, byte[] shardDescr, BlockIdExtended shardBlock)
    {
        ShardBlock = shardBlock;
        ShardProof = shardProof;
        ShardDescr = shardDescr;
    }

    public byte[] ShardProof { get; set; }
    public byte[] ShardDescr { get; set; }

    public BlockIdExtended ShardBlock { get; set; }
}