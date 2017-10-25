using Auctus.Business.Contracts;
using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using Auctus.Util;
using Microsoft.Extensions.Logging;

namespace Auctus.Service
{
    public class ProcessorServices 
    {
        public void PostNotSentTransactions(int nodeId, Cache cache, ILoggerFactory logger)
        {
            new PensionFundTransactionBusiness(logger, cache).PostNotSentTransactions(nodeId);
        }

        public void ReadPendingTransactions(int nodeId, Cache cache, ILoggerFactory logger)
        {
            new PensionFundTransactionBusiness(logger, cache).ReadPendingTransactions(nodeId);
        }

        public void ProcessAutoRecoveryTransactions(int nodeId, Cache cache, ILoggerFactory logger)
        {
            new PensionFundTransactionBusiness(logger, cache).ProcessAutoRecoveryTransactions(nodeId);
        }
    }
}
