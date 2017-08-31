using Auctus.DataAccess.Accounts;
using Auctus.DomainObjects.Accounts;
using System;
using System.Collections.Generic;
using System.Text;
using Auctus.Model;
using Auctus.Util;

namespace Auctus.Business.Accounts
{
    public class BonusDistributionBusiness : BaseBusiness<BonusDistribution, BonusDistributionData>
    {
        public BonusDistributionBusiness(Cache cache) : base(cache) { }

        internal static void Validate(Model.Company company)
        {
            if (company == null)
                throw new ArgumentNullException("company");
            if (company.BonusFee < 0 || company.BonusFee > 100)
                throw new ArgumentException("Bonus Fee should be a value between 0 and 100.");
            if (company.MaxBonusFee < 0 || company.MaxBonusFee > 100)
                throw new ArgumentException("Max Bonus Fee should be a value between 0 and 100.");
        }

        public BonusDistribution Create(String companyAddress, Int32 period, double releasedBonus)
        {
            var bonusDistribution = new BonusDistribution()
            {
                CompanyAddress = companyAddress,
                Period = period,
                ReleasedBonus = releasedBonus
            };
            Insert(bonusDistribution);
            return bonusDistribution;
        }

        internal void Create(string address, IEnumerable<VestingRules> vestingRules)
        {
            foreach(var vestingRule in vestingRules)
            {
                Create(address, vestingRule.Period, vestingRule.Percentage);
            }
        }
    }
}
