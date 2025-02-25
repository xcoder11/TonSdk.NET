using System.Collections.Generic;
using System.Numerics;
using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient.Queries;

public class GetLibrariesQuery(BigInteger[] libraryList) : LiteClientComplexQuery<LibraryEntry[]>
{
    public override uint Code => Codes.LibraryResult;

    public override LibraryEntry[] Decode(TLReadBuffer buffer)
    {
        var count = buffer.ReadUInt32();
        var list = new List<LibraryEntry>();
        for (var i = 0; i < count; i++)
        {
            var hash = new BigInteger(buffer.ReadInt256());
            var data = buffer.ReadBuffer();
            list.Add(new LibraryEntry
            {
                Data = data,
                Hash = hash
            });
        }

        return list.ToArray();
    }

    protected override void EncodeInternal(TLWriteBuffer writer)
    {
        writer.WriteUInt32((uint)libraryList.Length);
        foreach (var t in libraryList) writer.WriteInt256(t);
    }
}