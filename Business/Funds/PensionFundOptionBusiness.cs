using Auctus.Business.Accounts;
using Auctus.Business.Contracts;
using Auctus.DataAccess.Funds;
using Auctus.DomainObjects.Funds;
using Auctus.EthereumProxy;
using Auctus.Model;
using System;
using System.Collections.Generic;

namespace Auctus.Business.Funds
{
    public class PensionFundOptionBusiness : BaseBusiness<PensionFundOption, PensionFundOptionData>
    {
        public PensionFundOption Create(String address, Decimal Fee, Decimal LatePenalty, Int32 PensionFundId)
        {
            PensionFundOption pensionFundOption = new PensionFundOption();
            pensionFundOption.Address = address;
            pensionFundOption.Fee = Fee;
            pensionFundOption.LatePenalty = LatePenalty;
            pensionFundOption.PensionFundId = PensionFundId;
            Insert(pensionFundOption);
            return pensionFundOption;
        }
        
    }
}
