using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace Web.Controllers
{
    public class BaseController : Controller
    {
        protected ILogger Logger { get; }
        protected IMemoryCache Cache { get; private set; }

        public BaseController(IMemoryCache memoryCache, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            Logger = loggerFactory.CreateLogger(GetType().Namespace);
            Cache = memoryCache;
        }
    }
}