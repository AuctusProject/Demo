using Auctus.DomainObjects.Accounts;
using Auctus.DomainObjects.Contracts;
using Auctus.Util.DapperAttributes;
using Auctus.DomainObjects.Unprocessed;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Unprocessed
{
    public class UVestingRuleData : BaseData<UVestingRule>
    {
        public override string TableName => "UVestingRule";
    }
}
