using Auctus.Business.Funds;
using Auctus.DomainObjects.Contracts;
using Auctus.DomainObjects.Funds;
using Auctus.Model;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public class FundsServices : BaseServices
    {
        public FundsServices(Cache cache) : base(cache) { }

        public PensionFundContract CreateCompleteEntry(Fund fund, Company company, Employee employee)
        {
            return PensionFundBusiness.CreateCompleteEntry(fund, company, employee);
        }

        public PensionFundContract CheckContractCreationTransaction(String transactionHash, Int32 pensionFundContractId)
        {
            return PensionFundContractBusiness.CheckContractCreationTransaction(transactionHash, pensionFundContractId);
        }

        //TODO: REMOVE/REFACTOR AFTER IMPLEMENTATION
        public bool DeployContract()
        {
            return PensionFundBusiness.DeployContract();
        }
    }
}
