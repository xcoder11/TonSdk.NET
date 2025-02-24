using Newtonsoft.Json;

namespace TonSdk.Client;

public struct SendBocResult
{
    [JsonProperty("@type")] public string Type;
    [JsonProperty("hash")] public string Hash;
}