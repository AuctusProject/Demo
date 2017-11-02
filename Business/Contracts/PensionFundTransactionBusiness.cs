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
using System.Threading;
using Auctus.Model;
using Microsoft.Extensions.Configuration;

namespace Auctus.Business.Contracts
{
    public class PensionFundTransactionBusiness : BaseBusiness<PensionFundTransaction, PensionFundTransactionData>
    {
        public const double BLOCKCHAIN_TRANSACTION_TOLERANCE = 3;
        public const double CREATION_TRANSACTION_TOLERANCE = 2.5;
        public const double CREATION_TOLERANCE = 0.5;
        private readonly IConfigurationRoot configuration;
        private int MaxParallelism
        {
            get
            {
                if (configuration != null && configuration["MaxParallelism"] != null)
                    return Convert.ToInt32(configuration["MaxParallelism"]);
                return 1;
            }
        }

        private int NodeId
        {
            get
            {
                if (configuration != null && configuration["NodeId"] != null)
                    return Convert.ToInt32(configuration["NodeId"]);
                return 0;
            }
        }
        public PensionFundTransactionBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        public PensionFundTransactionBusiness(ILoggerFactory loggerFactory, Cache cache, IConfigurationRoot configuration) : this(loggerFactory, cache)
        {
            this.configuration = configuration;
        }

        public Withdrawal ReadWithdrawal(string contractAddress)
        {
            string cacheKey = string.Format("Withdrawal{0}", contractAddress);
            Withdrawal cachedWithdrawal = MemoryCache.Get<Withdrawal>(cacheKey);
            if (cachedWithdrawal != null)
                return cachedWithdrawal;

            PensionFund pensionFund = PensionFundBusiness.GetByContract(contractAddress);
            PensionFundTransaction transaction = Data.List(pensionFund.Option.PensionFundContract.TransactionHash).SingleOrDefault(c => c.FunctionType == FunctionType.CompleteWithdrawal);
            if (transaction == null)
                return null;

            var withdrawalEvent = WithdrawalEventBusiness.Get(pensionFund.Option.PensionFundContract.TransactionHash);
            Withdrawal withdrawal;
            if (withdrawalEvent != null)
            {
                withdrawal = new Withdrawal()
                {
                    TransactionHash = transaction.TransactionHash,
                    CreatedDate = transaction.CreationDate,
                    Responsable = transaction.WalletAddress,
                    BlockNumber = transaction.BlockNumber,
                    Period = withdrawalEvent.Period,
                    EmployeeAbsoluteBonus = withdrawalEvent.EmployeeAbsoluteBonus,
                    EmployeeBonus = withdrawalEvent.EmployeeBonus,
                    EmployeeSzaboCashback = withdrawalEvent.EmployeeSzaboCashback,
                    EmployeeTokenCashback = withdrawalEvent.EmployeeTokenCashback,
                    EmployerSzaboCashback = withdrawalEvent.EmployerSzaboCashback,
                    EmployerTokenCashback = withdrawalEvent.EmployerTokenCashback,
                    ReferenceDate = pensionFund.Option.PensionFundContract.CreationDate.AddMonths(withdrawalEvent.Period).Date
                };
                MemoryCache.Set<Withdrawal>(cacheKey, withdrawal);
            }
            else
            {
                withdrawal = new Withdrawal()
                {
                    TransactionHash = transaction.TransactionHash,
                    CreatedDate = transaction.CreationDate,
                    Responsable = transaction.WalletAddress,
                };
            }
            return withdrawal;
        }

