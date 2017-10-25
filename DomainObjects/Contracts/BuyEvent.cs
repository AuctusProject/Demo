using Auctus.DomainObjects.Accounts;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class BuyEvent
    {
        [DapperKey(false)]
        [DapperType(System.Data.DbType.UInt32)]
        public int PensionFundTransactionId { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public int Period { get; set; }
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public string Responsable { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double TokenAmount { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double SzaboInvested { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double LatePenalty { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public int DaysOverdue { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double PensionFundFee { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double AuctusFee { get; set; }
    }
}
