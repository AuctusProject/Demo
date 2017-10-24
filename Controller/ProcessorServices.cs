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
        public void PostNotSentTransactions(int nodeId)
        {
            new PensionFundTransactionBusiness(null, null).PostNotSentTransactions(nodeId);
        }

        public void ReadPendingTransactions(int nodeId)
        {
            new PensionFundTransactionBusiness(null, null).ReadPendingTransactions(nodeId);
        }

        public void ProcessAutoRecoveryTransactions(int nodeId)
        {
            new PensionFundTransactionBusiness(null, null).ProcessAutoRecoveryTransactions(nodeId);
        }
    }
}
