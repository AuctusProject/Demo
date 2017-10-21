using Auctus.DomainObjects.Accounts;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class PensionFundTransaction
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 Id { get; set; }
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String TransactionHash { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime CreationDate { get; set; }
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String PensionFundContractHash { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 ContractFunctionId { get; set; }
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String WalletAddress { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public int? GasUsed { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public int? BlockNumber { get; set; }
        [DapperType(System.Data.DbType.Byte)]
        public TransactionStatus TransactionStatus { get; set; }

        public FunctionType FunctionType { get { return FunctionType.Get(ContractFunctionId); } }
    }
}
