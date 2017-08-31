using Auctus.DomainObjects.Accounts;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class ReferenceContract
    {
        [DapperKey]
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String Address { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public String Description { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 SmartContractId { get; set; }

        public SmartContract SmartContract { get; set; }
        public ReferenceType ReferenceType { get { return ReferenceType.Get(Address); } }
    }
}
