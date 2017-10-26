using Auctus.DomainObjects.Accounts;
using Auctus.DomainObjects.Funds;
using Auctus.DomainObjects.Unprocessed;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using Auctus.DataAccess.Unprocessed;
using Auctus.Model;

namespace Auctus.Business.Unprocessed
{
    public class UCompanyBusiness : BaseBusiness<UCompany, UCompanyData>
    {
        public UCompanyBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache)
        {
        }

        internal UCompany Create(Model.Company company, int uPensionFundId)
        {
            var uCompany = new UCompany();
            uCompany.Name = company.Name;
            uCompany.BonusFee = company.BonusFee;
            uCompany.MaxBonusFee = company.MaxBonusFee;
            uCompany.UPensionFundId = uPensionFundId;
            Insert(uCompany);

            foreach (VestingRules rule in company.VestingRules)
            {
                UVestingRuleBusiness.Create(rule, uCompany.Id);
            }

            return uCompany;
        }
    }
}
