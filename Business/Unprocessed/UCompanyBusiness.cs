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

namespace Auctus.Business.Unprocessed
{
    public class UCompanyBusiness : BaseBusiness<UCompany, UCompanyData>
    {
        public UCompanyBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache)
        {
        }
    }
}
