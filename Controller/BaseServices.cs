using Auctus.Business.Accounts;
using Auctus.Business.Contracts;
using Auctus.Business.Funds;
using Auctus.Business.Unprocessed;
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
        protected readonly ILoggerFactory Logger;

        protected BaseServices(ILoggerFactory loggerFactory, Cache cache)
        {
            MemoryCache = cache;
            Logger = loggerFactory;
        }

        protected PensionFundBusiness PensionFundBusiness { get { return new PensionFundBusiness(Logger, MemoryCache); } }
        protected PensionFundOptionBusiness PensionFundOptionBusiness { get { return new PensionFundOptionBusiness(Logger, MemoryCache); } }
        protected SmartContractBusiness SmartContractBusiness { get { return new SmartContractBusiness(Logger, MemoryCache); } }
        protected PensionFundTransactionBusiness PensionFundTransactionBusiness { get { return new PensionFundTransactionBusiness(Logger, MemoryCache); } }
        protected PensionFundContractBusiness PensionFundContractBusiness { get { return new PensionFundContractBusiness(Logger, MemoryCache); } }
        protected UPensionFundBusiness UPensionFundBusiness { get { return new UPensionFundBusiness(Logger, MemoryCache); } }
        protected PensionFundReferenceContractBusiness PensionFundReferenceContractBusiness { get { return new PensionFundReferenceContractBusiness(Logger, MemoryCache); } }
        protected WalletBusiness WalletBusiness { get { return new WalletBusiness(Logger, MemoryCache); } }
        protected EmployeeBusiness EmployeeBusiness { get { return new EmployeeBusiness(Logger, MemoryCache); } }
        protected CompanyBusiness CompanyBusiness { get { return new CompanyBusiness(Logger, MemoryCache); } }
        protected BonusDistributionBusiness BonusDistributionBusiness { get { return new BonusDistributionBusiness(Logger, MemoryCache); } }
        protected ReferenceContractBusiness ReferenceContractBusiness { get { return new ReferenceContractBusiness(Logger, MemoryCache); } }
    }
}
