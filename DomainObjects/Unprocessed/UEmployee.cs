using Auctus.DomainObjects.Contracts;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Unprocessed
{
    public class UEmployee
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 Id { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public String Name { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double Salary { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double Contribution { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 UCompanyId { get; set; }
    }
}