        public Progress GeneratePayment(string contractAddress, int monthsAmount)
        {
            if (monthsAmount < 1 || monthsAmount > 60)
                throw new InvalidOperationException("Invalid months amount.");

            PensionFund pensionFund = PensionFundBusiness.GetByContract(contractAddress);
            List<PensionFundTransaction> transactions = Data.List(pensionFund.Option.PensionFundContract.TransactionHash);

            int payments = transactions.Count(c => c.ContractFunctionId == FunctionType.CompanyBuy.Type || c.ContractFunctionId == FunctionType.EmployeeBuy.Type);
            if (payments == 120)
                throw new InvalidOperationException("All payments already been made.");
            else if (payments + (monthsAmount * 2) > 120)
                throw new InvalidOperationException("Too many payments.");
            else if (transactions.Any(c => c.FunctionType == FunctionType.CompleteWithdrawal))
                throw new InvalidOperationException("Withdrawal already made.");

            List<PensionFundTransaction> newTransactions = new List<PensionFundTransaction>();
            Parallel.For(1, monthsAmount + 1, new ParallelOptions() { MaxDegreeOfParallelism = 5 },
            month =>
            {
                DateTime date = DateTime.UtcNow;
                newTransactions.Add(CreateTransaction(date, FunctionType.EmployeeBuy, pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.TransactionHash));
                newTransactions.Add(CreateTransaction(date, FunctionType.CompanyBuy, pensionFund.Option.Company.Address, pensionFund.Option.PensionFundContract.TransactionHash));
            });

            IEnumerable<Payment> newPayments = newTransactions.Select(c => new Payment()
            {
                CreatedDate = c.CreationDate,
                Responsable = c.WalletAddress
            });

            List<Payment> cachedPayment = MemoryCache.Get<List<Payment>>(GetCachePaymentKey(contractAddress));
            if (cachedPayment != null)
            {
                return PensionFundBusiness.GetProgress(pensionFund, newPayments.Concat(cachedPayment));
            }
            else
                return PensionFundBusiness.GetProgress(pensionFund, newPayments.ToList());
        }

        public Withdrawal GenerateWithdrawal(string contractAddress)
        {
            PensionFund pensionFund = PensionFundBusiness.GetByContract(contractAddress);
            List<PensionFundTransaction> transactions = Data.List(pensionFund.Option.PensionFundContract.TransactionHash);

            if (transactions.Any(c => !c.BlockNumber.HasValue))
                throw new InvalidOperationException("Wait for unfinished transactions.");
            else if (transactions.Any(c => c.FunctionType == FunctionType.CompleteWithdrawal))
                throw new InvalidOperationException("Withdrawal already made.");

            PensionFundTransaction withdrawalTransaction = CreateTransaction(DateTime.UtcNow, FunctionType.CompleteWithdrawal,
                pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.TransactionHash);

            return new Withdrawal()
            {
                TransactionHash = withdrawalTransaction.TransactionHash,
                CreatedDate = withdrawalTransaction.CreationDate,
                Responsable = withdrawalTransaction.WalletAddress
            };
        }

        private string GetCachePaymentKey(string contractAddress)
        {
            return string.Format("PaymentCompleted{0}", contractAddress);
        }

        private void GenerateContractTransaction(PensionFundTransaction pensionFundTransaction, string employeeAddress,
            string contractAddress, string abi, int gasLimit, int daysOverdue)
        {
            if (pensionFundTransaction.FunctionType == FunctionType.EmployeeBuy)
                pensionFundTransaction.TransactionHash = EthereumManager.EmployeeBuyFromDefaultPensionFund(employeeAddress, contractAddress, abi, daysOverdue, gasLimit);
            else if (pensionFundTransaction.FunctionType == FunctionType.CompanyBuy)
                pensionFundTransaction.TransactionHash = EthereumManager.EmployerBuyFromDefaultPensionFund(employeeAddress, contractAddress, abi, daysOverdue, gasLimit);
            else if (pensionFundTransaction.FunctionType == FunctionType.CompleteWithdrawal)
                pensionFundTransaction.TransactionHash = EthereumManager.WithdrawalFromDefaultPensionFund(employeeAddress, contractAddress, abi, gasLimit);
            else
                throw new ArgumentException("Invalid function type for payment transaction.");

            pensionFundTransaction.TransactionStatus = TransactionStatus.Pending;

            Update(pensionFundTransaction);
        }

        private PensionFundTransaction CreateTransaction(DateTime date, FunctionType functionType, string responsableAddress,
            string pensionFundContractHash, string transactionHash = null, int? blockNumber = null)
        {
            var transactionStatus = blockNumber.HasValue ?
                TransactionStatus.Completed : (!String.IsNullOrWhiteSpace(transactionHash) ? TransactionStatus.Pending : TransactionStatus.NotSent);

            PensionFundTransaction pensionFundTransaction = new PensionFundTransaction()
            {
                CreationDate = date,
                ContractFunctionId = functionType.Type,
                PensionFundContractHash = pensionFundContractHash,
                WalletAddress = responsableAddress,
                BlockNumber = blockNumber,
                TransactionHash = transactionHash,
                TransactionStatus = transactionStatus,
                NodeProcessorId = 0
            };
            Insert(pensionFundTransaction);
            return pensionFundTransaction;
        }

