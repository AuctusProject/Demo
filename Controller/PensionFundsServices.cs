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
    public class PensionFundsServices : BaseServices
    {
        public PensionFundsServices(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        public PensionFundInfo GetPensionFundInfo(string contractAddress)
        {
            return PensionFundBusiness.GetPensionFundInfo(contractAddress);
        }

        public Withdrawal GetWithdrawalInfo(string contractAddress)
        {
            return PensionFundBusiness.GetWithdrawalInfo(contractAddress);
        }

        public PensionFundContract CreateCompleteEntry(Fund fund, Company company, Employee employee)
        {
            return PensionFundBusiness.CreateCompleteEntry(fund, company, employee);
        }

        public PensionFundContract CheckContractCreationTransaction(String transactionHash)
        {
            return PensionFundContractBusiness.CheckContractCreationTransaction(transactionHash);
        }

        public Progress GeneratePayment(string contractAddress, int monthsAmount)
        {
            return PensionFundTransactionBusiness.GeneratePayment(contractAddress, monthsAmount);
        }

        public Withdrawal GenerateWithdrawal(string contractAddress)
        {
            return PensionFundTransactionBusiness.GenerateWithdrawal(contractAddress);
        }

        public Progress ReadPayments(string contractAddress)
        {
            return PensionFundTransactionBusiness.ReadPayments(contractAddress);
        }

        public Withdrawal ReadWithdrawal(string contractAddress)
        {
            return PensionFundTransactionBusiness.ReadWithdrawal(contractAddress);
        }
    }
}
