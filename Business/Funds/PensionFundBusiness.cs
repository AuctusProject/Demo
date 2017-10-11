using Auctus.Business.Accounts;
using Auctus.Business.Contracts;
using Auctus.DataAccess.Funds;
using Auctus.DomainObjects.Contracts;
using Auctus.DomainObjects.Funds;
using Auctus.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using Auctus.EthereumProxy;

namespace Auctus.Business.Funds
{
    public class PensionFundBusiness : BaseBusiness<PensionFund, PensionFundData>
    {
        public PensionFundBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        public Withdrawal GetWithdrawalInfo(string pensionFundContractAddress)
        {
            PensionFund pensionFund = GetByContract(pensionFundContractAddress);
            SmartContract smartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();
            WithdrawalInfo withdrawalInfo = EthereumManager.GetWithdrawalInfo(pensionFund.Option.Company.Employee.Address, pensionFundContractAddress, smartContract.ABI);
            return new Withdrawal()
            {
                EmployeeAbsoluteBonus = withdrawalInfo.EmployeeAbsoluteBonus,
                EmployeeBonus = withdrawalInfo.EmployeeBonus,
                EmployeeSzaboCashback = withdrawalInfo.EmployeeSzaboCashback,
                EmployeeTokenCashback = withdrawalInfo.EmployeeTokenCashback,
                EmployerSzaboCashback = withdrawalInfo.EmployerSzaboCashback,
                EmployerTokenCashback = withdrawalInfo.EmployerTokenCashback
            };
        }

        public PensionFundInfo GetPensionFundInfo(string pensionFundContractAddress)
        {
            PensionFund pensionFund = GetByContract(pensionFundContractAddress);
            Dictionary<int, double> assetReference = new Dictionary<int, double>();
            List<Asset> assets = new List<Asset>();
            foreach (PensionFundReferenceContract reference in pensionFund.Option.PensionFundContract.PensionFundReferenceContract)
            {
                ReferenceContract contract = ReferenceContractBusiness.Get(reference.ReferenceType);
                assets.Add(new Asset()
                {
                    Address = contract.Address,
                    Name = contract.Description,
                    Percentage = reference.Percentage
                });
                IEnumerable<ReferenceValue> values = contract.ReferenceValue.OrderBy(c => c.Period);
                double baseValue = values.ElementAt(0).Value;
                foreach (ReferenceValue value in values)
                {
                    if (!assetReference.ContainsKey(value.Period))
                        assetReference[value.Period] = 0;
                    assetReference[value.Period] += (value.Value / baseValue) * reference.Percentage / 100;
                }
            }
            return new PensionFundInfo()
            {
                PensionFundName = pensionFund.Name,
                EmployeeName = pensionFund.Option.Company.Employee.Name,
                CompanyName = pensionFund.Option.Company.Name,
                PensionFundAddress = pensionFund.Option.Address,
                EmployeeAddress = pensionFund.Option.Company.Employee.Address,
                CompanyAddress = pensionFund.Option.Company.Address,
                PensionFundFee = pensionFund.Option.Fee,
                PensionFundLatePenalty = pensionFund.Option.LatePenalty,
                AuctusFee = PensionFundContractBusiness.AUCTUS_FEE,
                ContractAddress = pensionFund.Option.PensionFundContract.Address,
                ContractTransactionHash = pensionFund.Option.PensionFundContract.TransactionHash,
                ContractBlockNumber = pensionFund.Option.PensionFundContract.BlockNumber.Value,
                AssetsReferenceValue = assetReference.Select(c => new AssetsReferenceValue() { Period = c.Key, Value = c.Value }),
                Assets = assets,
                EmployeeBaseContribution = pensionFund.Option.Company.Employee.Salary * pensionFund.Option.Company.Employee.Contribution / 100,
                CompanyBaseContribution = pensionFund.Option.Company.Employee.Salary * pensionFund.Option.Company.BonusRate / 100 *
                                            Math.Min(pensionFund.Option.Company.MaxSalaryBonusRate, pensionFund.Option.Company.Employee.Contribution) / 100
            };
        }

