namespace TonSdk.Adnl.LiteClient.Models;

public class ShardBlockLink
{
    public BlockIdExtended BlockIdExtended { get; set; }
    public byte[] Proof { get; set; }
}