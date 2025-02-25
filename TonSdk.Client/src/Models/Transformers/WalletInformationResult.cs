using TonSdk.Core;

namespace TonSdk.Client;

public struct WalletInformationResult
{
    public bool IsWallet;
    public Coins Balance;
    public AccountState State;
    public string? WalletType;
    public long? Seqno;
    public TransactionId LastTransactionId;
    public long? WalletId;

    internal WalletInformationResult(AddressInformationResult addressInformationResult)
    {
        WalletType = null;
        Seqno = null;
        WalletId = null;
        IsWallet = false;

        Balance = addressInformationResult.Balance;
        LastTransactionId = addressInformationResult.LastTransactionId;
        State = addressInformationResult.State;
    }
        
    internal WalletInformationResult(Transformers.OutV3WalletInformationResult walletInformationResult)
    {
        Seqno = long.Parse(walletInformationResult.Seqno);
        WalletType = walletInformationResult.WalletType;
        WalletId = long.Parse(walletInformationResult.WalletId);
        IsWallet = true;
            
        Balance = new Coins(walletInformationResult.Balance, new CoinsOptions(true, 9));
        LastTransactionId = new TransactionId()
        {
            Hash =  walletInformationResult.LastTransactionHash,
            Lt =  ulong.Parse(walletInformationResult.LastTransactionLt)
        };
        switch (walletInformationResult.Status)
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
    }

    internal WalletInformationResult(Transformers.OutWalletInformationResult walletInformationResult)
    {
        IsWallet = bool.Parse(walletInformationResult.IsWallet);
        if (IsWallet)
        {
            WalletType = walletInformationResult.WalletType;
            Seqno = long.Parse(walletInformationResult.Seqno);
            WalletId = long.Parse(walletInformationResult.WalletId);
        }
        else
        {
            WalletType = null;
            Seqno = null;
            WalletId = null;
        }

        Balance = new Coins(walletInformationResult.Balance, new CoinsOptions(true, 9));
        LastTransactionId = walletInformationResult.LastTransactionId;
        switch (walletInformationResult.State)
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
    }
}