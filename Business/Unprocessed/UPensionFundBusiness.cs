using Auctus.Util.DapperAttributes;
using Auctus.DomainObjects.Unprocessed;
using System;
using System.Collections.Generic;
using System.Text;
using Auctus.DataAccess.Unprocessed;
using Auctus.Util;
using Microsoft.Extensions.Logging;

namespace Auctus.Business.Unprocessed
{
    public class UPensionFundBusiness : BaseBusiness<UPensionFund, UPensionFundData>
    {
        public UPensionFundBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache)
        {
        }
    }
}
