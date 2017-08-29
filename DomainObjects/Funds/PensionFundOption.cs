using Auctus.DomainObjects.Accounts;
using Auctus.DomainObjects.Security;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Funds
{
    public class PensionFundOption
    {
        public String Address { get; set; }
        public Decimal Fee { get; set; }
        public Decimal LatePenalty { get; set; }
        public Int32 PensionFundId { get; set; }

        public Wallet Wallet { get; set; }
    }
}
