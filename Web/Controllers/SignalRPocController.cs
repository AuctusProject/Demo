using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Auctus.Web.Hubs;
using Auctus.Service;
using Microsoft.Extensions.Logging;
using Auctus.Util;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Controllers
{
    public class SignalRPocController : HubBaseController
    {
        public SignalRPocController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IConnectionManager connectionManager):
            base(loggerFactory, cache, serviceProvider, connectionManager)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Deploy()
        {
            Task.Run(() =>
            {
                var success = FundsServices.DeployContract();
                var msg = success ? "Your contract has been deployed" : "Contract deploy failed";

                var hubContext = HubConnectionManager.GetHubContext<AuctusDemoHub>();
                hubContext.Clients.Client(ConnectionId).deploy(msg);//Can also return a Json
            });

            return Content("Deploying your contract...");
        }
    }
}
