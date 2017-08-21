using Auctus.DomainObjects.Funds;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Accounts
{
    public class Company
    {
        public Int32 Id { get; set; }
        public Int32 PensionId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public Decimal BonusFee { get; set; }
        public Decimal MaxBonusFee { get; set; }
        public String WalletAddress { get; set; }

        public IEnumerable<Employee> Employees { get; set; }
        public PensionFund Fund { get; set; }
    }
}
