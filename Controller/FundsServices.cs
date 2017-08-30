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
        public static string CreateCompleteEntry(Int32 userId,
            string pensionFundName, string pensionFundDescription,
            string companyName, string companyDescription, double bonusFee, double maxBonusFee,
            string employeeName, string employeeSalary, string meployeeContributionPercentage, 
            IEnumerable<PensionFundOption> options)
        {
            new PensionFundBusiness().CreateCompleteEntry(fund, company, employee, contract);
        }

        public static PensionFundContract CreateCompleteEntry(Fund fund, Company company, Employee employee)
        {
            return new PensionFundBusiness().CreateCompleteEntry(fund, company, employee);
        }

        //TODO: REMOVE/REFACTOR AFTER IMPLEMENTATION
        public static bool DeployContract()
        {
            return PensionFundBusiness.DeployContract();
        }
    }
}
