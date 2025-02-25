namespace TonSdk.Adnl.LiteClient.Models;

public class BlockLinkForward : IBlockLink
{
    public bool ToKeyBlock { get; set; }
    public BlockIdExtended From { get; set; }
    public BlockIdExtended To { get; set; }
    public byte[] DestProof { get; set; }
    public byte[] ConfigProof { get; set; }
    public int ValidatorSetHash { get; set; }
    public int CatchainSeqno { get; set; }
    public Signature[] Signatures { get; set; }
}