using Auctus.DomainObjects.Funds;
using Auctus.DomainObjects.Security;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Accounts
{
    public class BonusDistribution
    {
        [DapperKey]
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String CompanyAddress { get; set; }
        [DapperKey]
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 Period { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double ReleasedBonus { get; set; }

        public Company Company { get; set; }
    }
}