        internal Progress GetProgress(PensionFund pensionFund, IEnumerable<Payment> payments)
        {
            Progress progress = new Progress();
            progress.StartTime = pensionFund.Option.PensionFundContract.CreationDate.Ticks;
            
            IEnumerable<Payment> completed = payments.Where(c => c.Period.HasValue).OrderBy(c => c.Period.Value);
            progress.Values = new List<ProgressValue>();
            progress.TransactionHistory = new List<TransactionHistory>();
            List<Payment> alreadyIdentified = new List<Payment>();
            Payment last = null;
            if (completed.Count() > 0)
            {
                last = completed.Last();
                string employeeTransaction = null, companyTransaction = null;
                int? employeeBlockNumber = null, companyBlockNumber = null;
                double? employeeToken = null, companyToken = null;
                ProgressValue progressValue = null;
                Payment previousPayment = null;
                double totalToken = 0, investedToken = 0, invested = 0, pensionFundFee = 0, auctusFee = 0;
                foreach (Payment payment in completed)
                {
                    if (progressValue == null || payment.Period.Value > progressValue.Period)
                    {
                        if (progressValue != null)
                        {
                            AddProgressValue(progress, progressValue, pensionFund, previousPayment, totalToken, investedToken, invested, pensionFundFee, auctusFee);
                            AddProgressTransaction(progress, payments, alreadyIdentified, previousPayment.CreatedDate, previousPayment.ReferenceDate.Value,
                                pensionFund.Option.Company.Employee.Address, pensionFund.Option.Company.Address, employeeTransaction, companyTransaction,
                                employeeBlockNumber, companyBlockNumber, employeeToken, companyToken);
                        }

                        progressValue = new ProgressValue();
                        progressValue.Date = payment.ReferenceDate.Value.Ticks;
                        progressValue.Period = payment.Period.Value;
                        employeeTransaction = null; companyTransaction = null; employeeBlockNumber = null;
                        companyBlockNumber = null; employeeToken = null; companyToken = null;
                    }
                    pensionFundFee += payment.PensionFundFee.Value;
                    auctusFee += payment.AuctusFee.Value;
                    totalToken += payment.TokenAmount.Value;
                    if (payment.Responsable == pensionFund.Option.Company.Employee.Address)
                    {
                        investedToken += payment.TokenAmount.Value;
                        invested += payment.SzaboInvested.Value;
                        employeeBlockNumber = payment.BlockNumber.Value;
                        employeeTransaction = payment.TransactionHash;
                        employeeToken = payment.TokenAmount.Value;
                    }
                    else
                    {
                        companyBlockNumber = payment.BlockNumber.Value;
                        companyTransaction = payment.TransactionHash;
                        companyToken = payment.TokenAmount.Value;
                    }
                    previousPayment = payment;
                }
                AddProgressValue(progress, progressValue, pensionFund, last, totalToken, investedToken, invested, pensionFundFee, auctusFee);
                AddProgressTransaction(progress, payments, alreadyIdentified, last.CreatedDate, last.ReferenceDate.Value,
                    pensionFund.Option.Company.Employee.Address, pensionFund.Option.Company.Address, employeeTransaction, companyTransaction,
                    employeeBlockNumber, companyBlockNumber, employeeToken, companyToken);
                IEnumerable<DomainObjects.Accounts.BonusDistribution> bonusDistribution = pensionFund.Option.Company.BonusDistribution.Where(c => c.Period * 12 <= last.Period.Value);
                progress.CurrentVestingBonus = bonusDistribution.Count() > 0 ? bonusDistribution.Max(c => c.ReleasedBonus) : 0;
            }
            progress.LastPeriod = last != null ? last.Period.Value : 0;
            DateTime lastDate = last != null ? last.ReferenceDate.Value : pensionFund.Option.PensionFundContract.CreationDate;
            if (progress.LastPeriod > 0)
            {
                ProgressValue progressValue = progress.Values.Where(c => c.Period == progress.LastPeriod).Single();
                progress.TotalToken = progressValue.Token;
                progress.TotalVested = progressValue.Vested;
                progress.TotalPensinonFundFee = progressValue.PensinonFundFee;
                progress.TotalAuctusFee = progressValue.AuctusFee;
                progress.TotalInvested = progressValue.Invested;
            }

            IEnumerable<DomainObjects.Accounts.BonusDistribution> bonusAfterPeriod = pensionFund.Option.Company.BonusDistribution.Where(c => c.Period * 12 > progress.LastPeriod);
            if (bonusAfterPeriod.Count() > 0)
            {
                progress.NextVestingBonus = bonusAfterPeriod.Min(c => c.ReleasedBonus);
                progress.NextVestingDate = GetFormattedDate(lastDate.AddMonths(bonusAfterPeriod.Min(c => c.Period) * 12 - progress.LastPeriod).Date);
            }
            else
            {
                progress.NextVestingBonus = progress.CurrentVestingBonus;
                progress.NextVestingDate = GetFormattedDate(lastDate);
            }

            IEnumerable<Payment> pendingToAdd = payments.Where(c => !c.ReferenceDate.HasValue && !alreadyIdentified.Contains(c));
            IEnumerable<Payment> employeeToAdd = pendingToAdd.Where(c => c.Responsable == pensionFund.Option.Company.Employee.Address).OrderBy(c => c.CreatedDate);
            IEnumerable<Payment> companyToAdd = pendingToAdd.Where(c => c.Responsable == pensionFund.Option.Company.Address).OrderBy(c => c.CreatedDate);
            foreach (var payment in employeeToAdd.Zip(companyToAdd, (e, c) => new { employee = e, company = c }))
            {
                TransactionHistory transaction = new TransactionHistory();
                transaction.CreationDate = payment.employee.CreatedDate.Ticks;
                transaction.EmployeeTransactionHash = payment.employee.TransactionHash;
                transaction.CompanyTransactionHash = payment.company.TransactionHash;
                progress.TransactionHistory.Add(transaction);
            }

            progress.TransactionHistory = progress.TransactionHistory.OrderByDescending(transactionHistory => transactionHistory.Status)
                .ThenByDescending(transactionHistory => transactionHistory.PaymentDate)
                .ThenByDescending(transactionHistory => transactionHistory.CreationDate).ToList();
            return progress;
        }

