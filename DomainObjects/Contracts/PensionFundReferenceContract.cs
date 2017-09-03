using Auctus.DomainObjects.Accounts;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class PensionFundReferenceContract
    {
        [DapperKey]
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String PensionFundContractHash { get; set; }
        [DapperKey]
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String ReferenceContractAddress { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double Percentage { get; set; }

        public ReferenceType ReferenceType { get { return ReferenceType.Get(ReferenceContractAddress); } }
    }
}
