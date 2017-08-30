using Auctus.DomainObjects.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class ReferenceContract
    {
        public String Address { get; set; }
        public String Description { get; set; }
        public Int32 SmartContractId { get; set; }

        public SmartContract SmartContract { get; set; }
    }
}
