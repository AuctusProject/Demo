using Auctus.Business.Accounts;
using DomainObjects.Accounts;
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

        public void createCompany(String name)
        {
            new CompanyBusiness().Insert(name);
        }

        public void removeCompany(Int32 id)
        {
            new CompanyBusiness().DeleteById(id);
        }
        #endregion  
    }
}
