using Auctus.Util.DapperAttributes;
using Auctus.DomainObjects.Unprocessed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auctus.DataAccess.Unprocessed;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using Auctus.Model;
using Auctus.DomainObjects.Funds;
using Auctus.EthereumProxy;
using Auctus.DomainObjects.Contracts;

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
            uPensionFund.Processed = 0;
            Insert(uPensionFund);
            return uPensionFund;

        }

        internal List<UPensionFund> ListUnprocessed()
        {
            return Data.ListUnprocessed();
        }
        
        public Tuple<string, string> CheckPensionFundCreation(int pensionFundId)
        {
            UPensionFund uPensionFund = Data.Get(pensionFundId);
            if (uPensionFund.Processed > 0)
            {
                PensionFund pensionFund = PensionFundBusiness.GetById(uPensionFund.Processed);

                List<PensionFundReferenceContract> referenceDistribution = PensionFundReferenceContractBusiness.List(pensionFund.Option.PensionFundContract.TransactionHash);
                if (!referenceDistribution.Any())
                    throw new InvalidOperationException("Reference contract cannot be found.");

                pensionFund.Option.Company.BonusDistribution = BonusDistributionBusiness.List(pensionFund.Option.Company.Address);
                if (!pensionFund.Option.Company.BonusDistribution.Any())
                    throw new ArgumentException("Bonus distribution cannot be found.");

                string smartContractCode = EthereumManager.GetPensionFundSmartContractCode(
                    pensionFund.Option.Address, pensionFund.Option.Company.Address, pensionFund.Option.Company.Employee.Address,
                    pensionFund.Option.Fee, pensionFund.Option.LatePenalty, AUCTUS_FEE, 
                    pensionFund.Option.Company.MaxSalaryBonusRate, pensionFund.Option.Company.Employee.Contribution,
                    pensionFund.Option.Company.BonusRate, pensionFund.Option.Company.Employee.Salary,
                    referenceDistribution.ToDictionary(c => c.ReferenceContractAddress, c => c.Percentage),
                    pensionFund.Option.Company.BonusDistribution.ToDictionary(c => c.Period, c => c.ReleasedBonus));

                return new Tuple<string, string>(pensionFund.Option.PensionFundContract.TransactionHash, smartContractCode);
            }
            else
                return null;
        }
    }
}
