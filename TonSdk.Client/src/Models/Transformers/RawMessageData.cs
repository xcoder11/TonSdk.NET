using TonSdk.Core.Boc;

namespace TonSdk.Client;

public struct RawMessageData
{
    public string Text;
    public Cell Body;
    public string InitState;

    internal RawMessageData(Transformers.OutRawMessageData outRawMessageData)
    {
        Text = outRawMessageData.Text ?? null;
        Body = outRawMessageData.Body != null ? Cell.From(outRawMessageData.Body) : null;
        InitState = outRawMessageData.InitState ?? null;
    }
    internal RawMessageData(Transformers.OutV3RawMessageData outRawMessageData)
    {
        Text = outRawMessageData.Decoded?.Comment;
        Body = outRawMessageData.Body != null ? Cell.From(outRawMessageData.Body) : null;
        InitState = null;
    }
}