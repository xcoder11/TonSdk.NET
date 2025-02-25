using TonSdk.Core;
using TonSdk.Core.Boc;

namespace TonSdk.Client;

public struct AddressInformationResult
{
    public AccountState State;
    public Coins Balance;
    public Cell Code;
    public Cell Data;
    public TransactionId LastTransactionId;
    public string FrozenHash;

    internal AddressInformationResult(Transformers.OutV3AddressInformationResult outAddressInformationResult)
    {
        switch (outAddressInformationResult.Status)
        {
            case "active":
            {
                State = AccountState.Active;
                break;
            }
            case "frozen":
            {
                State = AccountState.Frozen;
                break;
            }
            case "uninitialized":
            {
                State = AccountState.Uninit;
                break;
            }
            default:
            {
                State = AccountState.NonExist;
                break;
            }
        }

        Balance = new Coins(outAddressInformationResult.Balance, new CoinsOptions(true, 9));
        Code = string.IsNullOrEmpty(outAddressInformationResult.Code) ? null : Cell.From(outAddressInformationResult.Code);
        Data = string.IsNullOrEmpty(outAddressInformationResult.Data) ? null : Cell.From(outAddressInformationResult.Data);
        LastTransactionId = new TransactionId()
        {
            Hash = outAddressInformationResult.LastTransactionHash,
            Lt = ulong.Parse(string.IsNullOrEmpty(outAddressInformationResult.LastTransactionLt) ? "0" : outAddressInformationResult.LastTransactionLt)
        };
        // BlockId = outAddressInformationResult.BlockId;
        FrozenHash = outAddressInformationResult.FrozenHash;
        // SyncUtime = outAddressInformationResult.SyncUtime;
    }
        
    internal AddressInformationResult(Transformers.OutAddressInformationResult outAddressInformationResult)
    {
        switch (outAddressInformationResult.State)
        {
            case "active":
            {
                State = AccountState.Active;
                break;
            }
            case "frozen":
            {
                State = AccountState.Frozen;
                break;
            }
            case "uninitialized":
            {
                State = AccountState.Uninit;
                break;
            }
            default:
            {
                State = AccountState.NonExist;
                break;
            }
        }

        Balance = new Coins(outAddressInformationResult.Balance, new CoinsOptions(true, 9));
        Code = outAddressInformationResult.Code == "" ? null : Cell.From(outAddressInformationResult.Code);
        Data = outAddressInformationResult.Data == "" ? null : Cell.From(outAddressInformationResult.Data);
        LastTransactionId = outAddressInformationResult.LastTransactionId;
        // BlockId = outAddressInformationResult.BlockId;
        FrozenHash = outAddressInformationResult.FrozenHash;
        // SyncUtime = outAddressInformationResult.SyncUtime;
    }
}