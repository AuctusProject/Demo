using Auctus.DomainObjects.Accounts;
using Auctus.DomainObjects.Contracts;
using Auctus.Util.DapperAttributes;
using Auctus.DomainObjects.Unprocessed;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Linq;

namespace Auctus.DataAccess.Unprocessed
{
    public class UVestingRuleData : BaseData<UVestingRule>
    {
        public override string TableName => "UVestingRule";
        
        public List<UVestingRule> ListByCompany(int uCompanyId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("UCompanyId", uCompanyId, System.Data.DbType.Int32);
            return SelectByParameters<UVestingRule>(param).ToList();
        }
    }
}
