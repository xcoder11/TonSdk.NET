﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TonSdk.Adnl.LiteClient;
using TonSdk.Adnl.LiteClient.Models;
using TonSdk.Client.Stack;
using TonSdk.Core;
using TonSdk.Core.Block;
using TonSdk.Core.Boc;
using TonSdk.Core.Crypto;

namespace TonSdk.Client
{
    public class LiteClientApi(string host, int port, string pubKey) : IDisposable
    {
        private readonly LiteClient _liteClient = new(host, port, pubKey);

        private async Task Init(CancellationToken ct = default)
        {
            await _liteClient.Connect(ct);
        }

        public async Task<WalletInformationResult> GetWalletInformation(Address address, BlockIdExtended? block = null, CancellationToken ct = default)
        {
            await Init(ct);
            
            var result = new WalletInformationResult
            {
                IsWallet = false,
                Balance = new Coins(0)
            };

            var addressInformation = await GetAddressInformation(address, block, ct);
            result.State = addressInformation.State;
            
            if (addressInformation.State == AccountState.Uninit || addressInformation.State == AccountState.NonExist) 
                return result;

            result.IsWallet = true;
            result.Balance = addressInformation.Balance;
            result.LastTransactionId = addressInformation.LastTransactionId;

            string resultHash = addressInformation.Code.Hash.ToString("base64");
            foreach (var t in WalletUtils.KnownWallets.Where(t => string.Equals(t.CodeHash, resultHash)))
            {
                result.WalletType = t.Type;
                t.DataExtractor(ref result, addressInformation.Data);
                break;
            }
            return result;
        }
        public async Task<AddressInformationResult> GetAddressInformation(Address address, BlockIdExtended? block = null, CancellationToken ct = default)
        {
            var result = new AddressInformationResult();
            await Init(ct);
            var res = await _liteClient.GetAccountState(address, block?.ConvertBlockIdToAdnlBase(), ct);
            
            
            var cells = BagOfCells.DeserializeBoc(new Bits(res.Proof));

            var cs = cells[1].Parse().LoadRef().Parse();
            
            if (cs.LoadUInt(32) != 0x9023afe2) {
                throw new Exception("Invalid data");
            }

            int globalId = (int)cs.LoadInt(32);
            if ((uint)cs.LoadUInt(2) != 0) {
                throw new Exception("Invalid data");
            }
            //
            // var shardPrefixBits = cs.LoadUInt(6);
            // var workchainId = cs.LoadInt(32);
            // var shardPrefix = cs.LoadUInt(64);
            //
            // var seqno = cs.LoadUInt(32);
            // var vertSeno = cs.LoadUInt(32);
            // var genUTime = cs.LoadUInt(32);
            //
            // var genLt = cs.LoadUInt(64);
            // var minRefMcSeqno = cs.LoadUInt(32);
            //
            // cs.LoadRef();
            // Console.WriteLine(cs.RestoreRemainder().ToString("hex"));
            // bool beforeSplit = cs.LoadBit();
            //
            // var shardAccountsRef = cs.LoadRef();
            // // ShardAccount a;
            //
            // if (!shardAccountsRef.IsExotic) 
            // {
            //     var saOptions = new HashmapOptions<BigInteger, CellSlice>()
            //     {
            //         KeySize = 256,
            //         Serializers = new HashmapSerializers<BigInteger, CellSlice>
            //         {
            //             Key = k => new BitsBuilder(256).StoreUInt(k, 256).Build(),
            //             Value = v => new CellBuilder().Build()
            //         },
            //         Deserializers = new HashmapDeserializers<BigInteger, CellSlice>
            //         {
            //             Key = k => k.Parse().LoadUInt(256),
            //             Value = v => v.Parse()
            //         }
            //     };
            //
            //
            //     
            //     //var ss = shardAccounts.LoadRef().Parse();
            //     //ss.LoadBit();
            //     
            //     Console.WriteLine(Utils.BytesToHex(shardAccountsRef.Parse().Refs[0].Bits.ToBytes()));
            //     Console.WriteLine(Utils.BytesToHex(shardAccountsRef.Parse().Refs[1].Bits.ToBytes()));
            //
            //     //Console.WriteLine(ss.LoadUInt(64));
            //     //Console.WriteLine(ss.RemainderBits);
            //     
            // }
            //
            // // return default;
            // var shardState = cells[0].Parse().Refs[0].Parse().LoadRef().Parse().LoadRef().Parse();
            //
            //
            
            byte[] accountStateBytes = res.State;
            if (accountStateBytes.Length == 0)
            {
                result.State = AccountState.Uninit;
                return result;
            }
            
            CellSlice slice = Cell.From(new Bits(accountStateBytes)).Parse();
            
            slice.LoadBit(); // tag
            slice.LoadAddress(); // skip address (not needed)

            slice.LoadVarUInt(7);
            slice.LoadVarUInt(7);
            slice.LoadVarUInt(7);

            slice.LoadUInt32LE();
            
            if (slice.LoadBit())
                slice.LoadCoins();

            result.LastTransactionId = new TransactionId()
            {
                Lt = (ulong)slice.LoadUInt(64),
            };
            result.Balance = slice.LoadCoins();
            
            var hmOptions = new HashmapOptions<int, int>()
            {
                KeySize = 32,
                Serializers = null,
                Deserializers = null
            };

            slice.LoadDict(hmOptions);

            if (slice.LoadBit()) // active
            {
                result.State = AccountState.Active;
                if(slice.LoadBit())
                    slice.LoadUInt(5);
                
                if (slice.LoadBit())
                {
                    slice.LoadBit();
                    slice.LoadBit();
                }

                if (slice.LoadBit())
                    result.Code = slice.LoadRef();
                
                if (slice.LoadBit())
                    result.Data = slice.LoadRef();
                
                if (slice.LoadBit())
                    slice.LoadRef();
            }
            else if (slice.LoadBit()) // frozen
            {
                result.State = AccountState.Frozen;
                result.FrozenHash = slice.LoadBits(256).ToString("base64");
            }
            else result.State = AccountState.Uninit;
            
            return result;
        }
        public async Task<MasterchainInformationResult> GetMasterchainInfo(CancellationToken ct = default)
        {
            await Init(ct);
            return new MasterchainInformationResult(await _liteClient.GetMasterChainInfo(ct));
        }

