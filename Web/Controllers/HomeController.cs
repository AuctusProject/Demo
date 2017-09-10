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
                Name = "Simple Pension Fund",
                Fee = 3,
                LatePaymentFee = 0.07,
                AssetAllocations = new List<AssetAllocation>() {
                        new AssetAllocation(){
                            Percentage =20,
                            ReferenceContractAddress = "0xd9340654db260f5f69df1a6dd64ffe6be3632844"
                        },
                        new AssetAllocation(){
                            Percentage =80,
                            ReferenceContractAddress = "0x62e77625fd1ea74eefa35e040574c723d2275cd1"
                        }
                    }
            },
                new Company()
                {
                    Name = "Generic Company",
                    BonusFee = 100,
                    MaxBonusFee = 8,
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
                new Employee() { Name = "John Smith", ContributionPercentage = 10, Salary = 2000 });


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
