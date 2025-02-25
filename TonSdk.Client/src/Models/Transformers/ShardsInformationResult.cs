using System;
using System.Linq;

namespace TonSdk.Client;

public struct ShardsInformationResult
{
    public BlockIdExtended[] Shards;

    internal ShardsInformationResult(Transformers.OutShardsInformationResult outShardsInformationResult)
    {
        Shards = outShardsInformationResult.Shards;
    }
        
    internal ShardsInformationResult(Transformers.OutV3ShardsInformationResult outShardsInformationResult)
    {
        Shards = outShardsInformationResult.Blocks
            .Where(shard => shard.Workchain != -1)
            .Select(shard => new BlockIdExtended(shard.Workchain, shard.RootHash, shard.FileHash, Convert.ToInt64(shard.Shard, 16), Convert.ToInt32(shard.Seqno)))
            .ToArray();
    }
}