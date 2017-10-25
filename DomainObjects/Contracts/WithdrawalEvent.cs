using Auctus.DomainObjects.Accounts;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class WithdrawalEvent
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.UInt32)]
        public int PensionFundTransactionId { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public int Period { get; set; }
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public string Responsable { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double EmployeeBonus { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double EmployeeAbsoluteBonus { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double EmployerTokenCashback { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double EmployeeTokenCashback { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double EmployerSzaboCashback { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double EmployeeSzaboCashback { get; set; }
    }
}
