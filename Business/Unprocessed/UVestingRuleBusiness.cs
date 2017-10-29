using Auctus.DomainObjects.Accounts;
using Auctus.DomainObjects.Contracts;
using Auctus.Util.DapperAttributes;
using Auctus.DomainObjects.Unprocessed;
using System;
using System.Collections.Generic;
using System.Text;
using Auctus.DataAccess.Unprocessed;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using Auctus.Model;
using Dapper;
using System.Linq;

namespace Auctus.Business.Unprocessed
{
    public class UVestingRuleBusiness : BaseBusiness<UVestingRule, UVestingRuleData>
    {
        public UVestingRuleBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache)
        {
        }

        internal UVestingRule Create(VestingRules vestingRule, int uCompanyId)
        {
            var uVestingRule = new UVestingRule();
            uVestingRule.Percentage = vestingRule.Percentage;
            uVestingRule.Period = vestingRule.Period;
            uVestingRule.UCompanyId = uCompanyId;
            Insert(uVestingRule);
            return uVestingRule;
        }

        internal List<UVestingRule> ListByCompany(int uCompanyId)
        {
            return Data.ListByCompany(uCompanyId);
        }
    }
}
