using Auctus.DomainObjects.Accounts;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class PensionFundReferenceContract
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 PensionFundContractId { get; set; }
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String ReferenceContractAddress { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double Percentage { get; set; }

        public SmartContract SmartContract { get; set; }
    }
}
