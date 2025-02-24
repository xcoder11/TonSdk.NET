namespace TonSdk.Client;

public struct BlockDataResult
{
    public BlockIdExtended BlockIdExtended;
    public long GlobalId;
    public uint Version;
    public int Flags;
    public bool AfterMerge;
    public bool AfterSplit;
    public bool BeforeSplit;
    public bool WantMerge;
    public bool WantSplit;
    public long ValidatorListHashShort;
    public long CatchainSeqno;
    public long MinRefMcSeqno;
    public bool IsKeyBlock;
    public long PrevKeyBlockSeqno;
    public ulong StartLt;
    public ulong EndLt;
    public long RgenUtime;
    public BlockIdExtended[] PrevBlocks;
        
    internal BlockDataResult(Transformers.OutBlockHeaderResult outBlockHeaderResult)
    {
        BlockIdExtended = outBlockHeaderResult.Id;
        GlobalId = outBlockHeaderResult.GlobalId;
        Version = outBlockHeaderResult.Version;
        Flags = outBlockHeaderResult.Flags;
        AfterMerge = outBlockHeaderResult.AfterMerge;
        AfterSplit = outBlockHeaderResult.AfterSplit;
        BeforeSplit = outBlockHeaderResult.BeforeSplit;
        WantMerge = outBlockHeaderResult.WantMerge;
        WantSplit = outBlockHeaderResult.WantSplit;
        ValidatorListHashShort = outBlockHeaderResult.ValidatorListHashShort;
        CatchainSeqno = outBlockHeaderResult.CatchainSeqno;
        MinRefMcSeqno = outBlockHeaderResult.MinRefMcSeqno;
        IsKeyBlock = outBlockHeaderResult.IsKeyBlock;
        PrevKeyBlockSeqno = outBlockHeaderResult.PrevKeyBlockSeqno;
        StartLt = outBlockHeaderResult.StartLt;
        EndLt = outBlockHeaderResult.EndLt;
        RgenUtime = outBlockHeaderResult.RgenUtime;
        PrevBlocks = outBlockHeaderResult.PrevBlocks;
    }
}