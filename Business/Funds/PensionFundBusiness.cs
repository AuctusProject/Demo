using Auctus.Business.Accounts;
using Auctus.Business.Contracts;
using Auctus.DataAccess.Funds;
using Auctus.DomainObjects.Funds;
using Auctus.EthereumProxy;
using Auctus.Model;
using System;
using System.Collections.Generic;

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
            Validate(fund, company, employee, contract);
            var pensionFund = new PensionFundBusiness().Create(fund.Name);
            var pensionFundWallet = new WalletBusiness().Create();
            var pensionFundOption = new PensionFundOptionBusiness().Create(pensionFundWallet.Address, fund.Fee, fund.LatePaymentFee, pensionFund.Id);
            var companyWallet = new WalletBusiness().Create();
            var domainCompany = new CompanyBusiness().Create(companyWallet.Address, company.Name, company.BonusFee, company.MaxBonusFee, pensionFundOption.Address, company.VestingRules);
            var employeeWallet = new WalletBusiness().Create();
            new EmployeeBusiness().Create(employeeWallet.Address, employee.Name, employee.Salary, employee.ContributionPercentage, domainCompany.Address);
        }

        public PensionFund Create(String name)
        {
            var pensionFund = new PensionFund();
            pensionFund.Name = name;
            Insert(pensionFund);
            return pensionFund;
        }

        internal static void Validate(Fund fund, Company company, Employee employee, Contract contract)
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
    }
}
