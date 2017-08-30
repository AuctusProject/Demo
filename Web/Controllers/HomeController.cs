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

namespace Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IMemoryCache memoryCache, ILoggerFactory loggerFactory, IServiceProvider serviceProvider) : base (memoryCache, loggerFactory, serviceProvider)
        { }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Save()//Wizard model)
        {
            var pensionFundContract = new FundsServices().CreateCompleteEntry(new Fund()
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
            return Json(pensionFundContract); 
        }
    }
}
