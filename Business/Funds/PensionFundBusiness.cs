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
using Auctus.Util;

namespace Auctus.Business.Funds
{
    public class PensionFundBusiness : BaseBusiness<PensionFund, PensionFundData>
    {
        public PensionFundBusiness(Cache cache) : base(cache) { }

        public PensionFund Get(string contractAddress)
        {
            if (!EthereumProxy.EthereumManager.IsValidAddress(contractAddress))
                throw new Exception("Invalid contract address.");

            string cacheKey = string.Format("PensionFund{0}", contractAddress);
            PensionFund pensionFund = MemoryCache.Get<PensionFund>(cacheKey);
            if (pensionFund == null)
                MemoryCache.Create<PensionFund>(cacheKey, GetFromDatabase(contractAddress));

            return pensionFund;
        }

        private PensionFund GetFromDatabase(string contractAddress)
        {
            PensionFund pensionFund = Data.Get(contractAddress);
            if (pensionFund == null || pensionFund.Option.Company == null || pensionFund.Option.Company.Employee == null)
                throw new Exception("Pension fund cannot be found.");

            pensionFund.Option.PensionFundContract.PensionFundReferenceContract = PensionFundReferenceContractBusiness.List(pensionFund.Option.PensionFundContract.TransactionHash);
            if (!pensionFund.Option.PensionFundContract.PensionFundReferenceContract.Any())
                throw new Exception("Reference contract cannot be found.");

            pensionFund.Option.Company.BonusDistribution = BonusDistributionBusiness.List(pensionFund.Option.Company.Address);
            if (!pensionFund.Option.Company.BonusDistribution.Any())
                throw new Exception("Bonus distribution cannot be found.");

            return pensionFund;
        }

        public PensionFundContract CreateCompleteEntry(Fund fund, Company company, Employee employee)
        {
            Validate(fund, company, employee);
            var pensionFund = PensionFundBusiness.Create(fund.Name);
            var pensionFundWallet = WalletBusiness.Create();
            var pensionFundOption = PensionFundOptionBusiness.Create(pensionFundWallet.Address, fund.Fee, fund.LatePaymentFee, pensionFund.Id);
            var companyWallet = WalletBusiness.Create();
            var domainCompany = CompanyBusiness.Create(companyWallet.Address, company.Name, company.BonusFee, company.MaxBonusFee, pensionFundOption.Address, company.VestingRules);
            var employeeWallet = WalletBusiness.Create();
            var domainEmployee = EmployeeBusiness.Create(employeeWallet.Address, employee.Name, employee.Salary, employee.ContributionPercentage, domainCompany.Address);
            var pensionFundContract = PensionFundContractBusiness.Create(pensionFundOption.Address, domainCompany.Address, domainEmployee.Address,
                pensionFundOption.Fee, pensionFundOption.LatePenalty, domainCompany.MaxSalaryBonusRate, domainEmployee.Contribution,
                domainCompany.BonusRate, domainEmployee.Salary, 
                fund.AssetAllocations.ToDictionary(asset => asset.ReferenceContractAddress, asset => asset.Percentage),
                company.VestingRules.ToDictionary(bonus => bonus.Period, bonus => bonus.Percentage));
            foreach (AssetAllocation asset in fund.AssetAllocations)
                PensionFundReferenceContractBusiness.Create(pensionFundContract.TransactionHash, asset.ReferenceContractAddress, asset.Percentage);

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

        //TODO: REMOVE/REFACTOR AFTER IMPLEMENTATION
        public bool DeployContract()
        {
            //Do Something
            //Call EthereumProxy if the "Something" is succeded

            Thread.Sleep(3000); //Simulate blockchain processing time

            return true;
        }
    }
}
