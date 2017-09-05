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
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;
        protected readonly Cache MemoryCache;

        protected BaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider)
        {
            MemoryCache = cache;
            LoggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        protected FundsServices FundsServices { get { return new FundsServices(LoggerFactory, MemoryCache); } }
        protected SecurityServices SecurityServices { get { return new SecurityServices(LoggerFactory, MemoryCache); } }
        protected ContractsServices ContractsServices { get { return new ContractsServices(LoggerFactory, MemoryCache); } }
        protected AccountsServices AccountsServices { get { return new AccountsServices(LoggerFactory, MemoryCache); } }
    }
}