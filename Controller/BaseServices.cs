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
        protected readonly Cache MemoryCache;
        
        protected BaseServices(Cache cache)
        {
            MemoryCache = cache;
        }

        protected PensionFundBusiness PensionFundBusiness { get { return new PensionFundBusiness(MemoryCache); } }
        protected PensionFundOptionBusiness PensionFundOptionBusiness { get { return new PensionFundOptionBusiness(MemoryCache); } }
        protected SmartContractBusiness SmartContractBusiness { get { return new SmartContractBusiness(MemoryCache); } }
        protected PensionFundTransactionBusiness PensionFundTransactionBusiness { get { return new PensionFundTransactionBusiness(MemoryCache); } }
        protected PensionFundContractBusiness PensionFundContractBusiness { get { return new PensionFundContractBusiness(MemoryCache); } }
        protected PensionFundReferenceContractBusiness PensionFundReferenceContractBusiness { get { return new PensionFundReferenceContractBusiness(MemoryCache); } }
        protected WalletBusiness WalletBusiness { get { return new WalletBusiness(MemoryCache); } }
        protected EmployeeBusiness EmployeeBusiness { get { return new EmployeeBusiness(MemoryCache); } }
        protected CompanyBusiness CompanyBusiness { get { return new CompanyBusiness(MemoryCache); } }
        protected BonusDistributionBusiness BonusDistributionBusiness { get { return new BonusDistributionBusiness(MemoryCache); } }
        protected UserBusiness UserBusiness { get { return new UserBusiness(MemoryCache); } }
        protected ReferenceContractBusiness ReferenceContractBusiness { get { return new ReferenceContractBusiness(MemoryCache); } }
    }
}
