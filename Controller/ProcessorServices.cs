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
    }
}