        private string GetFormattedDate(DateTime date)
        {
            string month;
            switch(date.Month) {
                case 1:
                    month = "JAN";
                    break;
                case 2:
                    month = "FEB";
                    break;
                case 3:
                    month = "MAR";
                    break;
                case 4:
                    month = "APR";
                    break;
                case 5:
                    month = "MAY";
                    break;
                case 6:
                    month = "JUN";
                    break;
                case 7:
                    month = "JUL";
                    break;
                case 8:
                    month = "AUG";
                    break;
                case 9:
                    month = "SEP";
                    break;
                case 10:
                    month = "OCT";
                    break;
                case 11:
                    month = "NOV";
                    break;
                default:
                    month = "DEC";
                    break;
            }
            return string.Format("{0} {1} {2}", date.Day, month, date.Year.ToString().Substring(2));
        }

        private void AddProgressValue(Progress progress, ProgressValue progressValue, PensionFund pensionFund, Payment payment, double totalToken, 
            double employeeToken, double invested, double pensionFundFee, double auctusFee)
        {
            double price = payment.TokenAmount.Value / (payment.SzaboInvested.Value + payment.LatePenalty.Value);
            IEnumerable<DomainObjects.Accounts.BonusDistribution> bonusDistribution = pensionFund.Option.Company.BonusDistribution.Where(c => c.Period * 12 <= payment.Period.Value);
            progressValue.Invested = invested;
            progressValue.Vested = (employeeToken + (bonusDistribution.Count() > 0 ? (bonusDistribution.Max(c => c.ReleasedBonus) / 100 * (totalToken - employeeToken)) : 0)) / price;
            progressValue.Total = totalToken / price;
            progressValue.Token = employeeToken;
            progressValue.PensinonFundFee = pensionFundFee;
            progressValue.AuctusFee = auctusFee;
            progress.Values.Add(progressValue);
        }

