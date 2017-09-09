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
using Auctus.Model;
using System.Threading;

namespace Auctus.Business.Contracts
{
    public class PensionFundTransactionBusiness : BaseBusiness<PensionFundTransaction, PensionFundTransactionData>
    {
        public PensionFundTransactionBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }
        
        public Progress ReadPayments(string contractAddress)
        {
            PensionFund pensionFund = PensionFundBusiness.GetByContract(contractAddress);

            string cacheKey = GetCachePaymentKey(contractAddress);
            List<Payment> cachedPayment = MemoryCache.Get<List<Payment>>(cacheKey);
            if (cachedPayment != null && cachedPayment.Count == 120)
                return PensionFundBusiness.GetProgress(pensionFund, cachedPayment);
            
            SmartContract smartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();
            IEnumerable<PensionFundTransaction> transactions = Data.List(pensionFund.Option.PensionFundContract.TransactionHash).
                                                    Where(c => c.FunctionType == FunctionType.EmployeeBuy || c.FunctionType == FunctionType.CompanyBuy);
            IEnumerable<PensionFundTransaction> pendingTransactions = transactions.Where(c => !c.BlockNumber.HasValue && !string.IsNullOrEmpty(c.TransactionHash));

            if (!transactions.Any())
                return PensionFundBusiness.GetProgress(pensionFund, new List<Payment>());
            if (!pendingTransactions.Any())
            {
                if (cachedPayment == null && !transactions.Any(c => c.BlockNumber.HasValue))
                {
                    return PensionFundBusiness.GetProgress(pensionFund, transactions.Select(c => new Payment()
                    {
                        TransactionHash = c.TransactionHash,
                        CreatedDate = c.CreationDate,
                        Responsable = c.WalletAddress
                    }).ToList());
                }
                else if (cachedPayment != null && cachedPayment.Count == transactions.Count())
                    return PensionFundBusiness.GetProgress(pensionFund, cachedPayment);
                else if (cachedPayment != null && cachedPayment.Count == transactions.Count(c => !string.IsNullOrEmpty(c.TransactionHash)))
                {
                    cachedPayment.AddRange(transactions.Where(c => string.IsNullOrEmpty(c.TransactionHash)).Select(c => new Payment()
                    {
                        CreatedDate = c.CreationDate,
                        Responsable = c.WalletAddress
                    }));
                    return PensionFundBusiness.GetProgress(pensionFund, cachedPayment);
                }
            }

            List<BuyInfo> buyEvents = EthereumManager.ReadBuyFromDefaultPensionFund(contractAddress);
            HandleTransactions(smartContract, pensionFund, pendingTransactions, buyEvents.Select(c => new BaseEventInfo() { BlockNumber = c.BlockNumber, TransactionHash = c.TransactionHash }));

            IEnumerable<Payment> payments = transactions.Where(c => !c.BlockNumber.HasValue).Select(c => new Payment()
            {
                TransactionHash = c.TransactionHash,
                CreatedDate = c.CreationDate,
                Responsable = c.WalletAddress
            });
            List<Payment> completedPayments = new List<Payment>();
            if (buyEvents.Count > 0)
            {
                foreach (PensionFundTransaction trans in transactions)
                {
                    BuyInfo buyInfo = buyEvents.SingleOrDefault(c => c.TransactionHash == trans.TransactionHash);
                    if (buyInfo != null)
                    {
                        completedPayments.Add(new Payment()
                        {
                            TransactionHash = trans.TransactionHash,
                            CreatedDate = trans.CreationDate,
                            Responsable = trans.WalletAddress,
                            BlockNumber = buyInfo.BlockNumber,
                            Period = buyInfo.Period,
                            AuctusFee = buyInfo.AuctusFee,
                            LatePenalty = buyInfo.LatePenalty,
                            PensionFundFee = buyInfo.PensionFundFee,
                            SzaboInvested = buyInfo.SzaboInvested,
                            TokenAmount = buyInfo.TokenAmount,
                            DaysOverdue = buyInfo.DaysOverdue,
                            ReferenceDate = pensionFund.Option.PensionFundContract.CreationDate.AddMonths(buyInfo.Period).Date
                        });
                    }
                }
                if (completedPayments.Count > 0)
                    MemoryCache.Set<List<Payment>>(cacheKey, completedPayments);
            }
            completedPayments.AddRange(payments);
            return PensionFundBusiness.GetProgress(pensionFund, completedPayments);
        }

