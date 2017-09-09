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
    public class PensionFundController : HubBaseController
    {
        public PensionFundController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IConnectionManager connectionManager) : base(loggerFactory, cache, serviceProvider, connectionManager) { }

        [Route("/PensionFund/{contractAddress}")]
        public IActionResult Index(string contractAddress)
        {
            return View(PensionFundsServices.GetPensionFundInfo(contractAddress));
        }

        [HttpPost]
        public IActionResult GetWithdrawalInfo(string contractAddress)
        {
            return Json(PensionFundsServices.GetWithdrawalInfo(contractAddress));
        }

        [HttpPost]
        public IActionResult GeneratePayment(string contractAddress, int monthsAmount)
        {
            Progress progress = PensionFundsServices.GeneratePayment(contractAddress, monthsAmount);
            ReadPayments(contractAddress);
            return Json(progress);
        }

        [HttpPost]
        public void ReadPayments(string contractAddress)
        {
            Task.Factory.StartNew(() =>
            {
                var hubContext = HubConnectionManager.GetHubContext<AuctusDemoHub>();
                try
                {
                    Progress progress = PensionFundsServices.ReadPayments(contractAddress);
                    if (!progress.TransactionHistory.Any(c => !c.PaymentDate.HasValue))
                        hubContext.Clients.Client(ConnectionId).paymentsCompleted(Json(progress));
                    else
                        hubContext.Clients.Client(ConnectionId).paymentsUncompleted(Json(progress));
                }
                catch (Exception ex)
                {
                    Logger.LogError(new EventId(2), ex, string.Format("Erro on ReadPayments {0}.", contractAddress));
                    hubContext.Clients.Client(ConnectionId).readPaymentsError();
                }
            });
        }

        [HttpPost]
        public IActionResult GenerateWithdrawal(string contractAddress)
        {
            Withdrawal withdrawal = PensionFundsServices.GenerateWithdrawal(contractAddress);
            ReadWithdrawal(contractAddress);
            return Json(withdrawal);
        }

        [HttpPost]
        public void ReadWithdrawal(string contractAddress)
        {
            Task.Factory.StartNew(() =>
            {
                var hubContext = HubConnectionManager.GetHubContext<AuctusDemoHub>();
                try
                {
                    Withdrawal withdrawal = PensionFundsServices.ReadWithdrawal(contractAddress);
                    if (withdrawal.BlockNumber.HasValue)
                        hubContext.Clients.Client(ConnectionId).withdrawalCompleted(Json(withdrawal));
                    else
                        hubContext.Clients.Client(ConnectionId).withdrawalUncompleted(Json(withdrawal));
                }
                catch (Exception ex)
                {
                    Logger.LogError(new EventId(3), ex, string.Format("Erro on ReadWithdrawal {0}.", contractAddress));
                    hubContext.Clients.Client(ConnectionId).readWithdrawalError();
                }
            });
        }
    }
}
