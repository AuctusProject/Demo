using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Contracts
{
    public class PensionFundTransactionBusiness : BaseBusiness<PensionFundTransaction, PensionFundTransactionData>
    {
        public PensionFundTransactionBusiness(Cache cache) : base(cache) { }

        //public string GeneratePaymentContractTransaction(int pensionFundContractId, int monthsAmount)
        //{

        //}

        //public string GeneratePaymentContractTransaction(int pensionFundContractId, FunctionType functionType, string employeeAddress, 
        //    string contractAddress, string abi, int gasLimit, int daysOverdue)
        //{
        //    string transactionHash;
        //    if (functionType == FunctionType.EmployeeBuy)
        //        transactionHash = EthereumProxy.EthereumManager.EmployeeBuyFromDefaultPensionFund(employeeAddress, contractAddress, abi, daysOverdue, gasLimit);
        //    else if (functionType == FunctionType.CompanyBuy)
        //        transactionHash = EthereumProxy.EthereumManager.EmployerBuyFromDefaultPensionFund(employeeAddress, contractAddress, abi, daysOverdue, gasLimit);
        //    else
        //        throw new Exception("Invalid function type for payment transaction.");

        //    Insert(new PensionFundTransaction()
        //    {
        //        TransactionHash = transactionHash,
        //        CreationDate = DateTime.UtcNow,
        //        ContractFunctionId = functionType.Type,
        //        PensionFundContractId = pensionFundContractId,
        //        WalletAddress = em
        //    });
        //}
    }
}
