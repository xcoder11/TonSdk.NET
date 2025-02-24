using Newtonsoft.Json;

namespace TonSdk.Client;

public struct TransactionId
{
    [JsonProperty("lt")] public ulong Lt;
    [JsonProperty("hash")] public string Hash;
}