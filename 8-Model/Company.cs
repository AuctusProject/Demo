using System;
using System.Collections.Generic;

namespace Auctus.Model
{
    public class Company
    {
        public String Name { get; set; }
        public Decimal BonusFee { get; set; }
        public Decimal MaxBonusFee { get; set; }

        public IEnumerable<VestingRules> VestingRules { get; set; }
    }
}

