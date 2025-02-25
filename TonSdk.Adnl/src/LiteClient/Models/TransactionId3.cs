using TonSdk.Core;

namespace TonSdk.Adnl.LiteClient.Models;

public class TransactionId3 : ITransactionId
{
    public TransactionId3(Address account, long lt)
    {
        Account = account;
        Lt = lt;
    }

    public Address Account { get; set; }
    public long Lt { get; set; }
}