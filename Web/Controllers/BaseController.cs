using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Auctus.Util;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Auctus.Service;
using System.Net.Http;
using Auctus.Util.NotShared;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Auctus.Web.Model.Home;

namespace Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;
        protected readonly Cache MemoryCache;

        protected BaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider)
        {
            MemoryCache = cache;
            LoggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        protected bool IsValidRecaptcha(string recaptchaResponse)
        {
            string url = string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", Config.GOOGLE_CAPTCHA_SECRET, recaptchaResponse);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                using (HttpResponseMessage response = client.PostAsync(url, null).Result)
                {
                    RecaptchaResponse result = JsonConvert.DeserializeObject<RecaptchaResponse>(response.Content.ReadAsStringAsync().Result);
                    return result != null && result.Success;
                }
            }
        }

        protected PensionFundsServices PensionFundsServices { get { return new PensionFundsServices(LoggerFactory, MemoryCache); } }
        protected ContractsServices ContractsServices { get { return new ContractsServices(LoggerFactory, MemoryCache); } }
    }
}