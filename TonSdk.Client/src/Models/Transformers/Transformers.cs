using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using TonSdk.Core;
using TonSdk.Core.Boc;

namespace TonSdk.Client
{
    internal static class Transformers
    {
        internal static string[] PackRequestStack(object element)
        {
            if (element is Cell)
            {
                return new string[] { "tvm.Cell", ((Cell)element).ToString() };
            }

            if (element is BigInteger || element is uint || element is int || element is long || element is ulong)
            {
                return new string[] { "num", element.ToString()! };
            }

            if (element is Coins)
            {
                return new string[] { "num", ((Coins)element).ToNano() };
            }

            if (element is CellSlice)
            {
                return new string[] { "tvm.Slice", ((CellSlice)element).RestoreRemainder().ToString()! };
            }

            if (element is Address)
            {
                return new string[] { "tvm.Slice", ((Address)element).ToBOC() };
            }

            // TODO: Message Layout
            throw new Exception($"Unknown type of element: {element}");
        }

        // in
        internal struct EmptyBody : IRequestBody
        {
        }

        internal struct InShardsBody : IRequestBody
        {
            public long seqno { get; set; }

            internal InShardsBody(long seqno) => this.seqno = seqno;
        }

        internal struct InBlockTransactions : IRequestBody
        {
            public int workchain { get; set; }
            public long shard { get; set; }
            public long seqno { get; set; }
            public string root_hash { get; set; }
            public string file_hash { get; set; }
            public ulong? after_lt { get; set; }
            public string after_hash { get; set; }
            public uint? count { get; set; }

            internal InBlockTransactions(
                int workchain,
                long shard,
                long seqno,
                string root_hash = null,
                string file_hash = null,
                ulong? after_lt = null,
                string after_hash = null,
                uint? count = null)
            {
                this.workchain = workchain;
                this.shard = shard;
                this.seqno = seqno;
                this.root_hash = root_hash;
                this.file_hash = file_hash;
                this.after_lt = after_lt;
                this.after_hash = after_hash;
                this.count = count;
            }
        }
        
        internal struct InLookUpBlock : IRequestBody
        {
            public int workchain { get; set; }
            public long shard { get; set; }
            public long? seqno { get; set; }
            public ulong? lt { get; set; }
            public ulong? unixTime { get; set; }

            public InLookUpBlock(
                int workchain,
                long shard,
                long? seqno = null,
                ulong? lt = null,
                ulong? unixTime = null)
            {
                this.workchain = workchain;
                this.shard = shard;
                this.seqno = seqno;
                this.lt = lt;
                this.unixTime = unixTime;
            }
        }

        internal struct InBlockHeader : IRequestBody
        {
            public int workchain { get; set; }
            public long shard { get; set; }
            public long seqno { get; set; }
            public string root_hash { get; set; }
            public string file_hash { get; set; }

            public InBlockHeader(
                int workchain,
                long shard,
                long seqno,
                string root_hash = null,
                string file_hash = null)
            {
                this.workchain = workchain;
                this.shard = shard;
                this.seqno = seqno;
                this.root_hash = root_hash;
                this.file_hash = file_hash;
            }
        }

        internal struct InAdressInformationBody : IRequestBody
        {
            public string address { get; set; }

            internal InAdressInformationBody(string address) => this.address = address;
        }

        internal struct InTransactionsBody : IRequestBody
        {
            public string address;

            public uint limit;

            public ulong lt;

            public string hash;

            public ulong to_lt;

            public bool archival;
        }

        internal struct InRunGetMethodBody : IRequestBody
        {
            public string address;
            public string method;
            public string[][] stack;

            internal InRunGetMethodBody(string address, string method, string[][] stack)
            {
                this.address = address;
                this.method = method;
                this.stack = stack;
            }
        }

        internal struct InSendBocBody : IRequestBody
        {
            public string boc;
        }

        internal struct InEstimateFeeBody : IRequestBody
        {
            public string address;
            public string body;
            public string init_code;
            public string init_data;
            public bool ignore_chksig;
        }

