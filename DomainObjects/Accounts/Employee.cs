using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Accounts
{
    public class Employee
    {
        public Int32 Id { get; set; }
        public Int32 CompanyId { get; set; }
        public Int32 ContractId { get; set; }
        public String Name { get; set; }
        public Decimal Salary { get; set; }
        public Decimal ContributionPercentage { get; set; }
        public String WalletAddress { get; set; }

        public Company Company { get; set ; }
        public Contract Contract { get; set; }
    }
}
