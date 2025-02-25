using TonSdk.Core;
using TonSdk.Core.Boc;

namespace TonSdk.Client;

public struct TransactionsInformationResult
{
    public Address Address;
    public uint UTime;
    public int OutMsgCount;
    public Cell Data;
    public TransactionId TransactionId;
    public TransactionId PrevTransactionId;
    public Coins Fee;
    public Coins StorageFee;
    public Coins OtherFee;
    public AccountState OrigAccountStatus;
    public AccountState EndAccountStatus;
    public RawMessage InMsg;
    public RawMessage[] OutMsgs;

    internal TransactionsInformationResult(Transformers.OutTransactionsResult outTransactionsResult)
    {
        Address = new Address(outTransactionsResult.Address.AccountAddress);
        UTime = (uint)outTransactionsResult.Utime;
        Data = Cell.From(outTransactionsResult.Data);
        TransactionId = outTransactionsResult.TransactionId;
        Fee = new Coins(outTransactionsResult.Fee, new CoinsOptions(true, 9));
        StorageFee = new Coins(outTransactionsResult.StorageFee, new CoinsOptions(true, 9));
        OtherFee = new Coins(outTransactionsResult.OtherFee, new CoinsOptions(true, 9));
        InMsg = new RawMessage(outTransactionsResult.InMsg);

        OutMsgs = new RawMessage[outTransactionsResult.OutMsgs.Length];
        for (int i = 0; i < outTransactionsResult.OutMsgs.Length; i++)
        {
            OutMsgs[i] = new RawMessage(outTransactionsResult.OutMsgs[i]);
        }

        OrigAccountStatus = AccountState.Active;
        EndAccountStatus = AccountState.Active;
        OutMsgCount = OutMsgs.Length;
        PrevTransactionId = new TransactionId();
    }
        
    internal TransactionsInformationResult(Transformers.OutV3TransactionsResult outTransactionsResult)
    {
        Address = new Address(outTransactionsResult.Account);
        UTime = (uint)outTransactionsResult.Now;
        Data = null;
        TransactionId = new TransactionId()
        {
            Hash = outTransactionsResult.Hash,
            Lt = outTransactionsResult.Lt
        };
        PrevTransactionId = new TransactionId()
        {
            Hash = outTransactionsResult.PrevTransHash,
            Lt = ulong.Parse(outTransactionsResult.PrevTransLt)
        };
        Fee = new Coins(outTransactionsResult.Fee, new CoinsOptions(true, 9));
        StorageFee = null;
        OtherFee = null;
        InMsg = new RawMessage(outTransactionsResult.InMsg);

        OutMsgs = new RawMessage[outTransactionsResult.OutMsgs.Length];
        for (int i = 0; i < outTransactionsResult.OutMsgs.Length; i++)
        {
            OutMsgs[i] = new RawMessage(outTransactionsResult.OutMsgs[i]);
        }
            
        OrigAccountStatus = AccountState.Active;
        EndAccountStatus = AccountState.Active;
        OutMsgCount = OutMsgs.Length;
    }
}