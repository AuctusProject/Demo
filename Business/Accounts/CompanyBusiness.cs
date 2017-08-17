using Auctus.DataAccess.Accounts;
using DomainObjects.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Accounts
{
    public class CompanyBusiness : BaseBusiness<Company, CompanyData>
    {
        public override CompanyData data =>  new CompanyData();

        // Some specific business method 
    }
}
