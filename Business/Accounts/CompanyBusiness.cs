using Auctus.DataAccess.Accounts;
using Auctus.DomainObjects.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Accounts
{
    public class CompanyBusiness : BaseBusiness<Company, CompanyData>
    {
        internal static void Validate(Model.Company company)
        {
            if (company == null)
                throw new ArgumentNullException("company");
            if (company.BonusFee < 0 || company.BonusFee > 100)
                throw new ArgumentException("Bonus Fee should be a value between 0 and 100.");
            if (company.MaxBonusFee < 0 || company.MaxBonusFee > 100)
                throw new ArgumentException("Max Bonus Fee should be a value between 0 and 100.");
        }
    }
}
