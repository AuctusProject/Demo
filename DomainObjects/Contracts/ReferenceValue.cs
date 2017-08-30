using Auctus.DomainObjects.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class ReferenceValue
    {
        public String ReferenceContractAddress { get; set; }
        public Int32 Period { get; set; }
        public Double Value { get; set; }

        public ReferenceContract ReferenceContract { get; set; }
    }
}
