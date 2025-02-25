namespace TonSdk.Adnl.LiteClient.Models;

public class ChainVersion
{
    public ChainVersion(int version, long capabilities, int now)
    {
        Version = version;
        Capabilities = capabilities;
        Now = now;
    }

    public int Version { get; set; }
    public long Capabilities { get; set; }
    public int Now { get; set; }
}