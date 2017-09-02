using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Contracts
{
    public class ReferenceContractBusiness : BaseBusiness<ReferenceContract, ReferenceContractData>
    {
        public ReferenceContractBusiness(Cache cache) : base(cache) { }

        public ReferenceContract GetGoldReference()
        {
            return Get(ReferenceType.Gold);
        }

        public ReferenceContract GetSP500Reference()
        {
            return Get(ReferenceType.SP500);
        }

        public ReferenceContract GetMSCIWorldReference()
        {
            return Get(ReferenceType.MSCIWorld);
        }

        public ReferenceContract GetVWEHXReference()
        {
            return Get(ReferenceType.VWEHX);
        }

        public ReferenceContract GetBitcoinReference()
        {
            return Get(ReferenceType.Bitcoin);
        }

        public ReferenceContract Get(ReferenceType type)
        {
            return Get(type.Address);
        }

        private ReferenceContract Get(string address)
        {
            string cacheKey = string.Format("ReferenceContract{0}", address);
            ReferenceContract reference = MemoryCache.Get<ReferenceContract>(cacheKey);
            if (reference == null)
            {
                reference = Data.Get(address);
                if (reference != null)
                    MemoryCache.Create<ReferenceContract>(cacheKey, reference, 1440);
            }
            return reference;
        }
    }
}
