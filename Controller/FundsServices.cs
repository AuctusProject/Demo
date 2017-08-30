using Auctus.Business.Funds;
using Auctus.DomainObjects.Contracts;
using Auctus.DomainObjects.Funds;
using Auctus.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public class FundsServices
    {
        public string CreateCompleteEntry(Int32 userId,
            string pensionFundName, string pensionFundDescription,
            string companyName, string companyDescription, double bonusFee, double maxBonusFee,
            string employeeName, string employeeSalary, string meployeeContributionPercentage, 
            IEnumerable<PensionFundOption> options)
        {
            return new PensionFundBusiness().CreateCompleteEntry(userId,
             pensionFundName, pensionFundDescription,
             companyName, companyDescription, bonusFee, maxBonusFee,
             employeeName, employeeSalary, meployeeContributionPercentage,
             options);
        }

        public PensionFundContract CreateCompleteEntry(Fund fund, Company company, Employee employee)
        {
            return new PensionFundBusiness().CreateCompleteEntry(fund, company, employee);
        }
    }
}
