using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class SmartContract
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 Id { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public String Name { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 GasLimit { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public String ABI { get; set; }

        public SmartContractType SmartContractType { get { return SmartContractType.Get(Id); } }
    }
}