        internal struct InGetConfigParamBody : IRequestBody
        {
            public int config_id;
            public int seqno;
        }

        internal interface OutResult
        {
        }

        internal struct RootAddressInformation
        {
            [JsonProperty("ok")] public bool Ok { get; set; }
            [JsonProperty("result")] public OutAddressInformationResult Result { get; set; }
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
        }
        
        internal struct RootWalletInformation
        {
            [JsonProperty("ok")] public bool Ok { get; set; }

            [JsonProperty("result")]
            public OutWalletInformationResult Result { get; set; }
        }

        internal struct RootMasterchainInformation
        {
            [JsonProperty("ok")] public bool Ok { get; set; }
            [JsonProperty("result")] public OutMasterchanInformationResult Result { get; set; }
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
        }

        internal struct RootShardsInformation
        {
            [JsonProperty("ok")] public bool Ok { get; set; }
            [JsonProperty("result")] public OutShardsInformationResult Result { get; set; }
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
        }

        internal struct RootBlockTransactions
        {
            [JsonProperty("ok")] public bool Ok { get; set; }
            [JsonProperty("result")] public OutBlockTransactionsResult Result { get; set; }
            [JsonProperty("transactions")] public OutV3ShortTransactionsResult[] Transactions { get; set; }
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
        }

        internal struct RootBlockHeader
        {
            [JsonProperty("ok")] public bool Ok { get; set; }
            [JsonProperty("result")] public OutBlockHeaderResult Result { get; set; }
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
        }
        
        internal struct RootLookUpBlock
        {
            [JsonProperty("ok")] public bool Ok { get; set; }
            [JsonProperty("result")] public BlockIdExtended Result { get; set; }
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
        }
        
        internal struct RootV3LookUpBlock
        {
            [JsonProperty("blocks")] public BlockIdExtended[] Blocks { get; set; }
        }

        internal struct RootTransactions
        {
            [JsonProperty("ok")] public bool Ok { get; set; }
            [JsonProperty("result")] public OutTransactionsResult[] Result { get; set; }
            [JsonProperty("transactions")] public OutV3TransactionsResult[] Transactions { get; set; }
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
        }

        internal struct RootRunGetMethod
        {
            [JsonProperty("ok")] public bool Ok { get; set; }
            [JsonProperty("result")] public OutRunGetMethod Result { get; set; }
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
        }

        internal struct RootSendBoc
        {
            [JsonProperty("ok")] public bool Ok { get; set; }
            [JsonProperty("result")] public SendBocResult Result { get; set; }
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
            [JsonProperty("message_hash")] public string MessageHash { get; set; }
        }

        internal struct RootEstimateFee
        {
            [JsonProperty("ok")] public bool Ok { get; set; }
            [JsonProperty("result")] public EstimateFeeResult Result { get; set; }
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
        }

        internal struct RootGetConfigParam
        {
            [JsonProperty("ok")] public bool Ok { get; set; }
            [JsonProperty("result")] public OutGetConfigParamResult Result { get; set; }
            [JsonProperty("id")] public string Id { get; set; }
            [JsonProperty("jsonrpc")] public string JsonRPC { get; set; }
        }

        internal struct OutGetConfigParamResult
        {
            [JsonProperty("config")] public OutConfigParamResult Config;
        }

        internal struct OutConfigParamResult
        {
            [JsonProperty("bytes")] public string Bytes;
        }

        internal struct OutAddressInformationResult
        {
            [JsonProperty("state")] public string State;
            [JsonProperty("balance")] public string Balance;
            [JsonProperty("code")] public string Code;
            [JsonProperty("data")] public string Data;
            [JsonProperty("last_transaction_id")] public TransactionId LastTransactionId;
            [JsonProperty("block_id")] public BlockIdExtended BlockId;
            [JsonProperty("frozen_hash")] public string FrozenHash;
            [JsonProperty("sync_utime")] public long SyncUtime;
        }
        
