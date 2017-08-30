using Auctus.DomainObjects.Accounts;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class PensionFundContract
    {
        public Int32 Id { get; set; }
        public String Address { get; set; }
        public String TransactionHash { get; set; }
        public DateTime CreationDate { get; set; }
        public Int32 GasUsed { get; set; }
        public Int32 BlockNumber { get; set; }
        public Int32 SmartContractId { get; set; }
        public String PensionFundOptionAddress { get; set; }
        
        public SmartContract SmartContract { get; set; }

        [DapperIgnore]
        public string SmartContractCode { get; set; }
    }
}
