namespace TonSdk.Adnl.LiteClient.Models;

public class AllShardsInfo
{
    public AllShardsInfo(byte[] data)
    {
        Data = data;
    }

    public byte[] Data { get; set; }
}