using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class ContractFunction
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 Id { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public String Name { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 GasLimit { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 SmartContractId { get; set; }

        public SmartContract SmartContract { get; set; }
        public FunctionType FunctionType { get { return FunctionType.Get(Id); } }
    }
}