        public async Task<SendBocResult> SendBoc(Cell boc, CancellationToken ct = default)
        {
            await Init(ct);
            var data = await _liteClient.SendMessage(BagOfCells.SerializeBoc(boc).ToBytes(), ct);
            return new SendBocResult() { Type = data.ToString(), Hash = boc.Hash.ToString()};
        }

        public async Task<BlockIdExtended> LookUpBlock(int workchain, long shard, long? seqno = null, ulong? lt = null, ulong? unixTime = null, CancellationToken ct = default)
        {
            await Init(ct);
            var blockHeader = await _liteClient.LookUpBlock(workchain, shard, seqno, lt, unixTime, ct);
            return new BlockIdExtended(blockHeader.BlockId);
        }
        
        public async Task<BlockDataResult> GetBlockData(int workchain, long shard, long seqno, CancellationToken ct = default)
        {
            var result = new BlockDataResult();
            await Init(ct);
            var blockHeader = await _liteClient.LookUpBlock(workchain, shard, seqno, ct: ct);
            result.BlockIdExtended = new BlockIdExtended(blockHeader.BlockId);
            
            byte[] blockDataBytes = await _liteClient.GetBlock(blockHeader.BlockId, ct);
            if (blockDataBytes.Length == 0) 
                return result;
            
            CellSlice slice = Cell.From(new Bits(blockDataBytes)).Parse();
            result.Version = (uint)slice.LoadUInt(32);
            return result;
        }

