namespace TonSdk.Adnl.LiteClient.Models;

public class ListBlockTransactionsExtendedResult
{
    public ListBlockTransactionsExtendedResult(bool inComplete, byte[] transactions, byte[] proof)
    {
        InComplete = inComplete;
        Transactions = transactions;
        Proof = proof;
    }

    public bool InComplete { get; set; }
    public byte[] Transactions { get; set; }
    public byte[] Proof { get; set; }
}