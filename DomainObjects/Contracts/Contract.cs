using Auctus.DomainObjects.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class Contract
    {
        public Int32 Id { get; set; }
        public Int32 SmartContractId { get; set; }
        public Int32 PensionFundOptionId { get; set; }
        public Int32 CompanyId { get; set; }
        public Int32 TransactionCode { get; set; }
        public String Address { get; set; }

        public Company Company { get; set; }
        public SmartContract SmartContract { get; set; }
    }
}
