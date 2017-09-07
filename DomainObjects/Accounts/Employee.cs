using Auctus.DomainObjects.Contracts;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Accounts
{
    public class Employee
    {
        [DapperKey]
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String Address { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public String Name { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double Salary { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double Contribution { get; set; }
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String CompanyAddress { get; set; }
    }
}