        public async Task<BlockTransactionsResult> GetBlockTransactions(
            int workchain,
            long shard,
            long seqno,
            string? rootHash = null,
            string? fileHash = null,
            ulong? afterLt = null,
            string? afterHash = null,
            uint count = 10, CancellationToken ct = default)
        {
            var result = new BlockTransactionsResult();
            await Init(ct);

            Adnl.LiteClient.Models.BlockIdExtended blockId;
            if (rootHash == null || fileHash == null)
                blockId = (await _liteClient.LookUpBlock(workchain, shard, seqno, ct: ct)).BlockId;
            else
                blockId = new Adnl.LiteClient.Models.BlockIdExtended(workchain, Convert.FromBase64String(rootHash),
                    Convert.FromBase64String(fileHash), shard, (int)seqno);

            var transactionId = new Adnl.LiteClient.Models.TransactionId();
            if (afterLt != null && afterHash != null)
            {
                transactionId.Hash = Convert.FromBase64String(afterHash);
                transactionId.Lt = (long)afterLt;
            }
            else transactionId = null;
            
            var blockTransactions = await _liteClient.ListBlockTransactions(blockId, count, transactionId);

            result.Transactions = blockTransactions.TransactionIds.Select(tx => 
                new ShortTransactionsResult()
                {
                    Account = new Address(workchain, tx.Account).ToString(),
                    Hash = Convert.ToBase64String(tx.Hash),
                    Lt = (ulong)tx.Lt
                }).ToArray();
            result.Incomplete = blockTransactions.InComplete;
            result.ReqCount = (int)count;
            result.Id = new BlockIdExtended(blockId);
            return result;
        }
        
