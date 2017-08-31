using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Auctus.EthereumProxy;
using Auctus.Util;

namespace Auctus.Business.Contracts
{
    public class PensionFundContractBusiness : BaseBusiness<PensionFundContract, PensionFundContractData>
    {
        public PensionFundContractBusiness(Cache cache) : base(cache) { }

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
                pensionFundFee, pensionFundLatePenalty, 3, maxSalaryBonus, employeeContribution, employeeContributionBonus, employeeSalary, 
                referenceValues, bonusVestingDistribuition);
            Transaction demoContractTransaction = EthereumManager.GetTransaction(demoTransaction.Key);
            if (demoContractTransaction == null)
                throw new InvalidOperationException();

            var pensionFundContract = new PensionFundContract()
            {
                Address = demoContractTransaction.ContractAddress,
                BlockNumber = demoContractTransaction.BlockNumber,
                CreationDate = DateTime.UtcNow,
                GasUsed = demoContractTransaction.GasUsed,
                PensionFundOptionAddress = pensionFundAddress,
                TransactionHash = demoContractTransaction.TransactionHash,
                SmartContractId = defaultDemonstrationPensionFundSmartContract.Id,
                SmartContractCode = demoTransaction.Value
            };
            Insert(pensionFundContract);
            return pensionFundContract;
        }
    }
}
