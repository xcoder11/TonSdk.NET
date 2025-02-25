namespace TonSdk.Adnl.LiteClient.Models;

public class BlockIdExtended
{
    public BlockIdExtended(int workchain, byte[] rootHash, byte[] fileHash, long shard, int seqno)
    {
        Workchain = workchain;
        RootHash = rootHash;
        FileHash = fileHash;
        Shard = shard;
        Seqno = seqno;
    }

    public BlockIdExtended(int workchain, long shard, int seqno)
    {
        Workchain = workchain;
        Shard = shard;
        Seqno = seqno;
        RootHash = [];
        FileHash = [];
    }

    public int Workchain { get; set; }
    public long Shard { get; set; }
    public int Seqno { get; set; }
    public byte[] RootHash { get; set; }
    public byte[] FileHash { get; set; }
}