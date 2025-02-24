using Newtonsoft.Json;

namespace TonSdk.Client;

public struct EstimateFeeResultExtended : IEstimateFeeResult
{
    [JsonProperty("source_fees")] public SourceFees SourceFees { get; set; }
    [JsonProperty("destination_fees")] public SourceFees[] DestinationFees { get; set; }
}