using System.Collections.Generic;
using System.Numerics;
using TonSdk.Adnl.TL;

namespace TonSdk.Adnl.LiteClient;

public class LiteClientDecoder
{
    internal static MasterChainInfo DecodeGetMasterchainInfo(TLReadBuffer buffer)
    {
        // last:tonNode.blockIdExt
        var workchain = buffer.ReadInt32();
        var shard = buffer.ReadInt64();
        var seqno = buffer.ReadInt32();
        var rootHash = buffer.ReadBytes(32);
        var fileHash = buffer.ReadBytes(32);

        // state_root_hash:int256
        var stateRootHash = new BigInteger(buffer.ReadInt256());

        // init:tonNode.zeroStateIdExt
        var workchainI = buffer.ReadInt32();
        var rootHashI = buffer.ReadBytes(32);
        var fileHashI = buffer.ReadBytes(32);

        var lastBlock = new BlockIdExtended(workchain, rootHash, fileHash, shard, seqno);
        var initBlock = new BlockIdExtended(workchainI, rootHashI, fileHashI, 0, 0);

        return new MasterChainInfo(lastBlock, initBlock, stateRootHash);
    }

    internal static MasterChainInfoExtended DecodeGetMasterchainInfoExtended(TLReadBuffer buffer)
    {
        // mode:#
        buffer.ReadUInt32();

        // version:int
        var version = buffer.ReadInt32();

        // capabilities:long
        var capabilities = buffer.ReadInt64();

        // last:tonNode.blockIdExt
        var workchain = buffer.ReadInt32();
        var shard = buffer.ReadInt64();
        var seqno = buffer.ReadInt32();
        var rootHash = buffer.ReadBytes(32);
        var fileHash = buffer.ReadBytes(32);

        // last_uTime:int
        var lastUTime = buffer.ReadInt32();
        // now:int
        var time = buffer.ReadInt32();
        // state_root_hash:int256
        var stateRootHash = new BigInteger(buffer.ReadInt256());

        // init:tonNode.zeroStateIdExt
        var workchainI = buffer.ReadInt32();
        var rootHashI = buffer.ReadBytes(32);
        var fileHashI = buffer.ReadBytes(32);

        var lastBlock = new BlockIdExtended(workchain, rootHash, fileHash, shard, seqno);
        var initBlock = new BlockIdExtended(workchainI, rootHashI, fileHashI, 0, 0);

        return new MasterChainInfoExtended(version, capabilities, lastUTime, time, lastBlock, initBlock,
            stateRootHash);
    }

    internal static int DecodeGetTime(TLReadBuffer buffer)
    {
        // now:int
        var time = buffer.ReadInt32();
        return time;
    }

    internal static ChainVersion DecodeGetVersion(TLReadBuffer buffer)
    {
        // mode:#
        buffer.ReadInt32();
        // version:int
        var version = buffer.ReadInt32();
        // capabilities:long
        var capabilities = buffer.ReadInt64();
        // now:int
        var time = buffer.ReadInt32();

        return new ChainVersion(version, capabilities, time);
    }

    internal static byte[] DecodeGetBlock(TLReadBuffer buffer)
    {
        // id:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        var data = buffer.ReadBuffer();
        return data;
    }

    internal static BlockHeader DecodeBlockHeader(TLReadBuffer buffer)
    {
        var workchain = buffer.ReadInt32();
        var shard = buffer.ReadInt64();
        var seqno = buffer.ReadInt32();
        var rootHash = buffer.ReadBytes(32);
        var fileHash = buffer.ReadBytes(32);

        var blockIdExtended = new BlockIdExtended(workchain, rootHash, fileHash, shard, seqno);
        // mode:#
        buffer.ReadUInt32();

        // header_proof:bytes
        var headerProof = buffer.ReadBuffer();
        return new BlockHeader
        {
            BlockId = blockIdExtended,
            HeaderProof = headerProof
        };
    }

    internal static int DecodeSendMessage(TLReadBuffer buffer)
    {
        return buffer.ReadInt32();
    }

    internal static AccountStateResult DecodeGetAccountState(TLReadBuffer buffer)
    {
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

        var result = new AccountStateResult
        {
            ShardProof = buffer.ReadBuffer(),
            Proof = buffer.ReadBuffer(),
            State = buffer.ReadBuffer()
        };
        return result;
    }

    internal static RunSmcMethodResult DecodeRunSmcMethod(TLReadBuffer buffer)
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

        var shardProof = (mode & (1 << 0)) != 0 ? buffer.ReadBuffer() : null;
        var proof = (mode & (1 << 0)) != 0 ? buffer.ReadBuffer() : null;
        var stateProof = (mode & (1 << 1)) != 0 ? buffer.ReadBuffer() : null;
        var initC7 = (mode & (1 << 3)) != 0 ? buffer.ReadBuffer() : null;
        var libExtras = (mode & (1 << 4)) != 0 ? buffer.ReadBuffer() : null;
        var exitCode = buffer.ReadInt32();
        var result = (mode & (1 << 2)) != 0 ? buffer.ReadBuffer() : null;

