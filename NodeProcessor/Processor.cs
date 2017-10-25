using Auctus.Service;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.NodeProcessor
{
    public class Processor
    {
        private int NodeId;
        private Cache cache;
        private ILoggerFactory loggerFactory;

        public Processor(ILoggerFactory loggerFactory, IConfigurationRoot configuration, Cache cache)
        {
            NodeId = Convert.ToInt32(configuration["NodeId"]);
            this.cache = cache;
            this.loggerFactory = loggerFactory;
        }

        internal void Start()
        {
            while (true)
            {
                new ProcessorServices().PostNotSentTransactions(NodeId, cache, loggerFactory);
                //TODO:update pendings
                

                //TODO: improve database polling
                Task.Delay(2000);
            }
        }
    }
}
