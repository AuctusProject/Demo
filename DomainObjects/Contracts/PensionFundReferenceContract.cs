using Auctus.DomainObjects.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class PensionFundReferenceContract
    {
        public Int32 PensionFundContractId { get; set; }
        public String ReferenceContractAddress { get; set; }
        public Double Percentage { get; set; }

        public SmartContract SmartContract { get; set; }
    }
}
