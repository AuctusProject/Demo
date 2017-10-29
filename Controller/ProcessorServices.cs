using Auctus.Business.Contracts;
using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Auctus.Business.Funds;

namespace Auctus.Service
{
    public class ProcessorServices 
    {
        public void PostNotSentTransactions(Cache cache, ILoggerFactory logger, IConfigurationRoot configuration)
        {
            new PensionFundTransactionBusiness(logger, cache, configuration).PostNotSentTransactions();
        }

        public void ReadPendingTransactions(Cache cache, ILoggerFactory logger, IConfigurationRoot configuration)
        {
            new PensionFundTransactionBusiness(logger, cache, configuration).ReadPendingTransactions();
        }

        public void ProcessAutoRecoveryTransactions(Cache cache, ILoggerFactory logger, IConfigurationRoot configuration)
        {
            new PensionFundTransactionBusiness(logger, cache, configuration).ProcessAutoRecoveryTransactions();
        }

        public void ProcessPensionFundsEntries(Cache cache, ILoggerFactory logger, IConfigurationRoot configuration)
        {
            new PensionFundBusiness(logger, cache).ProcessPensionFundsEntries();
        }

        public void ReadContractMined(Cache cache, ILoggerFactory logger, IConfigurationRoot configuration)
        {
            new PensionFundContractBusiness(logger, cache).ReadContractMined();
        }
    }
}
