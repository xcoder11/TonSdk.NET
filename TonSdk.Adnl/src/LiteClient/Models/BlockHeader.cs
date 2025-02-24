namespace TonSdk.Adnl.LiteClient.Models;

public class BlockHeader
{
    public BlockIdExtended BlockId { get; set; }
    public byte[] HeaderProof { get; set; }
}