using Auctus.DomainObjects.Contracts;
using Auctus.DomainObjects.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Accounts
{
    public class Employee
    {
        public String Address { get; set; }
        public String Name { get; set; }
        public Double Salary { get; set; }
        public Double Contribution { get; set; }
        public String CompanyAddress { get; set; }
        
        public Company Company { get; set ; }
        public Wallet Wallet { get; set; }
    }
}
