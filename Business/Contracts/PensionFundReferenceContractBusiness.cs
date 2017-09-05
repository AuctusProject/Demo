using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Contracts
{
    public class PensionFundReferenceContractBusiness : BaseBusiness<PensionFundReferenceContract, PensionFundReferenceContractData>
    {
        public PensionFundReferenceContractBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        internal List<PensionFundReferenceContract> List(String pensionFundContractHash)
        {
            return Data.List(pensionFundContractHash);
        }

        internal void Delete(String pensionFundContractHash)
        {
            Data.Delete(pensionFundContractHash);
        }

        internal PensionFundReferenceContract Create(String pensionFundContractHash, String referenceContractAddress, double percentage)
        {
            var pensionFundReferenceContract = new PensionFundReferenceContract
            {
                PensionFundContractHash = pensionFundContractHash,
                ReferenceContractAddress = referenceContractAddress,
                Percentage = percentage
            };
            Insert(pensionFundReferenceContract);
            return pensionFundReferenceContract;
        }
    }
}
