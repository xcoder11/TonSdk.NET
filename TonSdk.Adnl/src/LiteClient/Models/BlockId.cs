namespace TonSdk.Adnl.LiteClient.Models;

public class BlockId
{
    public BlockId(int workchain, long shard, long seqno)
    {
        Workchain = workchain;
        Shard = shard;
        Seqno = seqno;
    }

    public int Workchain { get; set; }
    public long Shard { get; set; }
    public long Seqno { get; set; }
}