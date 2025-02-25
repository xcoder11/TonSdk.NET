using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using TonSdk.Adnl.Adnl;
using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Adnl.LiteClient.Queries;
using TonSdk.Adnl.TL;
using TonSdk.Core;

namespace TonSdk.Adnl.LiteClient;

public class LiteClient
{
    private readonly AdnlClientTcp _adnlClient;
    private Dictionary<string, TaskCompletionSource<TLReadBuffer>> _pendingRequests = new();

    public LiteClient(AdnlClientTcp adnlClient)
    {
        _adnlClient = adnlClient;
        _adnlClient.DataReceived += OnDataReceived;
    }

    public LiteClient(int host, int port, byte[] peerPublicKey) : this(new AdnlClientTcp(host, port, peerPublicKey))
    {
    }

    public LiteClient(int host, int port, string peerPublicKey) : this(new AdnlClientTcp(host, port, peerPublicKey))
    {
    }

    public LiteClient(string host, int port, byte[] peerPublicKey) : this(new AdnlClientTcp(host, port, peerPublicKey))
    {
    }

    public LiteClient(string host, int port, string peerPublicKey) : this(new AdnlClientTcp(host, port, peerPublicKey))
    {
    }

    public async Task Connect(CancellationToken ct = default)
    {
        if (_adnlClient.State == AdnlClientState.Open) return;
        _pendingRequests = new Dictionary<string, TaskCompletionSource<TLReadBuffer>>();
        await _adnlClient.Connect(ct);
        while (_adnlClient.State != AdnlClientState.Open)
        {
            if (ct.IsCancellationRequested) return;
            await Task.Delay(150, ct);
        }
    }

    public void Disconnect()
    {
        if (_adnlClient.State != AdnlClientState.Open) return;
        _pendingRequests = new Dictionary<string, TaskCompletionSource<TLReadBuffer>>();
        _adnlClient.End();
    }

    public async Task<TResult> Execute<TResult>(ILiteClientQuery<TResult> query, CancellationToken ct = default)
    {
        if (_adnlClient.State != AdnlClientState.Open)
            throw new Exception(
                "Connection to lite server must be init before method calling. Use Connect() method to set up connection.");
        var (id, data) = query.Encode();
        var tcs = new TaskCompletionSource<TLReadBuffer>();
        _pendingRequests.Add(Core.Crypto.Utils.BytesToHex(id), tcs);
        await _adnlClient.Write(data, ct);
        var payload = await tcs.Task;
        return query.Decode(payload);
    }

    public async Task PingPong(CancellationToken ct = default)
    {
        await _adnlClient.Write(new PingPongQuery().Encode().data, ct);
    }

    public async Task<MasterChainInfo> GetMasterChainInfo(CancellationToken ct = default)
    {
        return await Execute(GetMasterChainInfoQuery.Instance, ct);
    }

    public async Task<MasterChainInfoExtended> GetMasterChainInfoExtended(CancellationToken ct = default)
    {
        return await Execute(GetMasterChainInfoExtQuery.Instance, ct);
    }

    public async Task<int> GetTime(CancellationToken ct = default)
    {
        return await Execute(GetTimeQuery.Instance, ct);
    }

    public async Task<ChainVersion> GetVersion(CancellationToken ct = default)
    {
        return await Execute(GetVersionQuery.Instance, ct);
    }

    public async Task<byte[]> GetBlock(BlockIdExtended block, CancellationToken ct = default)
    {
        return await Execute(new GetBlockQuery(block), ct);
    }


    public async Task<BlockHeader> GetBlockHeader(BlockIdExtended block, CancellationToken ct = default)
    {
        return await Execute(new GetBlockHeaderQuery(block), ct);
    }

    public async Task<int> SendMessage(byte[] body, CancellationToken ct = default)
    {
        return await Execute(new SendMessageQuery(body), ct);
    }

    public async Task<AccountStateResult> GetAccountState(Address address, BlockIdExtended? blockIdExtended = null,
        CancellationToken ct = default)
    {
        return await Execute(new FetchAccountStateQuery(blockIdExtended ?? await GetCurrentBlockId(ct), address,
                "liteServer.getAccountState id:tonNode.blockIdExt account:liteServer.accountId = liteServer.AccountState"),
            ct);
    }

    private async Task<BlockIdExtended> GetCurrentBlockId(CancellationToken ct = default)
    {
        return (await GetMasterChainInfo(ct)).LastBlockId;
    }

    public async Task<AccountStateResult> GetAccountStatePrunned(Address address,
        BlockIdExtended? blockIdExtended = null, CancellationToken ct = default)
    {
        return await Execute(new FetchAccountStateQuery(blockIdExtended ?? await GetCurrentBlockId(ct), address,
                "liteServer.getAccountStatePrunned id:tonNode.blockIdExt account:liteServer.accountId = liteServer.AccountState"),
            ct);
    }

