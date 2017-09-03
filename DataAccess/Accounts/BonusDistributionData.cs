using Auctus.DomainObjects.Accounts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Auctus.DataAccess.Accounts
{
    public class BonusDistributionData : BaseData<BonusDistribution>
    {
        public override string TableName => "BonusDistribution";
        
        public List<BonusDistribution> List(string companyAddress)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("CompanyAddress", companyAddress, System.Data.DbType.AnsiStringFixedLength);
            return SelectByParameters<BonusDistribution>(param).ToList();
        }
    }
}
