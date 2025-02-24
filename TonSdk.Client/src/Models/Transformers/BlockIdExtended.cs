using System;
using Newtonsoft.Json;

namespace TonSdk.Client;

public class BlockIdExtended
{
    [JsonProperty("workchain")] public int Workchain;
    [JsonProperty("shard")] public long Shard;
    [JsonProperty("seqno")] public long Seqno;
    [JsonProperty("hash")] public string Hash;
    [JsonProperty("root_hash")] public string RootHash;
    [JsonProperty("file_hash")] public string FileHash;

    public BlockIdExtended()
    {
            
    }
        
    public BlockIdExtended(
        int workchain,
        string rootHash,
        string fileHash,
        long shard,
        int seqno)
    {
        Workchain = workchain;
        RootHash = rootHash;
        FileHash = fileHash;
        Shard = shard;
        Seqno = seqno;
    }
        
    public BlockIdExtended(Adnl.LiteClient.Models.BlockIdExtended blockIdExtended)
    {
        FileHash = Convert.ToBase64String(blockIdExtended.FileHash);
        RootHash = Convert.ToBase64String(blockIdExtended.RootHash);
        Seqno = blockIdExtended.Seqno;
        Shard = blockIdExtended.Shard;
        Workchain = blockIdExtended.Workchain;
    }
}
public static class BlockIdExtendedExtensions
{
    public static Adnl.LiteClient.Models.BlockIdExtended ConvertBlockIdToAdnlBase(this BlockIdExtended block)
    {
        return new Adnl.LiteClient.Models.BlockIdExtended(
            block.Workchain, 
            Convert.FromBase64String(block.RootHash),
            Convert.FromBase64String(block.FileHash),
            block.Shard, (int)block.Seqno);
    }
}