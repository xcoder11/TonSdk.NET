namespace TonSdk.Adnl.LiteClient.Models;

public class BlockLinkBack : IBlockLink
{
    public bool ToKeyBlock { get; set; }
    public BlockIdExtended From { get; set; }
    public BlockIdExtended To { get; set; }
    public byte[] DestProof { get; set; }
    public byte[] Proof { get; set; }
    public byte[] StateProof { get; set; }
}