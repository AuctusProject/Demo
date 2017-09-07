using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Auctus.Service;
using Microsoft.Extensions.Logging;
using Auctus.Web.Model.Home;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Auctus.Model;
using Auctus.Util;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Auctus.Web.Hubs;
using Auctus.DomainObjects.Contracts;

namespace Web.Controllers
{
    public class HomeController : HubBaseController
    {
        public HomeController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IConnectionManager connectionManager) : base(loggerFactory, cache, serviceProvider, connectionManager) { }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Save()//Wizard model)
        {
            var pensionFundContract = PensionFundsServices.CreateCompleteEntry(new Fund()
            {
                Fee = 5,
                LatePaymentFee = 5,
                AssetAllocations = new List<AssetAllocation>() {
                        new AssetAllocation(){
                            Percentage =100,
                            ReferenceContractAddress = "0x42a694a6587b9a14c766abfa15acdcca77c90405"
                        }
                    }
            },
                new Company()
                {
                    BonusFee = 100,
                    MaxBonusFee = 10,
                    VestingRules = new List<VestingRules>() {
                        new VestingRules() {
                            Percentage=20,
                            Period = 1
                        },
                        new VestingRules() {
                            Percentage=40,
                            Period = 2
                        },
                        new VestingRules() {
                            Percentage=60,
                            Period = 3
                        },
                        new VestingRules() {
                            Percentage=80,
                            Period = 4
                        },
                        new VestingRules() {
                            Percentage=100,
                            Period = 5
                        }
                    }
                },
                new Employee() { Name = "EmployeeName", ContributionPercentage = 10, Salary = 2000 });


            CheckContractCreationTransaction(pensionFundContract.TransactionHash);

            return Json(pensionFundContract);
        }

        [HttpPost]
        public void CheckContractCreationTransaction(String transactionHash)
        {
            Task.Factory.StartNew(() =>
            {
                var hubContext = HubConnectionManager.GetHubContext<AuctusDemoHub>();
                try
                {
                    var pensionFundContract = PensionFundsServices.CheckContractCreationTransaction(transactionHash);
                    if (pensionFundContract.BlockNumber.HasValue)
                        hubContext.Clients.Client(ConnectionId).deployCompleted(Json(
                            new { Address = pensionFundContract.Address,
                                BlockNumber = pensionFundContract.BlockNumber,
                                TransactionHash = pensionFundContract.TransactionHash }));
                    else
                        hubContext.Clients.Client(ConnectionId).deployUncompleted(pensionFundContract.TransactionHash);
                }
                catch(Exception ex)
                {
                    Logger.LogError(new EventId(1), ex, string.Format("Erro on CheckContractCreationTransaction {0}.", transactionHash));
                    hubContext.Clients.Client(ConnectionId).deployError();
                }
            });
        }
    }
}
