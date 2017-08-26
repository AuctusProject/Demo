using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Auctus.Business.Contracts
{
    public class ContractBusiness : BaseBusiness<Contract, ContractData>
    {
        internal static void Validate(Model.Contract contract)
        {
            if (contract == null)
                throw new ArgumentNullException("contract");
            ValidateVestingRules(contract.VestingRules);
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
    }
}
