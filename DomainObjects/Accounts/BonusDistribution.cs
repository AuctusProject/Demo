using Auctus.DomainObjects.Funds;
using Auctus.DomainObjects.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Accounts
{
    public class BonusDistribution
    {
        public String CompanyAddress { get; set; }
        public Int32 Period { get; set; }
        public Double ReleasedBonus { get; set; }

        public Company Company { get; set; }
    }
}
