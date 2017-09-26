using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace WebApi.Controllers
{
    [Route("api/Presale")]
    public class PresaleController : Controller
    {
        [HttpPost("Check")]
        public bool Check(string address)
        {
            Stream fileStream = new FileStream(@"subscribed_members.csv", FileMode.Open);
            List<string> addresses = new List<string>();
            using (var reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (values.Length > 1 && !String.IsNullOrWhiteSpace(values[1]) 
                        && values[1].Substring(0, 2).Equals("0x")
                        && values[1].Length == 42)
                    {
                        addresses.Add(values[1]);
                    }
                }
            }

            return addresses.Contains(address);
        }
    }
}
