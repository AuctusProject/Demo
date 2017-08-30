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

        public IActionResult Save(Wizard model)
        {
            var service = new AccountsService();
            FundsServices.CreateCompleteEntry(model.Fund,model.Company,model.Employee,model.Contract);
            
            return StatusCode(200); 
        }
    }
}
