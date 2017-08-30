using Auctus.Business.Funds;
using Auctus.DomainObjects.Funds;
using Auctus.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public class FundsServices
    {
        public static void CreateCompleteEntry(Fund fund, Company company, Employee employee, Contract contract)
        {
            new PensionFundBusiness().CreateCompleteEntry(fund, company, employee, contract);
        }

        //TODO: REMOVE/REFACTOR AFTER IMPLEMENTATION
        public static bool DeployContract()
        {
            return PensionFundBusiness.DeployContract();
        }       
    }
}
