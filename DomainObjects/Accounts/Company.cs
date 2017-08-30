using Auctus.DomainObjects.Accounts;
using Auctus.DomainObjects.Funds;
using Auctus.DomainObjects.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Accounts
{
    public class Company
    {
        public String Address { get; set; }
        public String Name { get; set; }
        public Double BonusRate { get; set; }
        public Double MaxSalaryBonusRate { get; set; }
        public String PensionFundOptionAddress { get; set; }

        public IEnumerable<Employee> Employees { get; set; }
        public IEnumerable<BonusDistribution> BonusDistribution { get; set; }
        public PensionFundOption PensionFundOption { get; set; }
        public Wallet Wallet { get; set; }
    }
}
