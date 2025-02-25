using System;

namespace TonSdk.Adnl.LiteClient.Models;

[Flags]
internal enum RunSmcModes
{
    None = 0,
    ShardProof = 1 << 0, // mode.0
    Proof = 1 << 0, // mode.0
    StateProof = 1 << 1, // mode.1
    InitC7 = 1 << 3, // mode.3
    LibExtras = 1 << 4, // mode.4
    Result = 1 << 2 // mode.2
}