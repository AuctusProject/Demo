﻿using Auctus.Business.Accounts;
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

namespace Auctus.Business.Funds
{
    public class PensionFundBusiness : BaseBusiness<PensionFund, PensionFundData>
    {
        public PensionFundBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        public PensionFundInfo GetPensionFundInfo(string pensionFundContractAddress)
        {
            PensionFund pensionFund = GetByContract(pensionFundContractAddress);
            AssetsReferenceValue[] assetReference = new AssetsReferenceValue[60];
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
                    assetReference[value.Period - 1].Period = value.Period;
                    assetReference[value.Period - 1].Value += (baseValue / value.Value) * reference.Percentage / 100;
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
                AssetsReferenceValue = assetReference,
                Assets = assets,
                Withdrawal = PensionFundTransactionBusiness.ReadWithdrawal(pensionFund.Option.PensionFundContract.Address),
                Progress = PensionFundTransactionBusiness.ReadPayments(pensionFund.Option.PensionFundContract.Address),
                EmployeeBaseContribution = pensionFund.Option.Company.Employee.Salary * pensionFund.Option.Company.Employee.Contribution / 100,
                CompanyBaseContribution = pensionFund.Option.Company.Employee.Salary * pensionFund.Option.Company.BonusRate / 100 *
                                            Math.Min(pensionFund.Option.Company.MaxSalaryBonusRate, pensionFund.Option.Company.Employee.Contribution) / 100
            };
        }

        internal Progress GetProgress(PensionFund pensionFund, List<Payment> payments)
        {
            Progress progress = new Progress();
            progress.StartTime = pensionFund.Option.PensionFundContract.CreationDate;
            
            IEnumerable<Payment> completed = payments.Where(c => c.ReferenceDate.HasValue).OrderBy(c => c.ReferenceDate.Value);
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
                double totalToken = 0, investedToken = 0, invested = 0, pensionFundFee = 0, auctusFee = 0;
                foreach (Payment payment in completed)
                {
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
                    if (progressValue == null || payment.ReferenceDate.Value > progressValue.Date || payment == last)
                    {
                        if (progressValue != null)
                        {
                            AddProgressValue(progress, progressValue, pensionFund, payment, totalToken, investedToken, invested, pensionFundFee, auctusFee);
                            AddProgressTransaction(progress, payments, alreadyIdentified, payment.CreatedDate, payment.ReferenceDate.Value,
                                pensionFund.Option.Company.Employee.Address, pensionFund.Option.Company.Address, employeeTransaction, companyTransaction,
                                employeeBlockNumber, companyBlockNumber, employeeToken, companyToken);
                        }

                        progressValue = new ProgressValue();
                        progressValue.Date = payment.ReferenceDate.Value;
                        employeeTransaction = null; companyTransaction = null; employeeBlockNumber = null;
                        companyBlockNumber = null; employeeToken = null; companyToken = null;
                    }
                }
                progress.CurrentVestingBonus = pensionFund.Option.Company.BonusDistribution.Where(c => c.Period * 12 <= last.Period.Value).Max(c => c.ReleasedBonus);
            }
            int lastPeriod = last != null ? last.Period.Value : 0;
            DateTime lastDate = last != null ? last.ReferenceDate.Value : progress.StartTime;
            IEnumerable<DomainObjects.Accounts.BonusDistribution> bonusAfterPeriod = pensionFund.Option.Company.BonusDistribution.Where(c => c.Period * 12 > lastPeriod);
            if (bonusAfterPeriod.Count() > 0)
            {
                progress.NextVestingBonus = bonusAfterPeriod.Min(c => c.ReleasedBonus);
                progress.NextVestingDate = lastDate.AddMonths(bonusAfterPeriod.Min(c => c.Period) * 12 - lastPeriod);
            }
            else
            {
                progress.NextVestingBonus = progress.CurrentVestingBonus;
                progress.NextVestingDate = lastDate;
            }

