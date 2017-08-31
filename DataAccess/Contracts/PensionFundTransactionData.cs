using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Contracts
{
    public class PensionFundTransactionData : BaseData<PensionFundTransaction>
    {
        public override string TableName => "PensionFundTransaction";
    }
}
