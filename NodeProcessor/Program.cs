using System;
using System.Threading.Tasks;

using System.IO;
using Microsoft.Extensions.Configuration;

namespace Auctus.NodeProcessor
{
    class Program
    {
        private static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            LoadConfigurations();

            var processor = new Processor(Convert.ToInt32(Configuration["NodeId"]));
            try
            {
                processor.Start();
            }

            catch(Exception ex)
            {
                //LOG Exception
            }
            
        }

        private static void LoadConfigurations()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

        }
    }
}