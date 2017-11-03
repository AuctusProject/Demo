using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Auctus.EthereumProxy;
using Auctus.Util;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Auctus.DomainObjects.Funds;

namespace Auctus.Business.Contracts
{
    public class PensionFundContractBusiness : BaseBusiness<PensionFundContract, PensionFundContractData>
    {
        public PensionFundContractBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        public PensionFundContract Create(String pensionFundAddress, String employerAddress, String employeeAddress, double pensionFundFee,
            double pensionFundLatePenalty,
            double maxSalaryBonus,
            double employeeContribution,
            double employeeContributionBonus,
            double employeeSalary,
            Dictionary<string, double> referenceValues,
            Dictionary<int, double> bonusVestingDistribuition)
        {
            var defaultDemonstrationPensionFundSmartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();

            KeyValuePair<string, string> demoTransaction = EthereumManager.DeployDefaultPensionFund(defaultDemonstrationPensionFundSmartContract.GasLimit, 
                pensionFundAddress, employerAddress, employeeAddress,
                pensionFundFee, pensionFundLatePenalty, AUCTUS_FEE, maxSalaryBonus, employeeContribution, employeeContributionBonus, employeeSalary, 
                referenceValues, bonusVestingDistribuition.ToDictionary(c => c.Key * 12, c => c.Value));            

            var pensionFundContract = new PensionFundContract()
            {
                CreationDate = DateTime.UtcNow,
                PensionFundOptionAddress = pensionFundAddress,
                TransactionHash = demoTransaction.Key,
                SmartContractId = defaultDemonstrationPensionFundSmartContract.Id,
                SmartContractCode = demoTransaction.Value
            };
            Insert(pensionFundContract);

            return pensionFundContract;
        }

        public void ReadContractMined()
        {
            var unminedContracts = Data.ListPendingMiningContracts();
            Parallel.ForEach(unminedContracts, new ParallelOptions() { MaxDegreeOfParallelism = 5 },
                contract =>
                {
                    try
                    {
                        Logger.LogInformation($"Read contract mined: {contract.TransactionHash}");
                        CheckContractCreationTransaction(contract);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.ToString());
                    }
                });
        }

        public PensionFundContract Get(String transactionHash)
        {
            return Data.GetPensionFundContract(transactionHash);
        }

        public PensionFundContract GetByAddress(String contractAddress)
        {
            return Data.GetPensionFundContractByAddress(contractAddress);
        }

        public PensionFundContract CheckContractCreationTransaction(PensionFundContract pensionFundContract)
        {
            Transaction demoContractTransaction = EthereumManager.GetTransaction(pensionFundContract.TransactionHash);
            if (demoContractTransaction == null)
            {
                if (pensionFundContract.CreationDate < DateTime.UtcNow.AddMinutes(PensionFundTransactionBusiness.BLOCKCHAIN_TRANSACTION_TOLERANCE))
                {
                    PoolInfo poolInfo = GetPoolInfo();
                    if (!poolInfo.Pending.Contains(pensionFundContract.TransactionHash))
                    {
                        Logger.LogError(string.Format("Transaction for creation contract {0} is lost.", pensionFundContract.TransactionHash));
                        List<PensionFundReferenceContract> referenceDistribution = PensionFundReferenceContractBusiness.List(pensionFundContract.TransactionHash);
                        if (!referenceDistribution.Any())
                            throw new InvalidOperationException("Reference contract cannot be found.");

                        PensionFund pensionFund = PensionFundBusiness.GetByTransaction(pensionFundContract.TransactionHash);
                        if (pensionFund == null)
                            throw new ArgumentException("Pension fund cannot be found.");

                        pensionFund.Option.Company.BonusDistribution = BonusDistributionBusiness.List(pensionFund.Option.Company.Address);
                        if (!pensionFund.Option.Company.BonusDistribution.Any())
                            throw new ArgumentException("Bonus distribution cannot be found.");

                        PensionFundReferenceContractBusiness.Delete(pensionFundContract.TransactionHash);
                        Delete(pensionFundContract);
                        pensionFundContract = Create(pensionFund.Option.Address, pensionFund.Option.Company.Address,
                            pensionFund.Option.Company.Employee.Address, pensionFund.Option.Fee, pensionFund.Option.LatePenalty,
                            pensionFund.Option.Company.MaxSalaryBonusRate, pensionFund.Option.Company.Employee.Contribution,
                            pensionFund.Option.Company.BonusRate, pensionFund.Option.Company.Employee.Salary,
                            referenceDistribution.ToDictionary(c => c.ReferenceContractAddress, c => c.Percentage),
                            pensionFund.Option.Company.BonusDistribution.ToDictionary(c => c.Period, c => c.ReleasedBonus));
                        foreach (PensionFundReferenceContract reference in referenceDistribution)
                            PensionFundReferenceContractBusiness.Create(pensionFundContract.TransactionHash, reference.ReferenceContractAddress, reference.Percentage);
                    }
                }
                return pensionFundContract;
            }
            else
                pensionFundContract = UpdateAfterMined(pensionFundContract, demoContractTransaction);

            return pensionFundContract;
        }

        public PensionFundContract UpdateAfterMined(PensionFundContract pensionFundContract, Transaction contractTransaction)
        {
            pensionFundContract.Address = contractTransaction.ContractAddress;
            pensionFundContract.BlockNumber = contractTransaction.BlockNumber;
            pensionFundContract.GasUsed = contractTransaction.GasUsed;
            Update(pensionFundContract);
            return pensionFundContract;
        }

        internal void UpdateWithdrawalCashbackWithSmartContractValues(string pensionFundContractAddress)
        {
            PensionFund pensionFund = PensionFundBusiness.GetByContract(pensionFundContractAddress);
            SmartContract smartContract = SmartContractBusiness.GetDefaultDemonstrationPensionFund();
            WithdrawalInfo withdrawalInfo = EthereumManager.GetWithdrawalInfo(pensionFund.Option.Company.Employee.Address, pensionFund.Option.PensionFundContract.Address, smartContract.ABI);
            pensionFund.Option.PensionFundContract.EmployeeCashback = withdrawalInfo.EmployeeSzaboCashback;
            pensionFund.Option.PensionFundContract.EmployerCashback = withdrawalInfo.EmployerSzaboCashback;
            Update(pensionFund.Option.PensionFundContract);
        }
    }
}
