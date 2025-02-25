namespace TonSdk.Adnl.LiteClient.Models;

public class AccountStateResult
{
    public byte[] ShardProof { get; set; }
    public byte[] Proof { get; set; }
    public byte[] State { get; set; }
}