        internal struct OutV3AddressInformationResult
        {
            [JsonProperty("status")] public string Status;
            [JsonProperty("balance")] public string Balance;
            [JsonProperty("code")] public string Code;
            [JsonProperty("data")] public string Data;
            [JsonProperty("last_transaction_lt")] public string LastTransactionLt;
            [JsonProperty("last_transaction_hash")] public string LastTransactionHash;
            [JsonProperty("frozen_hash")] public string FrozenHash;
        }

        internal struct OutWalletInformationResult
        {
            [JsonProperty("wallet")] public string IsWallet;
            [JsonProperty("balance")] public string Balance;
            [JsonProperty("account_state")] public string State;
            [JsonProperty("wallet_type")] public string WalletType;
            [JsonProperty("seqno")] public string Seqno;
            [JsonProperty("last_transaction_id")] public TransactionId LastTransactionId;
            [JsonProperty("wallet_id")] public string WalletId;
        }
        
        internal struct OutV3WalletInformationResult
        {
            [JsonProperty("balance")] public string Balance;
            [JsonProperty("status")] public string Status;
            [JsonProperty("wallet_type")] public string WalletType;
            [JsonProperty("seqno")] public string Seqno;
            [JsonProperty("last_transaction_lt")] public string LastTransactionLt;
            [JsonProperty("last_transaction_hash")] public string LastTransactionHash;
            [JsonProperty("wallet_id")] public string WalletId;
        }

        internal struct OutMasterchanInformationResult
        {
            [JsonProperty("last")] public BlockIdExtended LastBlock;
            [JsonProperty("init")] public BlockIdExtended InitBlock;
            [JsonProperty("state_root_hash")] public string StateRootHash;
        }
        
        internal struct OutV3MasterchainInformationResult
        {
            [JsonProperty("last")] public BlockIdExtended LastBlock;
            [JsonProperty("first")] public BlockIdExtended InitBlock;
        }

        internal struct OutShardsInformationResult
        {
            [JsonProperty("shards")] public BlockIdExtended[] Shards;
        }
        
        internal struct OutV3ShardsInformationResult
        {
            [JsonProperty("blocks")] public OutBlockIdExtended[] Blocks;
        }
        
        internal struct OutBlockIdExtended
        {
            [JsonProperty("workchain")] public int Workchain;
            [JsonProperty("shard")] public string Shard;
            [JsonProperty("seqno")] public string Seqno;
            [JsonProperty("hash")] public string Hash;
            [JsonProperty("root_hash")] public string RootHash;
            [JsonProperty("file_hash")] public string FileHash;
        }

        internal struct OutBlockTransactionsResult
        {
            [JsonProperty("id")] public BlockIdExtended Id;
            [JsonProperty("req_count")] public int ReqCount;
            [JsonProperty("incomplete")] public bool Incomplete;
            [JsonProperty("transactions")] public ShortTransactionsResult[] Transactions;
        }
        
        internal struct OutBlockHeaderResult
        {
            [JsonProperty("id")] public BlockIdExtended Id;
            [JsonProperty("global_id")] public long GlobalId;
            [JsonProperty("version")] public uint Version;
            [JsonProperty("flags")] public int Flags;
            [JsonProperty("after_merge")] public bool AfterMerge;
            [JsonProperty("after_split")] public bool AfterSplit;
            [JsonProperty("before_split")] public bool BeforeSplit;
            [JsonProperty("want_merge")] public bool WantMerge;
            [JsonProperty("want_split")] public bool WantSplit;
            [JsonProperty("validator_list_hash_short")] public long ValidatorListHashShort;
            [JsonProperty("catchain_seqno")] public long CatchainSeqno;
            [JsonProperty("min_ref_mc_seqno")] public long MinRefMcSeqno;
            [JsonProperty("is_key_block")] public bool IsKeyBlock;
            [JsonProperty("prev_key_block_seqno")] public long PrevKeyBlockSeqno;
            [JsonProperty("start_lt")] public ulong StartLt;
            [JsonProperty("end_lt")] public ulong EndLt;
            [JsonProperty("gen_utime")] public long RgenUtime;
            [JsonProperty("prev_blocks")] public BlockIdExtended[] PrevBlocks;
        }
        
