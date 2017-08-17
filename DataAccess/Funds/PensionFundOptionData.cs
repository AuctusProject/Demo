using Auctus.DomainObjects.Funds;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Funds
{
    public class PensionFundOptionData : BaseData<PensionFundOption>
    {
        public override string TableName => "PensionFundOption";
    }
}
