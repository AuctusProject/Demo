using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Contracts
{
    public class PensionFundReferenceContractBusiness : BaseBusiness<PensionFundReferenceContract, PensionFundReferenceContractData>
    {
        public PensionFundReferenceContractBusiness(Cache cache) : base(cache) { }

        internal List<PensionFundReferenceContract> List(int pensionFundContractId)
        {
            return Data.List(pensionFundContractId);
        }

        internal PensionFundReferenceContract Create(int pensionFundContractId, String referenceContractAddress, double percentage)
        {
            var pensionFundReferenceContract = new PensionFundReferenceContract
            {
                PensionFundContractId = pensionFundContractId,
                ReferenceContractAddress = referenceContractAddress,
                Percentage = percentage
            };
            Insert(pensionFundReferenceContract);
            return pensionFundReferenceContract;
        }
    }
}