        internal struct OutTransactionsResult
        {
            [JsonProperty("address")] public OutTxAddress Address;
            [JsonProperty("utime")] public long Utime;
            [JsonProperty("data")] public string Data;
            [JsonProperty("transaction_id")] public TransactionId TransactionId;
            [JsonProperty("fee")] public string Fee;
            [JsonProperty("storage_fee")] public string StorageFee;
            [JsonProperty("other_fee")] public string OtherFee;
            [JsonProperty("in_msg")] public OutRawMessage InMsg;
            [JsonProperty("out_msgs")] public OutRawMessage[] OutMsgs;
        }
        
        internal struct OutTxAddress
        {
            [JsonProperty("account_address")] public string AccountAddress;
        }
        
        internal struct OutV3TransactionsResult
        {
            [JsonProperty("account")] public string Account;
            [JsonProperty("now")] public long Now;
            [JsonProperty("lt")] public ulong Lt;
            [JsonProperty("hash")] public string Hash;
            [JsonProperty("total_fees")] public string Fee;
            [JsonProperty("prev_trans_hash")] public string PrevTransHash;
            [JsonProperty("prev_trans_lt")] public string PrevTransLt;
            [JsonProperty("in_msg")] public OutV3RawMessage InMsg;
            [JsonProperty("out_msgs")] public OutV3RawMessage[] OutMsgs;
        }

        internal struct OutRawMessage
        {
            [JsonProperty("source")] public string Source;
            [JsonProperty("destination")] public string Destination;
            [JsonProperty("value")] public string Value;
            [JsonProperty("fwd_fee")] public string FwdFee;
            [JsonProperty("ihr_fee")] public string IhrFee;
            [JsonProperty("created_lt")] public ulong CreaterLt;
            [JsonProperty("body_hash")] public string BodyHash;
            [JsonProperty("msg_data")] public OutRawMessageData MsgData;
            [JsonProperty("message")] public string Message;
        }
        
        internal struct OutV3RawMessage
        {
            [JsonProperty("hash")] public string Hash;
            [JsonProperty("source")] public string Source;
            [JsonProperty("destination")] public string Destination;
            [JsonProperty("value")] public string Value;
            [JsonProperty("opcode")] public string OpCode;
            [JsonProperty("fwd_fee")] public string FwdFee;
            [JsonProperty("ihr_fee")] public string IhrFee;
            [JsonProperty("created_lt")] public ulong CreatedLt;
            [JsonProperty("message_content")] public OutV3RawMessageData MsgData;
        }
        
        internal struct OutV3RawMessageData
        {
            [JsonProperty("hash")] public string BodyHash;
            [JsonProperty("body")] public string Body;
            [JsonProperty("decoded")] public OutV3MessageDataDecoded? Decoded;
        }
        
        internal struct OutV3MessageDataDecoded
        {
            [JsonProperty("comment")] public string Comment;
        }

        internal struct OutRawMessageData
        {
            [JsonProperty("text")] public string Text;
            [JsonProperty("body")] public string Body;
            [JsonProperty("init_state")] public string InitState;
        }
        
        public struct OutV3ShortTransactionsResult
        {
            [JsonProperty("description")] public OutV3ShortTransactionsDescription Description;
            [JsonProperty("account")] public string Account;
            [JsonProperty("lt")] public ulong Lt;
            [JsonProperty("hash")] public string Hash;
        }

        public struct OutV3ShortTransactionsDescription
        {
            [JsonProperty("compute_ph")] public OutV3ShortTransactionsDescriptionComputePh ComputePh;
        }
    
        public struct OutV3ShortTransactionsDescriptionComputePh
        {
            [JsonProperty("mode")] public int Mode;
        }
    }
}
