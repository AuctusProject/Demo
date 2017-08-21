using Auctus.Business.Funds;
using Auctus.DomainObjects.Funds;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public class FundsServices
    {
        public string CreateCompleteEntry(Int32 userId,
            string pensionFundName, string pensionFundDescription,
            string companyName, string companyDescription, decimal bonusFee, decimal maxBonusFee,
            string employeeName, string employeeSalary, string meployeeContributionPercentage, 
            IEnumerable<PensionFundOption> options)
        {
            return new PensionFundBusiness().CreateCompleteEntry(userId,
             pensionFundName, pensionFundDescription,
             companyName, companyDescription, bonusFee, maxBonusFee,
             employeeName, employeeSalary, meployeeContributionPercentage,
             options);
        }
    }
}
