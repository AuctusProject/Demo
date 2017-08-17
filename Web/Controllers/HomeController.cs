﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Auctus.Service;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {

            var service = new AccountsService();
            new AccountsService().createCompany("teste" + DateTime.Now.Ticks);
            var list = service.listAllCompanies();



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
    }
}
