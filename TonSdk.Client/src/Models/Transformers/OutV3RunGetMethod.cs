using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TonSdk.Client;

internal struct OutV3RunGetMethod
{
    [JsonProperty("gas_used")] public int GasUsed;
    [JsonProperty("stack")] public JObject[] Stack;
    [JsonProperty("exit_code")] public int ExitCode;
}