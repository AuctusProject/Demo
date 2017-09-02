using Auctus.DomainObjects.Accounts;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class ReferenceValue
    {
        [DapperKey]
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String ReferenceContractAddress { get; set; }
        [DapperKey]
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 Period { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double Value { get; set; }
    }
}
