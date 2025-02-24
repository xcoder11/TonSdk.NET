using Newtonsoft.Json;

namespace TonSdk.Client;

internal struct OutRunGetMethod
{
    [JsonProperty("gas_used")] public int GasUsed;
    [JsonProperty("stack")] public object[][] Stack;
    [JsonProperty("exit_code")] public int ExitCode;
}