using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using Auctus.DomainObjects.Funds;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Auctus.Business.Contracts
{
    public class PensionFundTransactionBusiness : BaseBusiness<PensionFundTransaction, PensionFundTransactionData>
    {
        public PensionFundTransactionBusiness(Cache cache) : base(cache) { }

        public List<string> GeneratePaymentContractTransaction(string contractAddress, int monthsAmount)
        {
            if (monthsAmount < 1 || monthsAmount > 60)
                throw new Exception("Invalid months amount.");

            PensionFund pensionFund = PensionFundBusiness.Get(contractAddress);
            SmartContract smartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();
            List<PensionFundTransaction> transactions = Data.List(pensionFund.Option.PensionFundContract.TransactionHash);

            int payments = transactions.Count(c => c.ContractFunctionId == FunctionType.CompanyBuy.Type || c.ContractFunctionId == FunctionType.EmployeeBuy.Type);
            if (payments == 120)
                throw new Exception("All payments already been made.");
            else if (payments + (monthsAmount * 2) > 120)
                throw new Exception("Too many payments.");

            List<string> transactionsHash = new List<string>();
            Parallel.ForEach(Enumerable.Range(1, monthsAmount), new ParallelOptions() { MaxDegreeOfParallelism = 5 },
                l =>
                {
                    transactionsHash.Add(GenerateContractTransaction(pensionFund.Option.PensionFundContract.TransactionHash, FunctionType.EmployeeBuy,
                        pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address, pensionFund.Option.Company.Employee.Address,
                        smartContract.ABI, smartContract.ContractFunctions.Single(c => c.FunctionType == FunctionType.EmployeeBuy).GasLimit + l, 0));
                    transactionsHash.Add(GenerateContractTransaction(pensionFund.Option.PensionFundContract.TransactionHash, FunctionType.CompanyBuy,
                        pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address, pensionFund.Option.Company.Address,
                        smartContract.ABI, smartContract.ContractFunctions.Single(c => c.FunctionType == FunctionType.CompanyBuy).GasLimit + l, 0));
                });
            return transactionsHash;
        }

        public string GenerateWithdrawalContractTransaction(string contractAddress)
        {
            PensionFund pensionFund = PensionFundBusiness.Get(contractAddress);
            SmartContract smartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();
            List<PensionFundTransaction> transactions = Data.List(pensionFund.Option.PensionFundContract.TransactionHash);
            
            if (transactions.Any(c => !c.BlockNumber.HasValue))
                throw new Exception("Wait for unfinished payment transactions.");

            return GenerateContractTransaction(pensionFund.Option.PensionFundContract.TransactionHash, FunctionType.CompleteWithdrawal,
                        pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address, pensionFund.Option.Company.Employee.Address,
                        smartContract.ABI, smartContract.ContractFunctions.Single(c => c.FunctionType == FunctionType.CompleteWithdrawal).GasLimit, 0);
        }

        private string GenerateContractTransaction(string pensionFundContractHash, FunctionType functionType, string employeeAddress,
            string contractAddress, string responsableAddress, string abi, int gasLimit, int daysOverdue)
        {
            string transactionHash;
            if (functionType == FunctionType.EmployeeBuy)
                transactionHash = EthereumProxy.EthereumManager.EmployeeBuyFromDefaultPensionFund(employeeAddress, contractAddress, abi, daysOverdue, gasLimit);
            else if (functionType == FunctionType.CompanyBuy)
                transactionHash = EthereumProxy.EthereumManager.EmployerBuyFromDefaultPensionFund(employeeAddress, contractAddress, abi, daysOverdue, gasLimit);
            else if (functionType == FunctionType.CompleteWithdrawal)
                transactionHash = EthereumProxy.EthereumManager.WithdrawalFromDefaultPensionFund(employeeAddress, contractAddress, abi, gasLimit);
            else
                throw new Exception("Invalid function type for payment transaction.");

            Insert(new PensionFundTransaction()
            {
                TransactionHash = transactionHash,
                CreationDate = DateTime.UtcNow,
                ContractFunctionId = functionType.Type,
                PensionFundContractHash = pensionFundContractHash,
                WalletAddress = responsableAddress
            });
            return transactionHash;
        }
    }
}
