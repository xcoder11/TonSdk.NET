using TonSdk.Core.Boc;

namespace TonSdk.Client;

public struct ConfigParamResult
{
    public Cell Bytes;

    internal ConfigParamResult(Transformers.OutConfigParamResult outConfigParamResult)
    {
        Bytes = Cell.From(outConfigParamResult.Bytes);
    }
}