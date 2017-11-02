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
    public class HomeController : BaseController
    {
        public HomeController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider) : base(loggerFactory, cache, serviceProvider) { }
        
        public IActionResult Index()
        {
            return View();
        }

        [Route("/Register")]
        public IActionResult StartDemo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(Wizard model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentException("Invalid data.");
                if (!IsValidRecaptcha(model.Captcha))
                    throw new InvalidOperationException("Invalid captcha.");

                int pensionFundId = PensionFundsServices.CreateUnprocessedEntry(model.Fund, model.Company, model.Employee);
                return Json(new { PensionFundId = pensionFundId }); 
            }
            catch(Exception e)
            {
                Response.StatusCode = 400;
                if (e is ArgumentException)
                    return Json(e.Message);
                return Json("We’re sorry, we had an unexpected error! Please try again in a minute.");
            }
        }
        
        //[HttpPost]
        //public IActionResult CheckPensionFundCreation(int pensionFundId)
        //{
        //    if (!string.IsNullOrEmpty(ConnectionId))
        //    {
        //        Task.Factory.StartNew(() =>
        //        {
        //            var hubContext = HubConnectionManager.GetHubContext<AuctusDemoHub>();
        //            try
        //            {
        //                var pensionFundCreated = PensionFundsServices.CheckPensionFundCreation(pensionFundId);
        //                if (pensionFundCreated != null)
        //                    hubContext.Clients.Client(ConnectionId).creationCompleted(Json(
        //                        new
        //                        {
        //                            SmartContractCode = pensionFundCreated.Item2,
        //                            TransactionHash = pensionFundCreated.Item1
        //                        }));
        //                else
        //                    hubContext.Clients.Client(ConnectionId).creationUncompleted(pensionFundId);
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.LogError(new EventId(1), ex, string.Format("Erro on CheckPensionFundCreation {0}.", pensionFundId));
        //                hubContext.Clients.Client(ConnectionId).creationUncompleted(pensionFundId);
        //            }
        //        });
        //        return Json(new { success = true });
        //    }
        //    else
        //        return Json(new { success = false });
        //}

        [HttpPost]
        public IActionResult CheckPensionFundCreation(int pensionFundId)
        {
            try
            {
                var pensionFundCreated = PensionFundsServices.CheckPensionFundCreation(pensionFundId);
                if (pensionFundCreated != null)
                    return Json(new { success = true, smartContractCode = pensionFundCreated.Item2, transactionHash = pensionFundCreated.Item1 });
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId(1), ex, string.Format("Erro on CheckPensionFundCreation {0}.", pensionFundId));
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult CheckContractMined(String transactionHash)
        {
            try
            {
                var pensionFundContract = PensionFundsServices.GetPensionFundContract(transactionHash);
                if (pensionFundContract == null)
                    throw new InvalidOperationException("Invalid transaction hash");

                if (pensionFundContract.BlockNumber.HasValue)
                    return Json(new { success = true, address = pensionFundContract.Address, blockNumber = pensionFundContract.BlockNumber, transactionHash = pensionFundContract.TransactionHash });
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId(1), ex, string.Format("Erro on CheckContractMined {0}.", transactionHash));
            }
            return Json(new { success = false });
        }

        //[HttpPost]
        //public IActionResult CheckContractMined(String transactionHash)
        //{
        //    if (!string.IsNullOrEmpty(ConnectionId))
        //    {
        //        Task.Factory.StartNew(() =>
        //        {
        //            var hubContext = HubConnectionManager.GetHubContext<AuctusDemoHub>();
        //            try
        //            {
        //                var pensionFundContract = PensionFundsServices.GetPensionFundContract(transactionHash);
        //                if (pensionFundContract == null)
        //                    throw new InvalidOperationException("Invalid transaction hash");

        //                if (pensionFundContract.BlockNumber.HasValue)
        //                    hubContext.Clients.Client(ConnectionId).deployCompleted(Json(
        //                        new
        //                        {
        //                            Address = pensionFundContract.Address,
        //                            BlockNumber = pensionFundContract.BlockNumber,
        //                            TransactionHash = pensionFundContract.TransactionHash
        //                        }));
        //                else
        //                    hubContext.Clients.Client(ConnectionId).deployUncompleted(pensionFundContract.TransactionHash);
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.LogError(new EventId(1), ex, string.Format("Erro on CheckContractMined {0}.", transactionHash));
        //                hubContext.Clients.Client(ConnectionId).deployError();
        //            }
        //        });
        //        return Json(new { success = true });
        //    }
        //    else
        //        return Json(new { success = false });
        //}
    }
}
