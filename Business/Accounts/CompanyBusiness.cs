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
            if (company.BonusFee <= 0)
                throw new ArgumentException("Bonus Fee must be greater then zero.");
            if (company.BonusFee >= 1000)
                throw new ArgumentException("Bonus Fee must be lesser then 1000.");
            if (company.MaxBonusFee <= 0)
                throw new ArgumentException("Max Bonus Fee must be greater then zero.");
            if (company.MaxBonusFee > 100)
                throw new ArgumentException("Max Bonus Fee must be lesser then 100.");

            ValidateVestingRules(company.VestingRules);
        }
        
        private static void ValidateVestingRules(IEnumerable<Model.VestingRules> vestingRules)
        {
            if (vestingRules == null || !vestingRules.Any())
                throw new ArgumentNullException("VestingRules");
            if (vestingRules.Any(c => c.Percentage > 100))
                throw new ArgumentException("Maximum is 100%.");
            if (vestingRules.Count() != vestingRules.Select(c => c.Period).Distinct().Count())
                throw new ArgumentException("Periods must be unique.");
            if (vestingRules.Count() != vestingRules.Select(c => c.Percentage).Distinct().Count())
                throw new ArgumentException("Percentage must be unique.");
            if (vestingRules.Count() > 10)
                throw new ArgumentException("Maximum of 10 vesting rules are allowed.");

            vestingRules = vestingRules.OrderBy(c => c.Period);
            if (vestingRules.Last().Percentage != 100)
                throw new ArgumentException("Last period must be 100 percentage.");

            Model.VestingRules previousVestingRule = null;
            foreach (var vestingRule in vestingRules)
            {
                if (previousVestingRule != null && vestingRule.Percentage < previousVestingRule.Percentage)
                        throw new ArgumentException("Vesting Rules percentages should be crescent.");

                previousVestingRule = vestingRule;
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
