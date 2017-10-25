using Auctus.Util.DapperAttributes;
using Auctus.DomainObjects.Unprocessed;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Unprocessed
{
    public class UPensionFundData : BaseData<UPensionFund>
    {
        public override string TableName => "UPensionFund";
    }
}
