using System;
using TonSdk.Adnl.LiteClient;
using TonSdk.Adnl.LiteClient.Models;

namespace TonSdk.Client;

public struct MasterchainInformationResult
{
    public BlockIdExtended LastBlock;
    public BlockIdExtended InitBlock;
    public string? StateRootHash;

    internal MasterchainInformationResult(Transformers.OutMasterchanInformationResult outAddressInformationResult)
    {
        LastBlock = outAddressInformationResult.LastBlock;
        InitBlock = outAddressInformationResult.InitBlock;
        StateRootHash = outAddressInformationResult.StateRootHash;
    }
        
    internal MasterchainInformationResult(Transformers.OutV3MasterchainInformationResult outAddressInformationResult)
    {
        LastBlock = outAddressInformationResult.LastBlock;
        InitBlock = outAddressInformationResult.InitBlock;
        StateRootHash = null;
    }
        
    internal MasterchainInformationResult(MasterChainInfo masterChainInfo)
    {
        LastBlock = new BlockIdExtended(masterChainInfo.LastBlockId);
        InitBlock = new BlockIdExtended(masterChainInfo.InitBlockId);
        StateRootHash = Convert.ToBase64String(masterChainInfo.StateRootHash.ToByteArray());
    }
}