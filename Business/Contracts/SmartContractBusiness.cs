using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Auctus.EthereumProxy;
using Auctus.Util;
using Microsoft.Extensions.Logging;

namespace Auctus.Business.Contracts
{
    public class SmartContractBusiness : BaseBusiness<SmartContract, SmartContractData>
    {
        public SmartContractBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        public SmartContract GetDefaultDemonstrationPensionFund()
        {
            return Get(SmartContractType.PensionFund.Type);
        }

        public SmartContract GetDefaultReferenceValue()
        {
            return Get(SmartContractType.ReferenceValue.Type);
        }
        
        private SmartContract Get(int id)
        {
            string cacheKey = string.Format("SmartContract{0}", id);
            SmartContract sc = MemoryCache.Get<SmartContract>(cacheKey);
            if (sc == null)
            {
                sc = Data.Get(id);
                if (sc != null)
                    MemoryCache.Set<SmartContract>(cacheKey, sc, 1440);
            }
            return sc;
        }
    }
}
