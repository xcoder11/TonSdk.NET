namespace TonSdk.Adnl.LiteClient.Models;

public class RunSmcOptions
{
    public bool ShardProof { get; set; }
    public bool Proof { get; set; }
    public bool StateProof { get; set; }
    public bool InitC7 { get; set; }
    public bool LibExtras { get; set; }
    public bool Result { get; set; }
}