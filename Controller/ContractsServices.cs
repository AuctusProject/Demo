using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public class ContractsServices : BaseServices
    {
        public ContractsServices(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

    }
}
