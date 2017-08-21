using Auctus.Business.Accounts;
using Auctus.DomainObjects.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public class AccountsService
    {
        #region Company
        public IEnumerable<Company> listAllCompanies()
        {
            return new CompanyBusiness().ListAll();
        }

        #endregion  
    }
}
