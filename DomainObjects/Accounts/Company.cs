using Auctus.DomainObjects.Accounts;
using Auctus.DomainObjects.Funds;
using Auctus.DomainObjects.Security;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Accounts
{
    public class Company
    {
        [DapperKey]
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String Address { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public String Name { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double BonusRate { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double MaxSalaryBonusRate { get; set; }
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String PensionFundOptionAddress { get; set; }

        public Employee Employee { get; set; }
        public IEnumerable<BonusDistribution> BonusDistribution { get; set; }
    }
}