    public async Task<ShardInfo> GetShardInfo(int workchain, long shard, bool exact = false,
        BlockIdExtended? blockIdExtended = null, CancellationToken ct = default)
    {
        return await Execute(
            new GetShardInfoQuery(blockIdExtended ?? await GetCurrentBlockId(ct), workchain, shard, exact), ct);
    }

    public async Task<byte[]>
        GetAllShardsInfo(BlockIdExtended? blockIdExtended = null, CancellationToken ct = default)
    {
        return await Execute(new GetAllShardsInfoQuery(blockIdExtended ?? await GetCurrentBlockId(ct)), ct);
    }


    public async Task<byte[]> GetTransactions(uint count, Address account, long lt, string hash,
        CancellationToken ct = default)
    {
        return await Execute(new GetTransactionsQuery(account, lt, hash, count), ct);
    }

    public async Task<BlockHeader> LookUpBlock(int workchain, long shard, long? seqno = null, ulong? lt = null,
        ulong? uTime = null, CancellationToken ct = default)
    {
        return await Execute(new GetLookUpBlockQuery(workchain, shard, seqno, lt, uTime), ct);
    }

    public async Task<ListBlockTransactionsResult> ListBlockTransactions(BlockIdExtended blockIdExtended, uint count,
        ITransactionId? after = null, bool? reverseOrder = null, bool? wantProof = null, CancellationToken ct = default)
    {
        return await Execute(new ListBlockTransactionsQuery(blockIdExtended, count, after, reverseOrder, wantProof),
            ct);
    }


    public async Task<ListBlockTransactionsExtendedResult> ListBlockTransactionsExtended(
        BlockIdExtended blockIdExtended, uint count, ITransactionId? after = null, bool? reverseOrder = null,
        bool? wantProof = null, CancellationToken ct = default)
    {
        return await Execute(new ListBlockTransactionsExtQuery(blockIdExtended, count, after, reverseOrder, wantProof),
            ct);
    }


    public async Task<ConfigInfo> GetConfigAll(BlockIdExtended? blockIdExtended = null, CancellationToken ct = default)
    {
        return await Execute(new GetConfigAllQuery(blockIdExtended ?? await GetCurrentBlockId(ct)), ct);
    }

    public async Task<ConfigInfo> GetConfigParams(int[] paramIds, BlockIdExtended? blockIdExtended = null,
        CancellationToken ct = default)
    {
        return await Execute(new GetConfigParamsQuery(blockIdExtended ?? await GetCurrentBlockId(ct), paramIds), ct);
    }

    public async Task<LibraryEntry[]> GetLibraries(BigInteger[] libraryList, CancellationToken ct = default)
    {
        return await Execute(new GetLibrariesQuery(libraryList), ct);
    }

    public async Task<ShardBlockProof> GetShardBlockProof(BlockIdExtended blockIdExtended,
        CancellationToken ct = default)
    {
        return await Execute(new GetShardBlockProofQuery(blockIdExtended), ct);
    }

    public async Task<RunSmcMethodResult> RunSmcMethod(Address address, string methodName, byte[] stack,
        RunSmcOptions options, BlockIdExtended? blockIdExtended = null, CancellationToken ct = default)
    {
        return await Execute(
            new RunSmcMethodQuery(address, methodName, stack, options, blockIdExtended ?? await GetCurrentBlockId(ct)),
            ct);
    }

    private void OnDataReceived(byte[] data)
    {
        var readBuffer = new TLReadBuffer(data);

        var answer = readBuffer.ReadUInt32();

        if (answer == Codes.Pong) return;

        var queryId = Core.Crypto.Utils.BytesToHex(readBuffer.ReadInt256()); // queryId
        var liteQuery = readBuffer.ReadBuffer();
        var liteQueryBuffer = new TLReadBuffer(liteQuery);
        var responseCode = liteQueryBuffer.ReadUInt32(); // liteQuery

        if (responseCode == Codes.Error)
        {
            var code = liteQueryBuffer.ReadInt32();
            var message = liteQueryBuffer.ReadString();
            Console.WriteLine("Error: " + message + ". Code: " + code);
            _pendingRequests[queryId].SetException(new Exception("Error: " + message + ". Code: " + code));
            _pendingRequests.Remove(queryId);
            return;
        }


        if (!_pendingRequests.TryGetValue(queryId, out TaskCompletionSource<TLReadBuffer>? value))
        {
            Console.Out.WriteLineAsync("Response id doesn't match any request's id");
            return;
        }

        value.SetResult(liteQueryBuffer);
        _pendingRequests.Remove(queryId);
    }
}