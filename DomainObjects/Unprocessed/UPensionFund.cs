using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Unprocessed
{
    public class UPensionFund
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 Id { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public String Name { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double Fee { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double LatePaymentFee { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double GoldPercentage { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double SPPercentage { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double VWEHXPercentage { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double MSCIPercentage { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public Double BitcoinPercentage { get; set; }
        [DapperType(System.Data.DbType.UInt32)]
        public Int32 Processed { get; set; }

        public UCompany Company { get; set; }
    }
}
