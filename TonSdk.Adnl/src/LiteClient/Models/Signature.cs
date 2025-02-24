using System.Numerics;

namespace TonSdk.Adnl.LiteClient.Models;

public class Signature
{
    public BigInteger NodeIdShort { get; set; }
    public byte[] SignatureBytes { get; set; }
}