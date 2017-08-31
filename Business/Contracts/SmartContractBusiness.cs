using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Auctus.EthereumProxy;
using Auctus.Util;

namespace Auctus.Business.Contracts
{
    public class SmartContractBusiness : BaseBusiness<SmartContract, SmartContractData>
    {
        public SmartContractBusiness(Cache cache) : base(cache) { }

        public SmartContract GetDefaultDemonstrationPensionFund()
        {
            return Get("Default Demonstration Pension Fund");
        }

        public SmartContract GetDefaultReferenceValue()
        {
            return Get("Default Reference Value");
        }

        private SmartContract Get(String name)
        {
            return ListAll().First(sc => sc.Name == name);
        }
    }
}
