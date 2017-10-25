using Auctus.DomainObjects.Accounts;
using Auctus.DomainObjects.Contracts;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Unprocessed
{
    public class UVestingRule
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 Id { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double Percentage { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public Int32 Period { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 UCompanyId { get; set; }
    }
}
