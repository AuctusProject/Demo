using System;
using System.Collections.Generic;

namespace Auctus.Model
{
    public class Company
    {
        public String Name { get; set; }
        public Double BonusFee { get; set; }
        public Double MaxBonusFee { get; set; }

        public IEnumerable<VestingRules> VestingRules { get; set; }
    }
}

