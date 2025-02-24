using Newtonsoft.Json;

namespace TonSdk.Client;

public struct ShortTransactionsResult
{
    [JsonProperty("mode")] public int Mode;
    [JsonProperty("account")] public string Account;
    [JsonProperty("lt")] public ulong Lt;
    [JsonProperty("hash")] public string Hash;
}