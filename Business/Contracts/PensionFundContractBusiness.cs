using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Auctus.EthereumProxy;
using Auctus.Util;
using System.Threading.Tasks;

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
                pensionFundFee, pensionFundLatePenalty, 20, maxSalaryBonus, employeeContribution, employeeContributionBonus, employeeSalary, 
                referenceValues, bonusVestingDistribuition);            

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

        public PensionFundContract CheckContractCreationTransaction(String transactionHash)
        {
            Transaction demoContractTransaction = EthereumManager.GetTransaction(transactionHash);
            if (demoContractTransaction == null)
                throw new InvalidOperationException();

            var pensionFundContract = UpdateAfterMined(demoContractTransaction);

            return pensionFundContract;
        }

        public PensionFundContract UpdateAfterMined(Transaction contractTransaction)
        {
            var pensionFundContract = Data.GetPensionFundContract(contractTransaction.TransactionHash);
            pensionFundContract.Address = contractTransaction.ContractAddress;
            pensionFundContract.BlockNumber = contractTransaction.BlockNumber;
            pensionFundContract.GasUsed = contractTransaction.GasUsed;
            Update(pensionFundContract);
            return pensionFundContract;
        }
    }
}
