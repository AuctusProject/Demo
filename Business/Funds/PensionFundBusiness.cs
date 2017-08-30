using Auctus.Business.Accounts;
using Auctus.Business.Contracts;
using Auctus.DataAccess.Funds;
using Auctus.DomainObjects.Funds;
using Auctus.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public void CreateCompleteEntry(Fund fund, Company company, Employee employee, Contract contract)
        {
            Validate(fund);
            CompanyBusiness.Validate(company);
            EmployeeBusiness.Validate(employee);
            ContractBusiness.Validate(contract);
        }

        internal static void Validate(Fund fund)
        {
            if (fund == null)
                throw new ArgumentNullException("fund");
            if (fund.LatePaymentFee < 0)
                throw new ArgumentException("Late Payment Fee cannot be negative.");
            if (fund.Fee < 0)
                throw new ArgumentException("Fee cannot be negative.");
            if (fund.Fee > 99)
                throw new ArgumentException("Fee cannot be greater than 99.");
        }

        public static bool DeployContract()
        {
            //Do Something
            //Call EthereumProxy if the "Something" is succeded

            Thread.Sleep(3000); //Simulate blockchain processing time

            return true;
        }
    }
}
