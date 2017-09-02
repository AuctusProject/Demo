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

        private const string SQL_BONUS_DISTRIBUTION = @"select * from BonusDistribution where CompanyAddress = @address";

        public List<BonusDistribution> List(string companyAddress)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("address", companyAddress, System.Data.DbType.AnsiStringFixedLength);
            return Query<BonusDistribution>(SQL_BONUS_DISTRIBUTION, param).ToList();
        }
    }
}
