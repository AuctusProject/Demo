using Auctus.DomainObjects.Accounts;
using Auctus.DomainObjects.Funds;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Unprocessed
{
    public class UCompany
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 Id { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public String Name { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double BonusFee { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double MaxBonusFee { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 UPensionFundId { get; set; }

        public UEmployee Employee { get; set; }
        public List<UVestingRule> VestingRules { get; set; }
    }
}
