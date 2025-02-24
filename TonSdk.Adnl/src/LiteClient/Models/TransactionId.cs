namespace TonSdk.Adnl.LiteClient.Models;

public class TransactionId : ITransactionId
{
    public byte[] Account { get; set; }
    public long Lt { get; set; }

    public byte[] Hash { get; set; }
}