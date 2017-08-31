using Auctus.Business.Accounts;
using Auctus.DomainObjects.Accounts;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public class AccountsServices : BaseServices
    {
        public AccountsServices(Cache cache) : base(cache) { }

        #region Company
        public IEnumerable<Company> listAllCompanies()
        {
            return CompanyBusiness.ListAll();
        }

        #endregion  
    }
}
