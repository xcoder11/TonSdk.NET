using Newtonsoft.Json;

namespace TonSdk.Client;

public struct EstimateFeeResult : IEstimateFeeResult
{
    [JsonProperty("@type")] public string Type;
    [JsonProperty("source_fees")] public SourceFees SourceFees { get; set; }
}