        public Progress ReadPayments(string contractAddress)
        {
            PensionFund pensionFund = PensionFundBusiness.GetByContract(contractAddress);

            string cacheKey = GetCachePaymentKey(contractAddress);
            List<Payment> cachedPayment = MemoryCache.Get<List<Payment>>(cacheKey);
            if (cachedPayment != null && cachedPayment.Count == 120)
                return PensionFundBusiness.GetProgress(pensionFund, cachedPayment);

            IEnumerable<PensionFundTransaction> transactions = Data.List(pensionFund.Option.PensionFundContract.TransactionHash).
                                                    Where(c => c.FunctionType == FunctionType.EmployeeBuy || c.FunctionType == FunctionType.CompanyBuy);

            if (!transactions.Any())
                return PensionFundBusiness.GetProgress(pensionFund, new List<Payment>());
            else if (cachedPayment != null && cachedPayment.Count == transactions.Count())
                return PensionFundBusiness.GetProgress(pensionFund, cachedPayment);

            List<BuyEvent> buyEvents = BuyEventBusiness.List(pensionFund.Option.PensionFundContract.TransactionHash);

            List<Payment> completedPayments = new List<Payment>();
            List<Payment> payments = new List<Payment>();
            foreach (PensionFundTransaction trans in transactions)
            {
                var buyEvent = buyEvents.SingleOrDefault(c => c.PensionFundTransactionId == trans.Id);
                var payment = new Payment()
                {
                    TransactionHash = trans.TransactionHash,
                    CreatedDate = trans.CreationDate,
                    Responsable = trans.WalletAddress
                };
                if (buyEvent != null)
                {
                    payment.BlockNumber = trans.BlockNumber;
                    payment.Period = buyEvent.Period;
                    payment.AuctusFee = buyEvent.AuctusFee;
                    payment.LatePenalty = buyEvent.LatePenalty;
                    payment.PensionFundFee = buyEvent.PensionFundFee;
                    payment.SzaboInvested = buyEvent.SzaboInvested;
                    payment.TokenAmount = buyEvent.TokenAmount;
                    payment.DaysOverdue = buyEvent.DaysOverdue;
                    payment.ReferenceDate = pensionFund.Option.PensionFundContract.CreationDate.AddMonths(buyEvent.Period).Date;
                    completedPayments.Add(payment);
                }

                payments.Add(payment);
            }
            if (completedPayments.Count > 0)
                MemoryCache.Set<List<Payment>>(cacheKey, completedPayments);

            return PensionFundBusiness.GetProgress(pensionFund, payments);
        }

        private Dictionary<string, List<PensionFundTransaction>> ListTransactionsGroupByContractAddress(TransactionStatus transactionStatus)
        {
            var pendingTransactions = Data.ListForProcessing(transactionStatus, NodeId);
            var transactionsByContract = pendingTransactions.GroupBy(p => p.PensionFundContractAddress).
                ToDictionary(p => p.Key, p => p.ToList());
            return transactionsByContract;
        }

        public void PostNotSentTransactions()
        {
            Dictionary<string, List<PensionFundTransaction>> transactionsByContract = ListTransactionsGroupByContractAddress(TransactionStatus.NotSent);

            foreach (var pensionFundContractHash in transactionsByContract.Keys)
            {
                PensionFund pensionFund = PensionFundBusiness.GetByContract(pensionFundContractHash);
                SmartContract smartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();

                var contractNotSentTransactions = transactionsByContract[pensionFundContractHash];

                Parallel.ForEach(contractNotSentTransactions, new ParallelOptions() { MaxDegreeOfParallelism = MaxParallelism}, (contractNotSentTransaction) =>
                {
                    GenerateContractTransaction(contractNotSentTransaction, pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address,
                        smartContract.ABI, smartContract.ContractFunctions.Single(c => c.FunctionType == contractNotSentTransaction.FunctionType).GasLimit, 0);
                });
            }
        }

