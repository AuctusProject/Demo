using Auctus.Business.Funds;
using Auctus.DomainObjects.Contracts;
using Auctus.DomainObjects.Funds;
using Auctus.EthereumProxy;
using Auctus.Model;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public class FundsServices : BaseServices
    {
        public FundsServices(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        public PensionFundContract CreateCompleteEntry(Fund fund, Company company, Employee employee)
        {
            return PensionFundBusiness.CreateCompleteEntry(fund, company, employee);
        }

        public PensionFundContract CheckContractCreationTransaction(String transactionHash)
        {
            return PensionFundContractBusiness.CheckContractCreationTransaction(transactionHash);
        }
    }
}
