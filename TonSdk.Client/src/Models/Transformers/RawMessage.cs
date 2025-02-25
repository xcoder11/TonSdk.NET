using TonSdk.Core;

namespace TonSdk.Client;

public struct RawMessage
{
    public string Hash;
    public Address Source;
    public Address Destination;
    public Coins Value;
    public Coins FwdFee;
    public Coins IhrFee;
    public ulong CreatedLt;
    public string OpCode;
    public string BodyHash;
    public RawMessageData MsgData;
    public string Message;

    internal RawMessage(Transformers.OutRawMessage outRawMessage)
    {
        Source = outRawMessage.Source != null && outRawMessage.Source.Length != 0
            ? new Address(outRawMessage.Source)
            : null;
        Destination = outRawMessage.Destination != null && outRawMessage.Destination.Length != 0
            ? new Address(outRawMessage.Destination)
            : null;
        Value = new Coins(outRawMessage.Value, new CoinsOptions(true, 9));
        FwdFee = new Coins(outRawMessage.FwdFee, new CoinsOptions(true, 9));
        IhrFee = new Coins(outRawMessage.IhrFee, new CoinsOptions(true, 9));
        CreatedLt = outRawMessage.CreaterLt;
        BodyHash = outRawMessage.BodyHash;
        MsgData = new RawMessageData(outRawMessage.MsgData);
        Message = outRawMessage.Message;
        Hash = "";
        OpCode = MsgData.Body != null && MsgData.Body.BitsCount >= 32
            ? $"0x{MsgData.Body.Parse().LoadUInt(32).ToString("X")}"
            : "";
    }
        
    internal RawMessage(Transformers.OutV3RawMessage outRawMessage)
    {
        Source = !string.IsNullOrEmpty(outRawMessage.Source)
            ? new Address(outRawMessage.Source)
            : null;
        Destination = new Address(outRawMessage.Destination);
        Value = new Coins(outRawMessage.Value, new CoinsOptions(true, 9));
        FwdFee = new Coins(outRawMessage.FwdFee, new CoinsOptions(true, 9));
        IhrFee = new Coins(outRawMessage.IhrFee, new CoinsOptions(true, 9));
        CreatedLt = outRawMessage.CreatedLt;
        BodyHash = outRawMessage.MsgData.BodyHash;
        MsgData = new RawMessageData(outRawMessage.MsgData);
        Message = null;
        Hash = outRawMessage.Hash;
        OpCode = outRawMessage.OpCode ?? "";
    }
}