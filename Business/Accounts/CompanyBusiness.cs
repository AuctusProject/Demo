using Auctus.DataAccess.Accounts;
using Auctus.DomainObjects.Accounts;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Auctus.Business.Accounts
{
    public class CompanyBusiness : BaseBusiness<Company, CompanyData>
    {
        public CompanyBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        internal static void Validate(Model.Company company)
        {
            if (company == null)
                throw new ArgumentNullException("company");
            if (company.BonusFee < 0 || company.BonusFee > 100)
                throw new ArgumentException("Bonus Fee should be a value between 0 and 100.");
            if (company.MaxBonusFee < 0 || company.MaxBonusFee > 100)
                throw new ArgumentException("Max Bonus Fee should be a value between 0 and 100.");

            ValidateVestingRules(company.VestingRules);
        }
        
        private static void ValidateVestingRules(IEnumerable<Model.VestingRules> vestingRules)
        {
            if (vestingRules != null && vestingRules.Any())
            {
                Model.VestingRules previousVestingRule = null;
                foreach (var vestingRule in vestingRules)
                {
                    if (previousVestingRule != null)
                    {
                        if (vestingRule.Period <= previousVestingRule.Period)
                        {
                            throw new ArgumentException("Vesting Rules periods should be crescent");
                        }
                        if (vestingRule.Percentage <= previousVestingRule.Percentage)
                        {
                            throw new ArgumentException("Vesting Rules percentages should be crescent");
                        }
                    }
                    previousVestingRule = vestingRule;
                }
            }
        }

        public Company Create(String address, String name, double bonusRate, double maxSalaryBonusRate, String pensionFundOptionAddress, IEnumerable<Model.VestingRules> vestingRules)
        {
            var company = new Company()
            {
                Address = address,
                BonusRate = bonusRate,
                MaxSalaryBonusRate = maxSalaryBonusRate,
                PensionFundOptionAddress = pensionFundOptionAddress,
                Name = name
            };
            Insert(company);
            BonusDistributionBusiness.Create(company.Address, vestingRules);
            return company;
        }
    }
}
