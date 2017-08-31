using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Auctus.EthereumProxy;

namespace Auctus.Business.Contracts
{
    public class PensionFundContractBusiness : BaseBusiness<PensionFundContract, PensionFundContractData>
    {
        public PensionFundContract Create(String pensionFundAddress, String employerAddress, String employeeAddress, double pensionFundFee,
            double pensionFundLatePenalty,
            double maxSalaryBonus,
            double employeeContribution,
            double employeeContributionBonus,
            double employeeSalary,
            Dictionary<string, double> referenceValues,
            Dictionary<int, double> bonusVestingDistribuition)
        {
            var defaultDemonstrationPensionFundSmartContract = new SmartContractBusiness().GetDefaultDemonstrationPensionFund();

            KeyValuePair<string, string> demoTransaction = EthereumManager.DeployDefaultPensionFund(defaultDemonstrationPensionFundSmartContract.GasLimit, 
                pensionFundAddress, employerAddress, employeeAddress,
                pensionFundFee, pensionFundLatePenalty, 20, maxSalaryBonus, employeeContribution, employeeContributionBonus, employeeSalary, 
                referenceValues, bonusVestingDistribuition);            

            var pensionFundContract = new PensionFundContract()
            {
                CreationDate = DateTime.Now,
                PensionFundOptionAddress = pensionFundAddress,
                TransactionHash = demoTransaction.Key,
                SmartContractId = defaultDemonstrationPensionFundSmartContract.Id,
                SmartContractCode = demoTransaction.Value
            };
            Insert(pensionFundContract);

            //TODO: Start process for polling transaction status;

            return pensionFundContract;
        }

        public PensionFundContract UpdateAfterMined(Int32 pensionFundContractId, Transaction contractTransaction)
        {
            throw new NotImplementedException();
        }
    }
}
