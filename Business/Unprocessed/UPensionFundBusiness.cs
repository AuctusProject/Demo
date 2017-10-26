using Auctus.Util.DapperAttributes;
using Auctus.DomainObjects.Unprocessed;
using System;
using System.Collections.Generic;
using System.Text;
using Auctus.DataAccess.Unprocessed;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using Auctus.Model;

namespace Auctus.Business.Unprocessed
{
    public class UPensionFundBusiness : BaseBusiness<UPensionFund, UPensionFundData>
    {
        public UPensionFundBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache)
        {
        }

        internal UPensionFund Create(Fund fund)
        {
            var uPensionFund = new UPensionFund();
            uPensionFund.Name = fund.Name;
            uPensionFund.Fee = fund.Fee;
            uPensionFund.LatePaymentFee = fund.LatePaymentFee;
            uPensionFund.GoldPercentage = fund.GoldPercentage;
            uPensionFund.SPPercentage = fund.SPPercentage;
            uPensionFund.VWEHXPercentage = fund.VWEHXPercentage;
            uPensionFund.MSCIPercentage = fund.MSCIPercentage;
            uPensionFund.BitcoinPercentage = fund.BitcoinPercentage;
            uPensionFund.Processed = false;
            Insert(uPensionFund);
            return uPensionFund;

        }

        internal List<UPensionFund> ListUnprocessed()
        {
            return Data.ListUnprocessed();
        }
    }
}
