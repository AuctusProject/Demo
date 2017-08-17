using Auctus.DomainObjects.Funds;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Funds
{
    public class PensionFundData : BaseData<PensionFund>
    {
        public override string TableName => "PensionFund";
    }
}
