using Auctus.DomainObjects.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Accounts
{
    public class CompanyData : BaseData<Company>
    {
        public override string TableName => "Company";
    }
}
