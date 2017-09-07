using Auctus.DomainObjects.Contracts;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public class ContractsServices : BaseServices
    {
        public ContractsServices(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        public ReferenceContract GetGoldReference()
        {
            return ReferenceContractBusiness.GetGoldReference();
        }

        public ReferenceContract GetSP500Reference()
        {
            return ReferenceContractBusiness.GetSP500Reference();
        }

        public ReferenceContract GetMSCIWorldReference()
        {
            return ReferenceContractBusiness.GetMSCIWorldReference();
        }

        public ReferenceContract GetVWEHXReference()
        {
            return ReferenceContractBusiness.GetVWEHXReference();
        }

        public ReferenceContract GetBitcoinReference()
        {
            return ReferenceContractBusiness.GetBitcoinReference();
        }
    }
}
