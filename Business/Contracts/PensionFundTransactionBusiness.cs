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
        public const double BLOCKCHAIN_TRANSACTION_TOLERANCE = 3;
        public const double CREATION_TRANSACTION_TOLERANCE = 2.5;
        public const double CREATION_TOLERANCE = 0.5;

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
            IEnumerable<PensionFundTransaction> pendingCreateTransactions = transactions.Where(c => string.IsNullOrEmpty(c.TransactionHash));
            IEnumerable<PensionFundTransaction> pendingCompleteTransactions = transactions.Where(c => !c.BlockNumber.HasValue && !string.IsNullOrEmpty(c.TransactionHash));

            if (!transactions.Any())
                return PensionFundBusiness.GetProgress(pensionFund, new List<Payment>());
            else if (cachedPayment != null && cachedPayment.Count == transactions.Count())
                return PensionFundBusiness.GetProgress(pensionFund, cachedPayment);
            else if (cachedPayment != null && (cachedPayment.Count + pendingCreateTransactions.Count()) == transactions.Count())
            {
                transactions = HandleTransactions(smartContract, pensionFund, transactions, pendingCreateTransactions, pendingCompleteTransactions, new BaseEventInfo[] { });
                return PensionFundBusiness.GetProgress(pensionFund, pendingCreateTransactions.Select(c => new Payment()
                {
                    CreatedDate = c.CreationDate,
                    Responsable = c.WalletAddress
                }).Concat(cachedPayment));
            }

            List < BuyInfo> buyEvents = EthereumManager.ReadBuyFromDefaultPensionFund(contractAddress);
            transactions = HandleTransactions(smartContract, pensionFund, transactions, pendingCreateTransactions, pendingCompleteTransactions, 
                buyEvents.Select(c => new BaseEventInfo() { BlockNumber = c.BlockNumber, TransactionHash = c.TransactionHash }));

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
            IEnumerable<Payment> remainingPayments = transactions.Where(c => string.IsNullOrEmpty(c.TransactionHash) || 
            !completedPayments.Any(l => l.TransactionHash == c.TransactionHash)).Select(c => new Payment()
            {
                TransactionHash = c.TransactionHash,
                CreatedDate = c.CreationDate,
                Responsable = c.WalletAddress
            });
            
            return PensionFundBusiness.GetProgress(pensionFund, remainingPayments.Concat(completedPayments));
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

            PensionFundTransaction[] pendingCreate = string.IsNullOrEmpty(transaction.TransactionHash) ? new PensionFundTransaction[] { transaction } : new PensionFundTransaction[0];
            PensionFundTransaction[] pendingComplete = pendingCreate.Length == 0 && !transaction.BlockNumber.HasValue ? new PensionFundTransaction[] { transaction } : new PensionFundTransaction[0];
            BaseEventInfo[] withdrawEvent = withdrawalInfo != null ? new BaseEventInfo[] { withdrawalInfo } : new BaseEventInfo[0];

            HandleTransactions(smartContract, pensionFund, new PensionFundTransaction[] { }, pendingCreate, pendingComplete, withdrawEvent);

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
            Parallel.For(1, monthsAmount + 1, new ParallelOptions() { MaxDegreeOfParallelism = 5 },
            month =>
            {
                DateTime date = DateTime.UtcNow;
                newTransactions.Add(CreateTransaction(date, FunctionType.EmployeeBuy, pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.TransactionHash));
                newTransactions.Add(CreateTransaction(date, FunctionType.CompanyBuy, pensionFund.Option.Company.Address, pensionFund.Option.PensionFundContract.TransactionHash));
            });

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
                    return PensionFundBusiness.GetProgress(pensionFund, newPayments.Concat(cachedPayment));
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

        private IEnumerable<PensionFundTransaction> HandleTransactions(SmartContract smartContract, PensionFund pensionFund, 
            IEnumerable<PensionFundTransaction> allTransactions, IEnumerable<PensionFundTransaction> pendingCreateTransactions, 
            IEnumerable<PensionFundTransaction> pendingCompleteTransactions, IEnumerable<BaseEventInfo> events)
        {
            DateTime chainTolerance = DateTime.UtcNow.AddMinutes(-BLOCKCHAIN_TRANSACTION_TOLERANCE);
            DateTime creationTransactionTolerance = DateTime.UtcNow.AddMinutes(-CREATION_TRANSACTION_TOLERANCE);
            DateTime creationTolerance = DateTime.UtcNow.AddMinutes(-CREATION_TOLERANCE);

            List<PensionFundTransaction> toBeRemoved = new List<PensionFundTransaction>();
            List<PensionFundTransaction> toBeAdded = new List<PensionFundTransaction>();

            int employeeTransactions = allTransactions.Count(c => c.WalletAddress == pensionFund.Option.Company.Employee.Address);
            int companyTransactions = allTransactions.Count(c => c.WalletAddress == pensionFund.Option.Company.Address);
            if (employeeTransactions != companyTransactions && allTransactions.Max(c => c.CreationDate) < creationTolerance)
            {
                string wallet;
                FunctionType function;
                if (employeeTransactions > companyTransactions)
                {
                    wallet = pensionFund.Option.Company.Address;
                    function = FunctionType.CompanyBuy;
                }
                else
                {
                    wallet = pensionFund.Option.Company.Employee.Address;
                    function = FunctionType.EmployeeBuy;
                }
                for (int i = 0; i < Math.Abs(employeeTransactions - companyTransactions); ++i)
                {
                    PensionFundTransaction newTransaction = CreateTransaction(DateTime.UtcNow, function, wallet, pensionFund.Option.PensionFundContract.TransactionHash);
                    toBeAdded.Add(newTransaction);
                    GenerateContractTransaction(newTransaction, pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address,
                        smartContract.ABI, smartContract.ContractFunctions.Single(c => c.FunctionType == newTransaction.FunctionType).GasLimit, 0);
                }
            }
            
            foreach (PensionFundTransaction notCreated in pendingCreateTransactions.Where(c => c.CreationDate < creationTransactionTolerance))
            {
                Logger.LogError(string.Format("Transaction not created for contract {0}.", pensionFund.Option.PensionFundContract.Address));
                Delete(notCreated);
                PensionFundTransaction newTransaction = CreateTransaction(DateTime.UtcNow, notCreated.FunctionType, notCreated.WalletAddress, pensionFund.Option.PensionFundContract.TransactionHash);
                if (allTransactions.Count() > 0)
                {
                    toBeRemoved.Add(allTransactions.Single(c => c.Id == notCreated.Id));
                    toBeAdded.Add(newTransaction);
                }
                GenerateContractTransaction(newTransaction, pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address,
                    smartContract.ABI, smartContract.ContractFunctions.Single(c => c.FunctionType == newTransaction.FunctionType).GasLimit, 0);
            }

            IEnumerable<PensionFundTransaction> completed = pendingCompleteTransactions.Where(c => events.Any(k => k.TransactionHash == c.TransactionHash));
            if (completed.Any())
            {
                Parallel.ForEach(completed, new ParallelOptions() { MaxDegreeOfParallelism = 5 },
                    trans =>
                    {
                        trans.BlockNumber = events.Single(c => c.TransactionHash == trans.TransactionHash).BlockNumber;
                        Update(trans);
                    });
            }
            else if (pendingCompleteTransactions.Any(c => c.CreationDate < chainTolerance))
            {
                PoolInfo poolInfo = GetPoolInfo();
                IEnumerable<PensionFundTransaction> lostTransactions = pendingCompleteTransactions.Where(c => c.CreationDate < chainTolerance &&
                    !poolInfo.Pending.Contains(c.TransactionHash));
                foreach (PensionFundTransaction lost in lostTransactions)
                {
                    Logger.LogError(string.Format("Transaction {0} for contract {1} is lost.", lost.TransactionHash, pensionFund.Option.PensionFundContract.Address));
                    Delete(lost);
                    PensionFundTransaction newTransaction = CreateTransaction(DateTime.UtcNow, lost.FunctionType, lost.WalletAddress, pensionFund.Option.PensionFundContract.TransactionHash);
                    if (allTransactions.Count() > 0)
                    {
                        toBeRemoved.Add(allTransactions.Single(c => c.Id == lost.Id));
                        toBeAdded.Add(newTransaction);
                    }
                    GenerateContractTransaction(newTransaction, pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address, 
                        smartContract.ABI, smartContract.ContractFunctions.Single(c => c.FunctionType == newTransaction.FunctionType).GasLimit, 0);
                }
            }
            else if (allTransactions.Count() > 0 && toBeAdded.Count == 0 && events.Count() > allTransactions.Count())
            {
                IEnumerable<BaseEventInfo> inconsistentInfos = events.Where(c => !allTransactions.Any(l => l.TransactionHash == c.TransactionHash));
                for (int i = 0; i < inconsistentInfos.Count(); ++i)
                {
                    FunctionType functionType;
                    string address;
                    if (i % 2 == 0)
                    {
                        functionType = FunctionType.EmployeeBuy;
                        address = pensionFund.Option.Company.Employee.Address;
                    }
                    else
                    {
                        functionType = FunctionType.CompanyBuy;
                        address = pensionFund.Option.Company.Address;
                    }
                    PensionFundTransaction newTransaction = CreateTransaction(DateTime.UtcNow, functionType, address, pensionFund.Option.PensionFundContract.TransactionHash, inconsistentInfos.ElementAt(i).BlockNumber);
                    toBeAdded.Add(newTransaction);
                }
            }
            if (toBeRemoved.Count > 0)
                allTransactions = allTransactions.Except(toBeRemoved);
            if (toBeAdded.Count > 0)
                allTransactions = allTransactions.Concat(toBeAdded);

            return allTransactions;
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
                    Parallel.For(0, transactions.Count(), new ParallelOptions() { MaxDegreeOfParallelism = 5 },
                    i =>
                    {
                        PensionFundTransaction transaction = transactions.ElementAt(i);
                        GenerateContractTransaction(transaction, pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address,
                            smartContract.ABI, smartContract.ContractFunctions.Single(c => c.FunctionType == transaction.FunctionType).GasLimit + i, 0);
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

        private PensionFundTransaction CreateTransaction(DateTime date, FunctionType functionType, string responsableAddress, 
            string pensionFundContractHash, int? blockNumber = null)
        {
            PensionFundTransaction pensionFundTransaction = new PensionFundTransaction()
            {
                CreationDate = date,
                ContractFunctionId = functionType.Type,
                PensionFundContractHash = pensionFundContractHash,
                WalletAddress = responsableAddress,
                BlockNumber = blockNumber
            };
            Insert(pensionFundTransaction);
            return pensionFundTransaction;
        }
    }
}
