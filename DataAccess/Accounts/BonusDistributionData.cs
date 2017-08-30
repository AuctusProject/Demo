using Auctus.DomainObjects.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Accounts
{
    public class BonusDistributionData : BaseData<BonusDistribution>
    {
        public override string TableName => "BonusDistribution";
    }
}
