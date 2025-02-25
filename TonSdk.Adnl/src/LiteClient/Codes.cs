using TonSdk.Adnl.Utils;

namespace TonSdk.Adnl.LiteClient;

public static class Codes
{
    public static readonly uint Ping = Crc32.Code("tcp.ping random_id:long = tcp.Pong");
    public static readonly uint Pong = Crc32.Code("tcp.pong random_id:long = tcp.Pong");

    public static readonly uint MasterchainInfo =
        Crc32.Code("liteServer.getMasterchainInfo = liteServer.MasterchainInfo");

    public static readonly uint MasterchainInfoExt =
        Crc32.Code("liteServer.getMasterchainInfoExt mode:# = liteServer.MasterchainInfoExt");

    public static readonly uint GetTime = Crc32.Code("liteServer.getTime = liteServer.CurrentTime");
    public static readonly uint Query = Crc32.Code("liteServer.query data:bytes = Object");
    public static readonly uint QueryId = Crc32.Code("adnl.message.query query_id:int256 query:bytes = adnl.Message");
    public static readonly uint Version = Crc32.Code("liteServer.getVersion = liteServer.Version");
    public static readonly uint Error = Crc32.Code("liteServer.error code:int message:string = liteServer.Error");

    public static readonly uint BlockData =
        Crc32.Code("liteServer.getBlock id:tonNode.blockIdExt = liteServer.BlockData");

    public static readonly uint BlockHeader =
        Crc32.Code("liteServer.getBlockHeader id:tonNode.blockIdExt mode:# = liteServer.BlockHeader");

    public static readonly uint SendMsgStatus =
        Crc32.Code("liteServer.sendMessage body:bytes = liteServer.SendMsgStatus");

    public static readonly uint ShardInfo =
        Crc32.Code(
            "liteServer.getShardInfo id:tonNode.blockIdExt workchain:int shard:long exact:Bool = liteServer.ShardInfo");

    public static readonly uint AllShardsInfo =
        Crc32.Code("liteServer.getAllShardsInfo id:tonNode.blockIdExt = liteServer.AllShardsInfo");

    public static readonly uint TransactionList =
        Crc32.Code(
            "liteServer.getTransactions count:# account:liteServer.accountId lt:long hash:int256 = liteServer.TransactionList");

    public static readonly uint LookUpBlock =
        Crc32.Code(
            "liteServer.lookupBlock mode:# id:tonNode.blockId lt:mode.1?long utime:mode.2?int = liteServer.BlockHeader");

    public static readonly uint BlockTransactions =
        Crc32.Code(
            "liteServer.listBlockTransactions id:tonNode.blockIdExt mode:# count:# after:mode.7?liteServer.transactionId3 reverse_order:mode.6?true want_proof:mode.5?true = liteServer.BlockTransactions");

    public static readonly uint BlockTransactionsExt =
        Crc32.Code(
            "liteServer.listBlockTransactionsExt id:tonNode.blockIdExt mode:# count:# after:mode.7?liteServer.transactionId3 reverse_order:mode.6?true want_proof:mode.5?true = liteServer.BlockTransactionsExt");

    public static readonly uint ConfigInfo =
        Crc32.Code("liteServer.getConfigAll mode:# id:tonNode.blockIdExt = liteServer.ConfigInfo");

    public static readonly uint ConfigInfoParams = 705764377;

    public static readonly uint LibraryResult =
        Crc32.Code("liteServer.getLibraries library_list:(vector int256) = liteServer.LibraryResult");

    public static readonly uint ShardBlockProof =
        Crc32.Code("liteServer.getShardBlockProof id:tonNode.blockIdExt = liteServer.ShardBlockProof");

    public static readonly uint RunMethodResult =
        Crc32.Code(
            "liteServer.runSmcMethod mode:# id:tonNode.blockIdExt account:liteServer.accountId method_id:long params:bytes = liteServer.RunMethodResult");
}