            IEnumerable<Payment> pendingToAdd = payments.Where(c => !c.ReferenceDate.HasValue && !alreadyIdentified.Contains(c));
            IEnumerable<Payment> employeeToAdd = pendingToAdd.Where(c => c.Responsable == pensionFund.Option.Company.Employee.Address).OrderBy(c => c.CreatedDate);
            IEnumerable<Payment> companyToAdd = pendingToAdd.Where(c => c.Responsable == pensionFund.Option.Company.Address).OrderBy(c => c.CreatedDate);
            foreach (var payment in employeeToAdd.Zip(companyToAdd, (e, c) => new { employee = e, company = c }))
            {
                TransactionHistory transaction = new TransactionHistory();
                transaction.CreationDate = payment.employee.CreatedDate;
                transaction.EmployeeTransactionHash = payment.employee.TransactionHash;
                transaction.CompanyTransactionHash = payment.company.TransactionHash;
                progress.TransactionHistory.Add(transaction);
            }
            return progress;
        }

        private void AddProgressValue(Progress progress, ProgressValue progressValue, PensionFund pensionFund, Payment payment, double totalToken, 
            double employeeToken, double invested, double pensionFundFee, double auctusFee)
        {
            double price = payment.TokenAmount.Value / (payment.SzaboInvested.Value + payment.LatePenalty.Value);
            progressValue.Invested = invested;
            progressValue.Vested = (employeeToken + (pensionFund.Option.Company.BonusDistribution.Where(c => c.Period * 12 <= payment.Period.Value).Max(c => c.ReleasedBonus) / 100 * (totalToken - employeeToken))) / price;
            progressValue.Total = totalToken / price;
            progressValue.Token = employeeToken;
            progressValue.PensinonFundFee = pensionFundFee;
            progressValue.AuctusFee = auctusFee;
            progress.Values.Add(progressValue);
        }

        private void AddProgressTransaction(Progress progress, List<Payment> payments, List<Payment> alreadyIdentified, DateTime createdDate, DateTime referenceDate,
            string employeeAddress, string companyAddress, string employeeTransaction, string companyTransaction, int? employeeBlockNumber, int? companyBlockNumber, 
            double? employeeToken, double? companyToken)
        {
            TransactionHistory transaction = new TransactionHistory();
            transaction.CreationDate = createdDate;
            transaction.PaymentDate = referenceDate;
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
                transaction.CompanyTransactionHash = FindPaymentThatMatchWith(payments, alreadyIdentified, createdDate, employeeAddress, companyAddress).TransactionHash;
            }
            else
            {
                transaction.CompanyTransactionHash = companyTransaction;
                transaction.EmployeeTransactionHash = FindPaymentThatMatchWith(payments, alreadyIdentified, createdDate, companyAddress, employeeAddress).TransactionHash;
            }
            progress.TransactionHistory.Add(transaction);
        }

        private Payment FindPaymentThatMatchWith(List<Payment> payments, List<Payment> alreadyIdentified, DateTime createdDate, string baseAddress, string searchAddress)
        {
            IEnumerable<Payment> possibilities = payments.Where(c => c.Responsable == searchAddress && !c.BlockNumber.HasValue && !alreadyIdentified.Contains(c));
            Payment possiblePayment = possibilities.Where(c => c.CreatedDate == createdDate).FirstOrDefault();
            if (possiblePayment == null)
                possiblePayment = possibilities.Where(c => !payments.Any(l => l.Responsable == baseAddress && c.CreatedDate == l.CreatedDate)).OrderBy(c => c.CreatedDate).First();

            alreadyIdentified.Add(possiblePayment);
            return possiblePayment;
        }

        public PensionFund GetByTransaction(string pensionFundContractHash)
        {
            return GetByTransaction(pensionFundContractHash);
        }

        public PensionFund GetByContract(string pensionFundContractAddress)
        {
            if (!EthereumProxy.EthereumManager.IsValidAddress(pensionFundContractAddress))
                throw new ArgumentException("Invalid contract address.");

            string cacheKey = string.Format("PensionFund{0}", pensionFundContractAddress);
            PensionFund pensionFund = MemoryCache.Get<PensionFund>(cacheKey);
            if (pensionFund == null)
                MemoryCache.Set<PensionFund>(cacheKey, GetFromDatabase(pensionFundContractAddress));

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
                fund.AssetAllocations.ToDictionary(asset => asset.ReferenceContractAddress, asset => asset.Percentage),
                company.VestingRules.ToDictionary(bonus => bonus.Period, bonus => bonus.Percentage));
            foreach (AssetAllocation asset in fund.AssetAllocations)
                PensionFundReferenceContractBusiness.Create(pensionFundContract.TransactionHash, asset.ReferenceContractAddress, asset.Percentage);

            return pensionFundContract;
        }

        public PensionFund Create(String name)
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
            if (fund.Fee < 0)
                throw new ArgumentException("Fee cannot be negative.");
            if (fund.Fee > 99)
                throw new ArgumentException("Fee cannot be greater than 99.");
        }
    }
}
