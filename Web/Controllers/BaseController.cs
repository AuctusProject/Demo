using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Auctus.Util;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Auctus.Service;

namespace Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ILogger logger;
        protected readonly IConnectionManager connectionManager;
        protected readonly Cache memoryCache;

        protected BaseController(ILoggerFactory loggerFactory, IConnectionManager connection, Cache cache, IServiceProvider serviceProvider)
        {
            memoryCache = cache;
            connection = connectionManager;
            logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        protected FundsServices FundsServices { get { return new FundsServices(memoryCache); } }
        protected SecurityServices SecurityServices { get { return new SecurityServices(memoryCache); } }
        protected ContractsServices ContractsServices { get { return new ContractsServices(memoryCache); } }
        protected AccountsServices AccountsServices { get { return new AccountsServices(memoryCache); } }
    }
}