        return new RunSmcMethodResult(shardProof, proof, stateProof, initC7, libExtras, exitCode, result);
    }

    internal static ShardInfo DecodeGetShardInfo(TLReadBuffer buffer)
    {
        // id:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        // shardblk:tonNode.blockIdExt
        var workchain = buffer.ReadInt32();
        var shard = buffer.ReadInt64();
        var seqno = buffer.ReadInt32();
        var rootHash = buffer.ReadInt256();
        var fileHash = buffer.ReadInt256();
        var shardBlock = new BlockIdExtended(workchain, rootHash, fileHash, shard, seqno);

        var shardProof = buffer.ReadBuffer();
        var shardDescr = buffer.ReadBuffer();

        return new ShardInfo(shardProof, shardDescr, shardBlock);
    }

    internal static byte[] DecodeGetAllShardsInfo(TLReadBuffer buffer)
    {
        // id:tonNode.blockIdExt
        var workchain = buffer.ReadInt32();
        var shard = buffer.ReadInt64();
        var seqno = buffer.ReadInt32();
        var rootHash = buffer.ReadInt256();
        var fileHash = buffer.ReadInt256();
        var block = new BlockIdExtended(workchain, rootHash, fileHash, shard, seqno);

        buffer.ReadBuffer();
        var data = buffer.ReadBuffer();
        return data;
    }

    internal static byte[] DecodeGetTransactions(TLReadBuffer buffer)
    {
        var count = buffer.ReadUInt32();
        for (var i = 0; i < count; i++)
        {
            // id:tonNode.blockIdExt
            buffer.ReadInt32();
            buffer.ReadInt64();
            buffer.ReadInt32();
            buffer.ReadInt256();
            buffer.ReadInt256();
        }

        return buffer.ReadBuffer();
    }

    internal static ListBlockTransactionsExtendedResult DecodeListBlockTransactionsExtended(TLReadBuffer buffer)
    {
        // id:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        buffer.ReadUInt32();
        var inComplete = buffer.ReadBool();
        var transactions = buffer.ReadBuffer();
        var proof = buffer.ReadBuffer();

        return new ListBlockTransactionsExtendedResult(inComplete, transactions, proof);
    }

    internal static ListBlockTransactionsResult DecodeListBlockTransactions(TLReadBuffer buffer)
    {
        // id:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        buffer.ReadUInt32();
        var inComplete = buffer.ReadBool();

        var count = buffer.ReadUInt32();

        var ids = new List<TransactionId>();
        for (var i = 0; i < count; i++)
        {
            var id = new TransactionId();
            buffer.ReadUInt32();
            id.Account = buffer.ReadInt256();
            id.Lt = buffer.ReadInt64();
            id.Hash = buffer.ReadInt256();
            ids.Add(id);
        }

        var proof = buffer.ReadBuffer();

        return new ListBlockTransactionsResult(inComplete, ids.ToArray(), proof);
    }

    internal static ConfigInfo DecodeGetConfigAll(TLReadBuffer buffer)
    {
        // mode:#
        buffer.ReadUInt32();

        // id:tonNode.blockIdExt
        buffer.ReadInt32();
        buffer.ReadInt64();
        buffer.ReadInt32();
        buffer.ReadInt256();
        buffer.ReadInt256();

        var stateProof = buffer.ReadBuffer();
        var configProof = buffer.ReadBuffer();

        return new ConfigInfo
        {
            StateProof = stateProof,
            ConfigProof = configProof
        };
    }

    internal static LibraryEntry[] DecodeGetLibraries(TLReadBuffer buffer)
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

    internal static ShardBlockProof DecodeGetShardBlockProof(TLReadBuffer buffer)
    {
        // masterchain_id:tonNode.blockIdExt
        var w = buffer.ReadInt32();
        var shard = buffer.ReadInt64();
        var seqno = buffer.ReadInt32();
        var rootHash = buffer.ReadBytes(32);
        var fileHash = buffer.ReadBytes(32);
        var masterChainId = new BlockIdExtended(w, rootHash, fileHash, shard, seqno);

        var count = buffer.ReadUInt32();

        var links = new List<ShardBlockLink>();
        for (var i = 0; i < count; i++)
        {
            // from:tonNode.blockIdExt
            var linkFromW = buffer.ReadInt32();
            var linkFromShard = buffer.ReadInt64();
            var linkFromSeqno = buffer.ReadInt32();
            var linkFromRootHash = buffer.ReadBytes(32);
            var linkFromFileHash = buffer.ReadBytes(32);
            var linkFrom = new BlockIdExtended(linkFromW, linkFromRootHash, linkFromFileHash, linkFromShard,
                linkFromSeqno);

            var proof = buffer.ReadBuffer();
            links.Add(new ShardBlockLink
            {
                BlockIdExtended = linkFrom,
                Proof = proof
            });
        }

        return new ShardBlockProof
        {
            MasterChainId = masterChainId,
            Links = links.ToArray()
        };
    }

    internal static PartialBlockProof DecodeGetBlockProof(TLReadBuffer buffer)
    {
        var complete = buffer.ReadBool();

        // from:tonNode.blockIdExt
        var fromW = buffer.ReadInt32();
        var fromShard = buffer.ReadInt64();
        var fromSeqno = buffer.ReadInt32();
        var fromRootHash = buffer.ReadBytes(32);
        var fromFileHash = buffer.ReadBytes(32);
        var from = new BlockIdExtended(fromW, fromRootHash, fromFileHash, fromShard, fromSeqno);

        // to:tonNode.blockIdExt
        var toW = buffer.ReadInt32();
        var toShard = buffer.ReadInt64();
        var toSeqno = buffer.ReadInt32();
        var toRootHash = buffer.ReadBytes(32);
        var toFileHash = buffer.ReadBytes(32);
        var to = new BlockIdExtended(toW, toRootHash, toFileHash, toShard, toSeqno);

        var blockLinks = new List<IBlockLink>();
        var count = buffer.ReadUInt32();
        for (var i = 0; i < count; i++)
        {
            var kind = buffer.ReadInt32();
            if (kind == -276947985)
            {
                // liteServer_blockLinkBack
                var toKeyBlock = buffer.ReadBool();

                // from:tonNode.blockIdExt
                var linkFromW = buffer.ReadInt32();
                var linkFromShard = buffer.ReadInt64();
                var linkFromSeqno = buffer.ReadInt32();
                var linkFromRootHash = buffer.ReadBytes(32);
                var linkFromFileHash = buffer.ReadBytes(32);
                var linkFrom = new BlockIdExtended(linkFromW, linkFromRootHash, linkFromFileHash, linkFromShard,
                    linkFromSeqno);

                // to:tonNode.blockIdExt
                var linkToW = buffer.ReadInt32();
                var linkToShard = buffer.ReadInt64();
                var linkToSeqno = buffer.ReadInt32();
                var linkToRootHash = buffer.ReadBytes(32);
                var linkToFileHash = buffer.ReadBytes(32);
                var linkTo = new BlockIdExtended(linkToW, linkToRootHash, linkToFileHash, linkToShard, linkToSeqno);

                var destProof = buffer.ReadBuffer();
                var proof = buffer.ReadBuffer();
                var stateProof = buffer.ReadBuffer();

                blockLinks.Add(new BlockLinkBack
                {
                    ToKeyBlock = toKeyBlock,
                    DestProof = destProof,
                    StateProof = stateProof,
                    Proof = proof,
                    From = linkFrom,
                    To = linkTo
                });
            }

            if (kind == 1376767516)
            {
                // liteServer_blockLinkForward
                var toKeyBlock = buffer.ReadBool();

                // from:tonNode.blockIdExt
                var linkFromW = buffer.ReadInt32();
                var linkFromShard = buffer.ReadInt64();
                var linkFromSeqno = buffer.ReadInt32();
                var linkFromRootHash = buffer.ReadBytes(32);
                var linkFromFileHash = buffer.ReadBytes(32);
                var linkFrom = new BlockIdExtended(linkFromW, linkFromRootHash, linkFromFileHash, linkFromShard,
                    linkFromSeqno);

                // to:tonNode.blockIdExt
                var linkToW = buffer.ReadInt32();
                var linkToShard = buffer.ReadInt64();
                var linkToSeqno = buffer.ReadInt32();
                var linkToRootHash = buffer.ReadBytes(32);
                var linkToFileHash = buffer.ReadBytes(32);
                var linkTo = new BlockIdExtended(linkToW, linkToRootHash, linkToFileHash, linkToShard, linkToSeqno);

                var destProof = buffer.ReadBuffer();
                var configProof = buffer.ReadBuffer();

                var validatorSetHash = buffer.ReadInt32();
                var catchainSeqno = buffer.ReadInt32();

                var signatures = new List<Signature>();
                var c = buffer.ReadUInt32();
                for (var j = 0; j < c; j++)
                {
                    var nodeIdShort = new BigInteger(buffer.ReadInt256());
                    var signature = buffer.ReadBuffer();
                    signatures.Add(new Signature
                    {
                        NodeIdShort = nodeIdShort,
                        SignatureBytes = signature
                    });
                }

                blockLinks.Add(new BlockLinkForward
                {
                    ToKeyBlock = toKeyBlock,
                    CatchainSeqno = catchainSeqno,
                    ConfigProof = configProof,
                    DestProof = destProof,
                    From = linkFrom,
                    To = linkTo,
                    Signatures = signatures.ToArray(),
                    ValidatorSetHash = validatorSetHash
                });
            }
        }

        return new PartialBlockProof
        {
            Complete = complete,
            From = from,
            To = to,
            BlockLinks = blockLinks.ToArray()
        };
    }
}