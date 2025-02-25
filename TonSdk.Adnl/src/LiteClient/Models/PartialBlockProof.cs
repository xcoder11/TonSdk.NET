namespace TonSdk.Adnl.LiteClient.Models;

public class PartialBlockProof
{
    public bool Complete { get; set; }
    public BlockIdExtended From { get; set; }
    public BlockIdExtended To { get; set; }
    public IBlockLink[] BlockLinks { get; set; }
}