using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using Auctus.DomainObjects.Funds;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Auctus.EthereumProxy;
using Microsoft.Extensions.Logging;

namespace Auctus.Business.Contracts
{
    public class PensionFundTransactionBusiness : BaseBusiness<PensionFundTransaction, PensionFundTransactionData>
    {
        public PensionFundTransactionBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        public List<BuyInfo> ReadCompletedPayments(string contractAddress)
        {
            string cacheKey = string.Format("Buy{0}", contractAddress);
            List<BuyInfo> cachedBuy = MemoryCache.Get<List<BuyInfo>>(cacheKey);
            if (cachedBuy != null && cachedBuy.Count == 120)
                return cachedBuy;

            PensionFund pensionFund = PensionFundBusiness.GetByContract(contractAddress);
            SmartContract smartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();
            List<PensionFundTransaction> transactions = Data.List(pensionFund.Option.PensionFundContract.TransactionHash);
            IEnumerable<PensionFundTransaction> pendingTransactions = transactions.Where(c => !c.BlockNumber.HasValue && 
                                                                        (c.FunctionType == FunctionType.EmployeeBuy || c.FunctionType == FunctionType.CompanyBuy));
            if (!pendingTransactions.Any() && cachedBuy != null && cachedBuy.Count == transactions.Count)
                return cachedBuy;

            List<BuyInfo> buyEvents = EthereumManager.ReadBuyFromDefaultPensionFund(contractAddress);
            HandleTransactions(smartContract, pensionFund, pendingTransactions, buyEvents.Select(c => new BaseEventInfo() { BlockNumber = c.BlockNumber, TransactionHash = c.TransactionHash }));
            if (buyEvents.Count > 0)
                MemoryCache.Set<List<BuyInfo>>(cacheKey, buyEvents);

            return buyEvents;
        }

        public WithdrawalInfo ReadCompletedWithdrawal(string contractAddress)
        {
            string cacheKey = string.Format("Withdrawal{0}", contractAddress);
            WithdrawalInfo cachedWithdrawal = MemoryCache.Get<WithdrawalInfo>(cacheKey);
            if (cachedWithdrawal != null)
                return cachedWithdrawal;

            PensionFund pensionFund = PensionFundBusiness.GetByContract(contractAddress);
            SmartContract smartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();
            List<PensionFundTransaction> transactions = Data.List(pensionFund.Option.PensionFundContract.TransactionHash);
            IEnumerable<PensionFundTransaction> pendingTransactions = transactions.Where(c => !c.BlockNumber.HasValue && c.FunctionType == FunctionType.CompleteWithdrawal);
 
            WithdrawalInfo withdrawalInfo = EthereumManager.ReadWithdrawalFromDefaultPensionFund(contractAddress);
            List<BaseEventInfo> baseInfo = new List<BaseEventInfo>();
            if (withdrawalInfo != null)
                baseInfo.Add(withdrawalInfo);

            HandleTransactions(smartContract, pensionFund, pendingTransactions, baseInfo);
            if (withdrawalInfo != null)
                MemoryCache.Set<WithdrawalInfo>(cacheKey, withdrawalInfo);

            return withdrawalInfo;
        }

