namespace TonSdk.Adnl.LiteClient.Models;

public class ValidatorStats
{
    public int Count { get; set; }
    public bool Complete { get; set; }
    public byte[] StateProof { get; set; }
    public byte[] DataProof { get; set; }
}