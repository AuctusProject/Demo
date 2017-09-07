using Auctus.DomainObjects.Accounts;
using Auctus.DomainObjects.Contracts;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Funds
{
    public class PensionFundOption
    {
        [DapperKey]
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String Address { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double Fee { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double LatePenalty { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 PensionFundId { get; set; }
        
        public PensionFundContract PensionFundContract { get; set; }
        public Company Company { get; set; }
}
}
