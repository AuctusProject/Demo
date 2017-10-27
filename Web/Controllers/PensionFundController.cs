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
using Auctus.Web.Hubs;
using Auctus.DomainObjects.Contracts;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Web.Controllers
{
    public class PensionFundController : HubBaseController
    {
        private static readonly ConcurrentDictionary<string, string> CONTRACT_TRANSACTING = new ConcurrentDictionary<string, string>();

        public PensionFundController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IConnectionManager connectionManager) : base(loggerFactory, cache, serviceProvider, connectionManager) { }

        [Route("/PensionFund/{contractAddress}")]
        public IActionResult Index(string contractAddress)
        {
            return View(PensionFundsServices.GetPensionFundInfo(contractAddress));
        }

        [HttpGet]
        [Route("/PensionFund/GetWithdrawalInfo")]
        public IActionResult GetWithdrawalInfo(string contractAddress)
        {
            return Json(PensionFundsServices.GetWithdrawalInfo(contractAddress));
        }

        [HttpPost]
        [Route("/PensionFund/GeneratePayment")]
        public IActionResult GeneratePayment(string contractAddress, int monthsAmount)
        {
            if (CanTransactWith(contractAddress))
            {
                try
                {
                    return Json(PensionFundsServices.GeneratePayment(contractAddress, monthsAmount),
                        new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
                }
                finally
                {
                    ReleaseTransactionWith(contractAddress);
                }
            }
            else
                return new EmptyResult();
        }

        [HttpPost]
        [Route("/PensionFund/ReadPayments")]
        public IActionResult ReadPayments(string contractAddress)
        {
            if (!string.IsNullOrEmpty(ConnectionId))
            {
                Task.Factory.StartNew(() =>
                {
                    var hubContext = HubConnectionManager.GetHubContext<AuctusDemoHub>();
                    try
                    {
                        if (CanTransactWith(contractAddress))
                        {
                            try
                            {
                                Progress progress = PensionFundsServices.ReadPayments(contractAddress);
                                if (progress.Completed)
                                    hubContext.Clients.Client(ConnectionId).paymentsCompleted(Json(progress).Value);
                                else
                                    hubContext.Clients.Client(ConnectionId).paymentsUncompleted(Json(progress).Value);
                            }
                            finally
                            {
                                ReleaseTransactionWith(contractAddress);
                            }
                        }
                        else
                            hubContext.Clients.Client(ConnectionId).readPaymentsError();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(new EventId(2), ex, string.Format("Erro on ReadPayments {0}.", contractAddress));
                        hubContext.Clients.Client(ConnectionId).readPaymentsError();
                    }
                });
                return Json(new { success = true });
            }
            else
                return Json(new { success = false });
        }
        
        [HttpPost]
        [Route("/PensionFund/GenerateWithdrawal")]
        public IActionResult GenerateWithdrawal(string contractAddress)
        {
            if (CanTransactWith(contractAddress))
            {
                try
                {
                    return Json(PensionFundsServices.GenerateWithdrawal(contractAddress),
                        new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
                }
                finally
                {
                    ReleaseTransactionWith(contractAddress);
                }
            }
            else
                return new EmptyResult();
        }

        [HttpPost]
        [Route("/PensionFund/ReadWithdrawal")]
        public IActionResult ReadWithdrawal(string contractAddress)
        {
            if (!string.IsNullOrEmpty(ConnectionId))
            {
                Task.Factory.StartNew(() =>
                {
                    var hubContext = HubConnectionManager.GetHubContext<AuctusDemoHub>();
                    try
                    {
                        Withdrawal withdrawal = PensionFundsServices.ReadWithdrawal(contractAddress);
                        if (withdrawal == null || withdrawal.Completed)
                            hubContext.Clients.Client(ConnectionId).withdrawalCompleted(Json(withdrawal).Value);
                        else
                            hubContext.Clients.Client(ConnectionId).withdrawalUncompleted(Json(withdrawal).Value);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(new EventId(3), ex, string.Format("Erro on ReadWithdrawal {0}.", contractAddress));
                        hubContext.Clients.Client(ConnectionId).readWithdrawalError();
                    }
                });
                return Json(new { success = true });
            }
            else
                return Json(new { success = false });
        }
        
        private bool CanTransactWith(string contractAddress)
        {
            return CONTRACT_TRANSACTING.TryAdd(contractAddress, null);
        }

        private bool ReleaseTransactionWith(string contractAddress)
        {
            string nullValue;
            return CONTRACT_TRANSACTING.TryRemove(contractAddress, out nullValue);
        }
    }
}
