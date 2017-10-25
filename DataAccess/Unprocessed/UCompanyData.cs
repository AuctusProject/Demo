using Auctus.DomainObjects.Accounts;
using Auctus.DomainObjects.Funds;
using Auctus.DomainObjects.Unprocessed;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Unprocessed
{
    public class UCompanyData : BaseData<UCompany>
    {
        public override string TableName => "UCompany";
    }
}