        public Withdrawal ReadWithdrawal(string contractAddress)
        {
            string cacheKey = string.Format("Withdrawal{0}", contractAddress);
            Withdrawal cachedWithdrawal = MemoryCache.Get<Withdrawal>(cacheKey);
            if (cachedWithdrawal != null)
                return cachedWithdrawal;

            PensionFund pensionFund = PensionFundBusiness.GetByContract(contractAddress);
            SmartContract smartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();
            PensionFundTransaction transaction = Data.List(pensionFund.Option.PensionFundContract.TransactionHash).Where(c => c.FunctionType == FunctionType.CompleteWithdrawal).SingleOrDefault();
            if (transaction == null)
                return null;

            WithdrawalInfo withdrawalInfo = EthereumManager.ReadWithdrawalFromDefaultPensionFund(contractAddress);
            List<BaseEventInfo> baseInfo = new List<BaseEventInfo>();
            if (withdrawalInfo != null)
                baseInfo.Add(withdrawalInfo);

            List<PensionFundTransaction> pending = new List<PensionFundTransaction>();
            if (!transaction.BlockNumber.HasValue)
                pending.Add(transaction);

            HandleTransactions(smartContract, pensionFund, pending, baseInfo);
            Withdrawal withdrawal = null;
            if (withdrawalInfo != null)
            {
                withdrawal = new Withdrawal()
                {
                    TransactionHash = transaction.TransactionHash,
                    CreatedDate = transaction.CreationDate,
                    Responsable = transaction.WalletAddress,
                    BlockNumber = withdrawalInfo.BlockNumber,
                    Period = withdrawalInfo.Period,
                    EmployeeAbsoluteBonus = withdrawalInfo.EmployeeAbsoluteBonus,
                    EmployeeBonus = withdrawalInfo.EmployeeBonus,
                    EmployeeSzaboCashback = withdrawalInfo.EmployeeSzaboCashback,
                    EmployeeTokenCashback = withdrawalInfo.EmployeeTokenCashback,
                    EmployerSzaboCashback = withdrawalInfo.EmployerSzaboCashback,
                    EmployerTokenCashback = withdrawalInfo.EmployerTokenCashback,
                    ReferenceDate = pensionFund.Option.PensionFundContract.CreationDate.AddMonths(withdrawalInfo.Period).Date
                };
                MemoryCache.Set<Withdrawal>(cacheKey, withdrawal);
            }
            else
            {
                withdrawal = new Withdrawal()
                {
                    TransactionHash = transaction.TransactionHash,
                    CreatedDate = transaction.CreationDate,
                    Responsable = transaction.WalletAddress
                };
            }
            return withdrawal;
        }

        public Progress GeneratePayment(string contractAddress, int monthsAmount)
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

            List<PensionFundTransaction> newTransactions = new List<PensionFundTransaction>();
            foreach (int month in Enumerable.Range(1, monthsAmount))
            {
                DateTime date = DateTime.UtcNow;
                newTransactions.Add(CreateTransaction(date, FunctionType.EmployeeBuy, pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.TransactionHash));
                newTransactions.Add(CreateTransaction(date, FunctionType.CompanyBuy, pensionFund.Option.Company.Address, pensionFund.Option.PensionFundContract.TransactionHash));
            }

            GeneratePaymentContractTransaction(newTransactions, pensionFund, smartContract);

