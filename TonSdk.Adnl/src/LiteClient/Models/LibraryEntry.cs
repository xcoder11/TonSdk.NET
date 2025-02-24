using System.Numerics;

namespace TonSdk.Adnl.LiteClient.Models;

public class LibraryEntry
{
    public BigInteger Hash { get; set; }
    public byte[] Data { get; set; }
}