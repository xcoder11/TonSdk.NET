using System.Text;
using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;
using TonSdk.Adnl.Utils;
using TonSdk.Core;

namespace TonSdk.Adnl.LiteClient.Queries;

public class RunSmcMethodQuery(
    Address account,
    string methodName,
    byte[] stack,
    RunSmcOptions options,
    BlockIdExtended block) : LiteClientComplexQuery<RunSmcMethodResult>
{
    public override uint Code => Codes.RunMethodResult;

    public override RunSmcMethodResult Decode(TLReadBuffer buffer)
    {
        var mode = buffer.ReadUInt32();
        // id:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        // shardblk:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        var shardProof = (mode & (1 << 0)) != 0 ? buffer.ReadBuffer() : [];
        var proof = (mode & (1 << 0)) != 0 ? buffer.ReadBuffer() : [];
        var stateProof = (mode & (1 << 1)) != 0 ? buffer.ReadBuffer() : [];
        var initC7 = (mode & (1 << 3)) != 0 ? buffer.ReadBuffer() : [];
        var libExtras = (mode & (1 << 4)) != 0 ? buffer.ReadBuffer() : [];
        var exitCode = buffer.ReadInt32();
        var result = (mode & (1 << 2)) != 0 ? buffer.ReadBuffer() : [];

        return new RunSmcMethodResult(shardProof, proof, stateProof, initC7, libExtras, exitCode, result);
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        var crc = Crc32.CalculateCrc16Xmodem(Encoding.UTF8.GetBytes(methodName));
        var crcExtended = (ulong)(crc & 0xffff) | 0x10000;

        uint mode = 0;
        if (options.ShardProof || options.Proof)
            mode |= 1u << 0;
        if (options.StateProof)
            mode |= 1u << 1;
        if (options.Result)
            mode |= 1u << 2;
        if (options.InitC7)
            mode |= 1u << 3;
        if (options.LibExtras)
            mode |= 1u << 4;

        writer.WriteUInt32(mode);

        FillBlockPart(writer, block);

        writer.WriteInt32(account.GetWorkchain());
        writer.WriteBytes(account.GetHash(), 32);

        writer.WriteInt64((long)crcExtended);
        writer.WriteBuffer(stack);
    }
}