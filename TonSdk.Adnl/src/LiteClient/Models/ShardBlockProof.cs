namespace TonSdk.Adnl.LiteClient.Models;

public class ShardBlockProof
{
    public BlockIdExtended MasterChainId { get; set; }
    public ShardBlockLink[] Links { get; set; }
}