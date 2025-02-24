namespace TonSdk.Client;

public struct BlockTransactionsResultExtended
{
    public BlockIdExtended Id;
    public int ReqCount;
    public bool Incomplete;
    public TransactionsInformationResult[] Transactions;
}