        public void ReadPendingTransactions()
        {
            Dictionary<string, List<PensionFundTransaction>> transactionsByContract = ListTransactionsGroupByContractAddress(TransactionStatus.Pending);
            PoolInfo poolInfo = null;
            foreach (var pensionFundContractAddress in transactionsByContract.Keys)
            {
                List<BuyInfo> buyEvents = EthereumManager.ReadBuyFromDefaultPensionFund(pensionFundContractAddress);
                var contractPendingTransactions = transactionsByContract[pensionFundContractAddress];
                Parallel.ForEach(contractPendingTransactions, new ParallelOptions() { MaxDegreeOfParallelism = MaxParallelism }, (contractPendingTransaction) =>
                {
                    BaseEventInfo baseInfo = GetEventInfo(pensionFundContractAddress, buyEvents, contractPendingTransaction);

                    if (baseInfo != null)
                    {
                        SaveEventInfo(contractPendingTransaction, baseInfo);
                        contractPendingTransaction.BlockNumber = baseInfo.BlockNumber;
                        contractPendingTransaction.TransactionStatus = TransactionStatus.Completed;
                        Update(contractPendingTransaction);
                    }
                    else
                    {
                        poolInfo = SendToAutoRecoveryIfTransactionIsLost(poolInfo, contractPendingTransaction);
                    }
                });

                SaveWithdrawalValuesIfAllTransactionsCompleted(contractPendingTransactions);
            }
        }

        private void SaveWithdrawalValuesIfAllTransactionsCompleted(IEnumerable<PensionFundTransaction> contractPendingTransactions)
        {
            if(contractPendingTransactions.All(contract => contract.TransactionStatus == TransactionStatus.Completed))
            {
                PensionFundContractBusiness.UpdateWithdrawalCashbackWithSmartContractValues(contractPendingTransactions.First().PensionFundContractAddress);
            }
        }

        private PoolInfo SendToAutoRecoveryIfTransactionIsLost(PoolInfo poolInfo, PensionFundTransaction contractPendingTransaction)
        {
            DateTime chainTolerance = DateTime.UtcNow.AddMinutes(-BLOCKCHAIN_TRANSACTION_TOLERANCE);
            if (contractPendingTransaction.CreationDate < chainTolerance)
            {
                if (poolInfo == null)
                    poolInfo = GetPoolInfo();
                if (!poolInfo.Pending.Contains(contractPendingTransaction.TransactionHash))
                {
                    contractPendingTransaction.TransactionStatus = TransactionStatus.AutoRecovery;
                    Update(contractPendingTransaction);
                }
            }

            return poolInfo;
        }

        private void SaveEventInfo(PensionFundTransaction contractPendingTransaction, BaseEventInfo baseInfo)
        {
            if (baseInfo is WithdrawalInfo)
            {
                WithdrawalEventBusiness.Save(contractPendingTransaction.Id, baseInfo as WithdrawalInfo);
            }
            else
            {
                BuyEventBusiness.Save(contractPendingTransaction.Id, baseInfo as BuyInfo);
            }
        }

        private static BaseEventInfo GetEventInfo(string pensionFundContractHash, IEnumerable<BuyInfo> buyEvents, PensionFundTransaction contractPendingTransaction)
        {
            BaseEventInfo baseInfo;
            if (contractPendingTransaction.FunctionType == FunctionType.CompleteWithdrawal)
            {
                baseInfo = EthereumManager.ReadWithdrawalFromDefaultPensionFund(pensionFundContractHash);
            }
            else
            {
                baseInfo = buyEvents.SingleOrDefault(c => c.TransactionHash == contractPendingTransaction.TransactionHash);
            }

            return baseInfo;
        }

        public void ProcessAutoRecoveryTransactions()
        {
            Dictionary<string, List<PensionFundTransaction>> transactionsByContract = ListTransactionsGroupByContractAddress(TransactionStatus.AutoRecovery);

            foreach (var pensionFundContractAddress in transactionsByContract.Keys)
            {
                PensionFund pensionFund = PensionFundBusiness.GetByContract(pensionFundContractAddress);
                var contractPendingTransactions = transactionsByContract[pensionFundContractAddress];

                Parallel.ForEach(contractPendingTransactions, new ParallelOptions() { MaxDegreeOfParallelism = MaxParallelism }, (lostTransaction) =>
                {
                    Logger.LogError(string.Format("Transaction {0} for contract {1} is lost.", lostTransaction.TransactionHash, pensionFund.Option.PensionFundContract.Address));
                    CreateTransaction(DateTime.UtcNow, lostTransaction.FunctionType, lostTransaction.WalletAddress, pensionFund.Option.PensionFundContract.TransactionHash);
                    Delete(lostTransaction);
                });
            }
        }
    }
}
