using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Contracts
{
    public class PensionFundContractData : BaseData<PensionFundContract>
    {
        public override string TableName => "PensionFundContract";
    }
}
