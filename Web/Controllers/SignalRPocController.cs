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

namespace Web.Controllers
{
    public class SignalRPocController : BaseController
    {
        public SignalRPocController(ILoggerFactory loggerFactory, IConnectionManager connection, Cache cache, IServiceProvider serviceProvider) : base (loggerFactory, connection, cache, serviceProvider) { }

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

                var hubContext = connectionManager.GetHubContext<AuctusDemoHub>();
                hubContext.Clients.All.deploy(msg);//Can also return a Json
            });

            return Content("Deploying your contract...");
        }
    }
}
