using Auctus.Business.Accounts;
using Auctus.Business.Contracts;
using Auctus.Business.Funds;
using Auctus.Business.Security;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public abstract class BaseServices
    {
        protected readonly Cache memoryCache;
        
        protected BaseServices(Cache cache)
        {
            memoryCache = cache;
        }

        protected PensionFundBusiness PensionFundBusiness { get { return new PensionFundBusiness(memoryCache); } }
        protected PensionFundOptionBusiness PensionFundOptionBusiness { get { return new PensionFundOptionBusiness(memoryCache); } }
        protected SmartContractBusiness SmartContractBusiness { get { return new SmartContractBusiness(memoryCache); } }
        protected PensionFundTransactionBusiness PensionFundTransactionBusiness { get { return new PensionFundTransactionBusiness(memoryCache); } }
        protected PensionFundContractBusiness PensionFundContractBusiness { get { return new PensionFundContractBusiness(memoryCache); } }
        protected WalletBusiness WalletBusiness { get { return new WalletBusiness(memoryCache); } }
        protected EmployeeBusiness EmployeeBusiness { get { return new EmployeeBusiness(memoryCache); } }
        protected CompanyBusiness CompanyBusiness { get { return new CompanyBusiness(memoryCache); } }
        protected BonusDistributionBusiness BonusDistributionBusiness { get { return new BonusDistributionBusiness(memoryCache); } }
        protected UserBusiness UserBusiness { get { return new UserBusiness(memoryCache); } }
    }
}
