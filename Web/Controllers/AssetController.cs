using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Auctus.Util;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Auctus.DomainObjects.Contracts;

namespace Web.Controllers
{
    public class AssetController : BaseController
    {
        public AssetController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider) : base (loggerFactory, cache, serviceProvider) { }

        [HttpGet]
        public IActionResult GetGoldReference()
        {
            //return GetJsonFromReferenceContract(ContractsServices.GetGoldReference());
            return null;
        }

        [HttpGet]
        public IActionResult GetMSCIWorldReference()
        {
            return GetJsonFromReferenceContract(ContractsServices.GetMSCIWorldReference());
        }

        [HttpGet]
        public IActionResult GetSP500Reference()
        {
            return GetJsonFromReferenceContract(ContractsServices.GetSP500Reference());
        }

        [HttpGet]
        public IActionResult GetVWEHXReference()
        {
            return GetJsonFromReferenceContract(ContractsServices.GetVWEHXReference());
        }

        [HttpGet]
        public IActionResult GetBitcoinReference()
        {
            return GetJsonFromReferenceContract(ContractsServices.GetBitcoinReference());
        }

        private JsonResult GetJsonFromReferenceContract(ReferenceContract referenceContract)
        {
            return Json(new { Address = referenceContract.Address, Name = referenceContract.Description,
                Values = referenceContract.ReferenceValue.Select(c => new double[]{ c.Period, c.Value}) });
        }
    }
}