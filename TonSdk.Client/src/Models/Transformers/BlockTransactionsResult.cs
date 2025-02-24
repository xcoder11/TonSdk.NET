namespace TonSdk.Client;

public struct BlockTransactionsResult
{
    public BlockIdExtended Id;
    public int ReqCount;
    public bool Incomplete;
    public ShortTransactionsResult[] Transactions;

    internal BlockTransactionsResult(Transformers.OutBlockTransactionsResult outBlockTransactionsResult)
    {
        Id = outBlockTransactionsResult.Id;
        ReqCount = outBlockTransactionsResult.ReqCount;
        Incomplete = outBlockTransactionsResult.Incomplete;
        Transactions = outBlockTransactionsResult.Transactions;
    }
}