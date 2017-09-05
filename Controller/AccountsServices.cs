using Auctus.Business.Accounts;
using Auctus.DomainObjects.Accounts;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public class AccountsServices : BaseServices
    {
        public AccountsServices(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        #region Company
        public IEnumerable<Company> listAllCompanies()
        {
            return CompanyBusiness.ListAll();
        }

        #endregion  
    }
}
