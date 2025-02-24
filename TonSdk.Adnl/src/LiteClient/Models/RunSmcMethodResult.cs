namespace TonSdk.Adnl.LiteClient.Models;

public class RunSmcMethodResult
{
    public RunSmcMethodResult(byte[] shardProof, byte[] proof, byte[] stateProof, byte[] initC7, byte[] libExtras,
        int exitCode, byte[] result)
    {
        ShardProof = shardProof;
        Proof = proof;
        StateProof = stateProof;
        InitC7 = initC7;
        LibExtras = libExtras;
        ExitCode = exitCode;
        Result = result;
    }

    public byte[] ShardProof { get; set; }
    public byte[] Proof { get; set; }
    public byte[] StateProof { get; set; }
    public byte[] InitC7 { get; set; }
    public byte[] LibExtras { get; set; }
    public int ExitCode { get; set; }
    public byte[] Result { get; set; }
}