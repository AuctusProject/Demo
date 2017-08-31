using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Controllers
{
    public class HubBaseController : BaseController
    {
        protected readonly IConnectionManager HubConnectionManager;
        protected string ConnectionId { get; set; }

        public HubBaseController(IMemoryCache memoryCache, ILoggerFactory loggerFactory, IServiceProvider serviceProvider, IConnectionManager connectionManager)
            : base(memoryCache, loggerFactory, serviceProvider)
        {
            HubConnectionManager = connectionManager;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string connectionId = HttpContext.Request.Headers["HubConnectionId"];
            if (!string.IsNullOrEmpty(connectionId) && connectionId != "undefined" )
                ConnectionId = connectionId;
            base.OnActionExecuting(filterContext);
        }
    }
}