        public List<string> GeneratePaymentContractTransaction(string contractAddress, int monthsAmount)
        {
            if (monthsAmount < 1 || monthsAmount > 60)
                throw new InvalidOperationException("Invalid months amount.");

            PensionFund pensionFund = PensionFundBusiness.GetByContract(contractAddress);
            SmartContract smartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();
            List<PensionFundTransaction> transactions = Data.List(pensionFund.Option.PensionFundContract.TransactionHash);

            int payments = transactions.Count(c => c.ContractFunctionId == FunctionType.CompanyBuy.Type || c.ContractFunctionId == FunctionType.EmployeeBuy.Type);
            if (payments == 120)
                throw new InvalidOperationException("All payments already been made.");
            else if (payments + (monthsAmount * 2) > 120)
                throw new InvalidOperationException("Too many payments.");
            else if (transactions.Any(c => c.FunctionType == FunctionType.CompleteWithdrawal))
                throw new InvalidOperationException("Withdrawal already made.");

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
            PensionFund pensionFund = PensionFundBusiness.GetByContract(contractAddress);
            SmartContract smartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();
            List<PensionFundTransaction> transactions = Data.List(pensionFund.Option.PensionFundContract.TransactionHash);
            
            if (transactions.Any(c => !c.BlockNumber.HasValue))
                throw new InvalidOperationException("Wait for unfinished transactions.");

            return GenerateContractTransaction(pensionFund.Option.PensionFundContract.TransactionHash, FunctionType.CompleteWithdrawal,
                        pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address, pensionFund.Option.Company.Employee.Address,
                        smartContract.ABI, smartContract.ContractFunctions.Single(c => c.FunctionType == FunctionType.CompleteWithdrawal).GasLimit, 0);
        }

        private void HandleTransactions(SmartContract smartContract, PensionFund pensionFund,
            IEnumerable<PensionFundTransaction> pendingTransactions, IEnumerable<BaseEventInfo> events)
        {
            IEnumerable<PensionFundTransaction> completed = pendingTransactions.Where(c => events.Any(k => k.TransactionHash == c.TransactionHash));
            DateTime tolerance = DateTime.UtcNow.AddMinutes(-2);
            if (!completed.Any() && pendingTransactions.Count() > completed.Count() && pendingTransactions.Any(c => c.CreationDate < tolerance))
            {
                PoolInfo poolInfo = GetPoolInfo();
                IEnumerable<PensionFundTransaction> lostTransactions = pendingTransactions.Where(c => c.CreationDate < tolerance &&
                    !poolInfo.Pending.Contains(c.TransactionHash) && !poolInfo.Queued.Contains(c.TransactionHash));
                foreach (PensionFundTransaction lost in lostTransactions)
                {
                    Logger.LogError(string.Format("Transaction {0} for contract {1} is lost.", lost.TransactionHash, pensionFund.Option.PensionFundContract.Address));
                    Delete(lost);
                    int gasLimit = smartContract.ContractFunctions.Single(c => c.FunctionType == lost.FunctionType).GasLimit;
                    GenerateContractTransaction(pensionFund.Option.PensionFundContract.TransactionHash, lost.FunctionType, pensionFund.Option.Company.Employee.Address, 
                        pensionFund.Option.PensionFundContract.Address, lost.WalletAddress, smartContract.ABI, gasLimit, 0);
                }
            }
            else
            {
                Parallel.ForEach(completed, new ParallelOptions() { MaxDegreeOfParallelism = 5 },
                    trans =>
                    {
                        trans.BlockNumber = events.Single(c => c.TransactionHash == trans.TransactionHash).BlockNumber;
                        Update(trans);
                    });
            }
        }

        private string GenerateContractTransaction(string pensionFundContractHash, FunctionType functionType, string employeeAddress,
            string contractAddress, string responsableAddress, string abi, int gasLimit, int daysOverdue)
        {
            string transactionHash;
            if (functionType == FunctionType.EmployeeBuy)
                transactionHash = EthereumManager.EmployeeBuyFromDefaultPensionFund(employeeAddress, contractAddress, abi, daysOverdue, gasLimit);
            else if (functionType == FunctionType.CompanyBuy)
                transactionHash = EthereumManager.EmployerBuyFromDefaultPensionFund(employeeAddress, contractAddress, abi, daysOverdue, gasLimit);
            else if (functionType == FunctionType.CompleteWithdrawal)
                transactionHash = EthereumManager.WithdrawalFromDefaultPensionFund(employeeAddress, contractAddress, abi, gasLimit);
            else
                throw new ArgumentException("Invalid function type for payment transaction.");

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