        private void AddProgressTransaction(Progress progress, IEnumerable<Payment> payments, List<Payment> alreadyIdentified, DateTime createdDate, DateTime referenceDate,
            string employeeAddress, string companyAddress, string employeeTransaction, string companyTransaction, int? employeeBlockNumber, int? companyBlockNumber, 
            double? employeeToken, double? companyToken)
        {
            TransactionHistory transaction = new TransactionHistory();
            transaction.CreationDate = createdDate.Ticks;
            transaction.PaymentDate = referenceDate.ToString("yyyy - MM - dd");
            transaction.CompanyBlockNumber = companyBlockNumber;
            transaction.EmployeeBlockNumber = employeeBlockNumber;
            transaction.EmployeeToken = employeeToken;
            transaction.CompanyToken = companyToken;
            if (!string.IsNullOrEmpty(employeeTransaction) && !string.IsNullOrEmpty(companyTransaction))
            {
                transaction.EmployeeTransactionHash = employeeTransaction;
                transaction.CompanyTransactionHash = companyTransaction;
            }
            else if (!string.IsNullOrEmpty(employeeTransaction))
            {
                transaction.EmployeeTransactionHash = employeeTransaction;
                transaction.CompanyTransactionHash = FindPaymentThatMatchWith(payments, alreadyIdentified, createdDate, employeeAddress, companyAddress)?.TransactionHash;
            }
            else
            {
                transaction.CompanyTransactionHash = companyTransaction;
                transaction.EmployeeTransactionHash = FindPaymentThatMatchWith(payments, alreadyIdentified, createdDate, companyAddress, employeeAddress)?.TransactionHash;
            }
            progress.TransactionHistory.Add(transaction);
        }

        private Payment FindPaymentThatMatchWith(IEnumerable<Payment> payments, List<Payment> alreadyIdentified, DateTime createdDate, string baseAddress, string searchAddress)
        {
            IEnumerable<Payment> possibilities = payments.Where(c => c.Responsable == searchAddress && !c.BlockNumber.HasValue && !alreadyIdentified.Contains(c));
            Payment possiblePayment = possibilities.Where(c => c.CreatedDate == createdDate).FirstOrDefault();
            if (possiblePayment == null)
                possiblePayment = possibilities.Where(c => !payments.Any(l => l.Responsable == baseAddress && c.CreatedDate == l.CreatedDate)).OrderBy(c => c.CreatedDate).FirstOrDefault();

            if (possiblePayment != null)
                alreadyIdentified.Add(possiblePayment);
            return possiblePayment;
        }

        internal PensionFund GetByTransaction(string pensionFundContractHash)
        {
            return GetByTransaction(pensionFundContractHash);
        }

        internal PensionFund GetByContract(string pensionFundContractAddress)
        {
            if (!EthereumProxy.EthereumManager.IsValidAddress(pensionFundContractAddress))
                throw new ArgumentException("Invalid contract address.");

            string cacheKey = string.Format("PensionFund{0}", pensionFundContractAddress);
            PensionFund pensionFund = MemoryCache.Get<PensionFund>(cacheKey);
            if (pensionFund == null)
            {
                pensionFund = GetFromDatabase(pensionFundContractAddress);
                if (pensionFund != null)
                    MemoryCache.Set<PensionFund>(cacheKey, pensionFund);
            }
            return pensionFund;
        }

        private PensionFund GetFromDatabase(string pensionFundContractAddress)
        {
            PensionFund pensionFund = Data.GetByContract(pensionFundContractAddress);
            if (pensionFund == null || pensionFund.Option.Company == null || pensionFund.Option.Company.Employee == null)
                throw new ArgumentException("Pension fund cannot be found.");

            pensionFund.Option.PensionFundContract.PensionFundReferenceContract = PensionFundReferenceContractBusiness.List(pensionFund.Option.PensionFundContract.TransactionHash);
            if (!pensionFund.Option.PensionFundContract.PensionFundReferenceContract.Any())
                throw new ArgumentException("Reference contract cannot be found.");

            pensionFund.Option.Company.BonusDistribution = BonusDistributionBusiness.List(pensionFund.Option.Company.Address);
            if (!pensionFund.Option.Company.BonusDistribution.Any())
                throw new ArgumentException("Bonus distribution cannot be found.");

            return pensionFund;
        }