            IEnumerable<Payment> newPayments = newTransactions.Select(c => new Payment()
            {
                CreatedDate = c.CreationDate,
                Responsable = c.WalletAddress
            });
            if (payments == 0)
                return PensionFundBusiness.GetProgress(pensionFund, newPayments.ToList());
            else
            {
                List<Payment> cachedPayment = MemoryCache.Get<List<Payment>>(GetCachePaymentKey(contractAddress));
                if (cachedPayment != null && cachedPayment.Count == payments)
                {
                    cachedPayment.AddRange(newPayments);
                    return PensionFundBusiness.GetProgress(pensionFund, cachedPayment);
                }
                else
                    return ReadPayments(contractAddress);
            }
        }
        
        public Withdrawal GenerateWithdrawal(string contractAddress)
        {
            PensionFund pensionFund = PensionFundBusiness.GetByContract(contractAddress);
            SmartContract smartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();
            List<PensionFundTransaction> transactions = Data.List(pensionFund.Option.PensionFundContract.TransactionHash);
            
            if (transactions.Any(c => !c.BlockNumber.HasValue))
                throw new InvalidOperationException("Wait for unfinished transactions.");
            else if (transactions.Any(c => c.FunctionType == FunctionType.CompleteWithdrawal))
                throw new InvalidOperationException("Withdrawal already made.");

            PensionFundTransaction withdrawalTransaction = CreateTransaction(DateTime.UtcNow, FunctionType.CompleteWithdrawal, 
                pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.TransactionHash);
            withdrawalTransaction = GenerateContractTransaction(withdrawalTransaction, pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address, 
                smartContract.ABI, smartContract.ContractFunctions.Single(c => c.FunctionType == FunctionType.CompleteWithdrawal).GasLimit, 0);
            return new Withdrawal()
            {
                TransactionHash = withdrawalTransaction.TransactionHash,
                CreatedDate = withdrawalTransaction.CreationDate,
                Responsable = withdrawalTransaction.WalletAddress
            };
        }

        private void HandleTransactions(SmartContract smartContract, PensionFund pensionFund, IEnumerable<PensionFundTransaction> pendingTransactions, IEnumerable<BaseEventInfo> events)
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
                    PensionFundTransaction newTransaction = CreateTransaction(DateTime.UtcNow, lost.FunctionType, lost.WalletAddress, pensionFund.Option.PensionFundContract.TransactionHash);
                    GenerateContractTransaction(newTransaction, pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address, 
                        smartContract.ABI, smartContract.ContractFunctions.Single(c => c.FunctionType == newTransaction.FunctionType).GasLimit, 0);
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

        private string GetCachePaymentKey(string contractAddress)
        {
            return string.Format("PaymentCompleted{0}", contractAddress);
        }

        private void GeneratePaymentContractTransaction(IEnumerable<PensionFundTransaction> transactions, PensionFund pensionFund, SmartContract smartContract)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    int index = 0;
                    Mutex mutex = new Mutex(false);
                    Parallel.ForEach(transactions, new ParallelOptions() { MaxDegreeOfParallelism = 5 },
                    trans =>
                    {
                        try
                        {
                            index++;
                            GenerateContractTransaction(trans, pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address,
                                smartContract.ABI, smartContract.ContractFunctions.Single(c => c.FunctionType == trans.FunctionType).GasLimit + index, 0);
                            if (mutex.WaitOne())
                            {
                                ++index;
                                mutex.ReleaseMutex();
                            }
                        }
                        finally
                        {
                            mutex.ReleaseMutex();
                        }
                    });
                }
                catch(Exception ex)
                {
                    Logger.LogError(new EventId(5), ex, string.Format("Error on GeneratePaymentContractTransaction for contract {0}.", pensionFund.Option.Company.Employee.Address));
                }
            });
        }

        private PensionFundTransaction GenerateContractTransaction(PensionFundTransaction pensionFundTransaction,  string employeeAddress, 
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

            Update(pensionFundTransaction);
            return pensionFundTransaction;
        }

        private PensionFundTransaction CreateTransaction(DateTime date, FunctionType functionType, string responsableAddress, string pensionFundContractHash)
        {
            PensionFundTransaction pensionFundTransaction = new PensionFundTransaction()
            {
                CreationDate = date,
                ContractFunctionId = functionType.Type,
                PensionFundContractHash = pensionFundContractHash,
                WalletAddress = responsableAddress
            };
            Insert(pensionFundTransaction);
            return pensionFundTransaction;
        }
    }
}
