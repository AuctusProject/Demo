using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Contracts
{
    public class PensionFundTransactionBusiness : BaseBusiness<PensionFundTransaction, PensionFundTransactionData>
    {
        public PensionFundTransactionBusiness(Cache cache) : base(cache) { }


    }
}
