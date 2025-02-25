namespace TonSdk.Adnl.LiteClient.Models;

public class ListBlockTransactionsResult
{
    public ListBlockTransactionsResult(bool inComplete, TransactionId[] transactionIds, byte[] proof)
    {
        InComplete = inComplete;
        TransactionIds = transactionIds;
        Proof = proof;
    }

    public bool InComplete { get; set; }
    public TransactionId[] TransactionIds { get; set; }
    public byte[] Proof { get; set; }
}