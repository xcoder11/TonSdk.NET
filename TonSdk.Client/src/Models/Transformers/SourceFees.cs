using Newtonsoft.Json;

namespace TonSdk.Client;

public struct SourceFees
{
    [JsonProperty("@type")] public string Type;
    [JsonProperty("in_fwd_fee")] public long InFwdFee;
    [JsonProperty("storage_fee")] public long StorageFee;
    [JsonProperty("gas_fee")] public long GasFee;
    [JsonProperty("fwd_fee")] public long FwdFee;
}