using Auctus.DataAccess.Funds;
using Auctus.DomainObjects.Funds;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Funds
{
    public class PensionFundBusiness : BaseBusiness<PensionFund, PensionFundData>
    {
        public string CreateCompleteEntry(Int32 userId,
            string pensionFundName, string pensionFundDescription,
            string companyName, string companyDescription, decimal bonusFee, decimal maxBonusFee,
            string employeeName, string employeeSalary, string meployeeContributionPercentage,
            IEnumerable<PensionFundOption> options)
        {
            //Insert();
            return "";
        }
    }
}
