using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Auctus.Web.Hubs;
using Auctus.Service;

namespace Web.Controllers
{
    public class SignalRPocController : Controller
    {
        private readonly IConnectionManager _connectionManager;

        public SignalRPocController(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
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

                var hubContext = _connectionManager.GetHubContext<AuctusDemoHub>();
                hubContext.Clients.All.deploy(msg);//Can also return a Json
            });

            return Content("Deploying your contract...");
        }
    }
}