        public PensionFundContract CreateCompleteEntry(Fund fund, Company company, Employee employee)
        {
            Validate(fund, company, employee);
            var assetDictionary = GetAssetAllocationDictionary(fund);
            var pensionFund = PensionFundBusiness.Create(fund.Name);
            var pensionFundWallet = WalletBusiness.Create();
            var pensionFundOption = PensionFundOptionBusiness.Create(pensionFundWallet.Address, fund.Fee, fund.LatePaymentFee, pensionFund.Id);
            var companyWallet = WalletBusiness.Create();
            var domainCompany = CompanyBusiness.Create(companyWallet.Address, company.Name, company.BonusFee, company.MaxBonusFee, pensionFundOption.Address, company.VestingRules);
            var employeeWallet = WalletBusiness.Create();
            var domainEmployee = EmployeeBusiness.Create(employeeWallet.Address, employee.Name, employee.Salary, employee.ContributionPercentage, domainCompany.Address);
            var pensionFundContract = PensionFundContractBusiness.Create(pensionFundOption.Address, domainCompany.Address, domainEmployee.Address,
                pensionFundOption.Fee, pensionFundOption.LatePenalty, domainCompany.MaxSalaryBonusRate, domainEmployee.Contribution,
                domainCompany.BonusRate, domainEmployee.Salary,
                assetDictionary,
                company.VestingRules.ToDictionary(bonus => bonus.Period, bonus => bonus.Percentage));
            foreach (var asset in assetDictionary)
                PensionFundReferenceContractBusiness.Create(pensionFundContract.TransactionHash, asset.Key, asset.Value);

            return pensionFundContract;
        }

        public Dictionary<String, Double> GetAssetAllocationDictionary(Fund fund)
        {
            var dictionary = new Dictionary<String, Double>();
            if (fund.GoldPercentage > 0)
                dictionary.Add(ReferenceType.Gold.Address, fund.GoldPercentage);
            if (fund.MSCIPercentage > 0)
                dictionary.Add(ReferenceType.MSCIWorld.Address, fund.MSCIPercentage);
            if (fund.SPPercentage > 0)
                dictionary.Add(ReferenceType.SP500.Address, fund.SPPercentage);
            if (fund.VWEHXPercentage > 0)
                dictionary.Add(ReferenceType.VWEHX.Address, fund.VWEHXPercentage);
            if (fund.BitcoinPercentage > 0)
                dictionary.Add(ReferenceType.Bitcoin.Address, fund.BitcoinPercentage);
            return dictionary;
        }

        internal PensionFund Create(String name)
        {
            var pensionFund = new PensionFund();
            pensionFund.Name = name;
            Insert(pensionFund);
            return pensionFund;
        }

        internal static void Validate(Fund fund, Company company, Employee employee)
        {
            Validate(fund);
            CompanyBusiness.Validate(company);
            EmployeeBusiness.Validate(employee);
        }

        internal static void Validate(Fund fund)
        {
            if (fund == null)
                throw new ArgumentNullException("fund");
            if (fund.LatePaymentFee < 0)
                throw new ArgumentException("Late Payment Fee cannot be negative.");
            ValidateNonNegative(fund.Fee, "Fee");
            if (fund.Fee > 99)
                throw new ArgumentException("Fee cannot be greater than 99.");
            ValidateNonNegative(fund.BitcoinPercentage, "BitcoinPercentage");
            ValidateNonNegative(fund.VWEHXPercentage, "VWEHXPercentage");
            ValidateNonNegative(fund.GoldPercentage, "GoldPercentage");
            ValidateNonNegative(fund.MSCIPercentage, "MSCIPercentage");
            ValidateNonNegative(fund.SPPercentage, "SPPercentage");
            if ((fund.BitcoinPercentage + fund.VWEHXPercentage + fund.GoldPercentage +fund.MSCIPercentage + fund.SPPercentage) != 100)
                throw new ArgumentException("Asset allocations must match 100 percentage.");
        }

        internal static void ValidateNonNegative(double value, String fieldName)
        {
            if (value < 0)
                throw new ArgumentException($"{fieldName} cannot be negative.");
        }
    }
}