        public async Task<BlockTransactionsResultExtended> GetBlockTransactionsExtended(
            int workchain,
            long shard,
            long seqno,
            string? rootHash = null,
            string? fileHash = null,
            ulong? afterLt = null,
            string? afterHash = null,
            uint count = 10, CancellationToken ct = default)
        {
            try
            {
                var result = new BlockTransactionsResultExtended();
                await Init(ct);

                Adnl.LiteClient.Models.BlockIdExtended blockId;
                if (rootHash == null || fileHash == null)
                    blockId = (await _liteClient.LookUpBlock(workchain, shard, seqno, ct: ct)).BlockId;
                else
                    blockId = new Adnl.LiteClient.Models.BlockIdExtended(workchain, Convert.FromBase64String(rootHash),
                        Convert.FromBase64String(fileHash), shard, (int)seqno);

                var transactionId = new Adnl.LiteClient.Models.TransactionId();
                if (afterLt != null && afterHash != null)
                {
                    transactionId.Hash = Convert.FromBase64String(afterHash);
                    transactionId.Lt = (long)afterLt;
                }
                else transactionId = null;
            
                var blockTransactions = await _liteClient.ListBlockTransactionsExtended(blockId, count, transactionId, ct: ct);
                
                result.Incomplete = blockTransactions.InComplete;
                result.ReqCount = (int)count;
                result.Id = new BlockIdExtended(blockId);
                if (blockTransactions.Transactions.Length == 0)
                {
                    result.Transactions = [];
                    return result;
                }

                var cells = BagOfCells.DeserializeBoc(new Bits(blockTransactions.Transactions));
                result.Transactions = cells.Select(cell => Parsers.Utils.ParseTransactionCell(cell)).ToArray();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<TransactionsInformationResult[]> GetTransactions(Address address, uint limit, long lt, string hash, CancellationToken ct = default)
        {
            try
            {
                var result = new List<TransactionsInformationResult>();
                await Init(ct);

                byte[] transactions = await _liteClient.GetTransactions(limit, address, lt, hash, ct);

                if (transactions.Length == 0)
                    return result.ToArray();
                
                var cells = BagOfCells.DeserializeBoc(new Bits(transactions));
                
                result.AddRange(cells.Select(cell => Parsers.Utils.ParseTransactionCell(cell)));

                return result.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<RunGetMethodResult?> RunGetMethod(Address address, string method, IStackItem[] stackItems, BlockIdExtended? blockId = null, CancellationToken ct = default)
        {
            await Init(ct);
            
            var result = new RunGetMethodResult();
            byte[] stackBytes = BagOfCells.SerializeBoc(StackUtils.SerializeStack(stackItems)).ToBytes();
            var smcResult = await _liteClient.RunSmcMethod(address, method, stackBytes, new RunSmcOptions() { Result = true },
                blockId?.ConvertBlockIdToAdnlBase(), ct);
            try
            {
                var resultStack = StackUtils.DeserializeStack(Convert.ToBase64String(smcResult.Result));
                result.StackItems = resultStack;
                result.ExitCode = smcResult.ExitCode;
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ConfigParamResult> GetConfigParam(int configId, CancellationToken ct = default)
        {
            await Init(ct);
            
            var result = new ConfigParamResult();
            byte[] configBytes = (await _liteClient.GetConfigParams([configId], ct: ct)).ConfigProof;
            result.Bytes = Cell.From(new Bits(configBytes));
            return result;
        }

        public async Task<EstimateFeeResult> EstimateFee(MessageX messageX, bool ignore = true, CancellationToken ct = default)
        {
            await Init(ct);
            var result = new EstimateFeeResult();
            return result;
        }

        public async Task<ShardsInformationResult> GetShards(long seqno, CancellationToken ct = default)
        {
            try
            {
                await Init(ct);
                var result = new ShardsInformationResult();
                var block = (await _liteClient.LookUpBlock(-1, -9223372036854775808, seqno, ct: ct)).BlockId;
                var data = (await _liteClient.GetAllShardsInfo(block, ct));
                var cells = BagOfCells.DeserializeBoc(new Bits(data));

                foreach (var cell in cells)
                {
                    var hmOptions = new HashmapOptions<uint, CellSlice>()
                    {
                        KeySize = 32,
                        Serializers = new HashmapSerializers<uint, CellSlice>
                        {
                            Key = k => new BitsBuilder(32).StoreUInt(k, 32).Build(),
                            Value = v => new CellBuilder().Build()
                        },
                        Deserializers = new HashmapDeserializers<uint, CellSlice>
                        {
                            Key = k => (uint)k.Parse().LoadUInt(32),
                            Value = v => v.Parse()
                        }
                    };
                
                    var hashes = cell.Parse().LoadDict(hmOptions);
                    var binTree = hashes.Get(0).LoadRef().Parse();
                    var shards = new List<BlockIdExtended>();
                    LoadBinTreeR(binTree, ref shards);

                    result.Shards = shards.ToArray();
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
        
        private BlockIdExtended LoadShardDescription(CellSlice slice)
        {
            uint type = (uint)slice.LoadUInt(4);
            
            if (type != 0xa && type != 0xb)
                throw new Exception("not a ShardDescr");
            
            int seqno = (int)slice.LoadUInt(32);
            slice.LoadUInt(32);
            slice.LoadUInt(64);
            slice.LoadUInt(64);
            byte[] rootHash = slice.LoadBits(256).ToBytes(); // root_hash
            byte[] fileHash = slice.LoadBits(256).ToBytes(); // file_hash
            slice.LoadBit();
            slice.LoadBit();
            slice.LoadBit();
            slice.LoadBit();
            slice.LoadBit();
            slice.LoadUInt(3);
            slice.LoadUInt(32);
            long shard = (long)slice.LoadInt(64);

            return new BlockIdExtended(new Adnl.LiteClient.Models.BlockIdExtended(0, rootHash, fileHash, shard, seqno));
        }
        
        private void LoadBinTreeR(CellSlice slice, ref List<BlockIdExtended> shards)
        {
            if (!slice.LoadBit())
            {
                shards.Add(LoadShardDescription(slice));
            }
            else
            {
                LoadBinTreeR(slice.LoadRef().Parse(), ref shards);
                LoadBinTreeR(slice.LoadRef().Parse(), ref shards);
            }
        }

        public void Dispose()
        {
            _liteClient.Disconnect();
        }
    }
}