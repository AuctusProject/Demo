using Auctus.Business.Accounts;
using Auctus.Business.Contracts;
using Auctus.DataAccess.Funds;
using Auctus.DomainObjects.Contracts;
using Auctus.DomainObjects.Funds;
using Auctus.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Auctus.Business.Funds
{
    public class PensionFundBusiness : BaseBusiness<PensionFund, PensionFundData>
    {
        public string CreateCompleteEntry(Int32 userId,
            string pensionFundName, string pensionFundDescription,
            string companyName, string companyDescription, double bonusFee, double maxBonusFee,
            string employeeName, string employeeSalary, string meployeeContributionPercentage,
            IEnumerable<PensionFundOption> options)
        {
            //Insert();
            return "";
        }

        public PensionFundContract CreateCompleteEntry(Fund fund, Company company, Employee employee)
        {
            Validate(fund, company, employee);
            var pensionFund = new PensionFundBusiness().Create(fund.Name);
            var pensionFundWallet = new WalletBusiness().Create();
            var pensionFundOption = new PensionFundOptionBusiness().Create(pensionFundWallet.Address, fund.Fee, fund.LatePaymentFee, pensionFund.Id);
            var companyWallet = new WalletBusiness().Create();
            var domainCompany = new CompanyBusiness().Create(companyWallet.Address, company.Name, company.BonusFee, company.MaxBonusFee, pensionFundOption.Address, company.VestingRules);
            var employeeWallet = new WalletBusiness().Create();
            var domainEmployee = new EmployeeBusiness().Create(employeeWallet.Address, employee.Name, employee.Salary, employee.ContributionPercentage, domainCompany.Address);
            var pensionFundContract = new PensionFundContractBusiness().Create(pensionFundOption.Address, domainCompany.Address, domainEmployee.Address,
                pensionFundOption.Fee, pensionFundOption.LatePenalty, domainCompany.MaxSalaryBonusRate, domainEmployee.Contribution,
                domainCompany.BonusRate, domainEmployee.Salary, 
                fund.AssetAllocations.ToDictionary(asset => asset.ReferenceContractAddress, asset => asset.Percentage),
                company.VestingRules.ToDictionary(bonus => bonus.Period, bonus => bonus.Percentage));

            return pensionFundContract;
        }

        public PensionFund Create(String name)
        {
            var pensionFund = new PensionFund();
            pensionFund.Name = name;
            Insert(pensionFund);
            return pensionFund;
        }

        internal static void Validate(Fund fund, Company company, Employee employee)
        {
            Validate(fund);
            CompanyBusiness.Validate(company);
            EmployeeBusiness.Validate(employee);
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
