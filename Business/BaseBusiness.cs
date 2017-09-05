using Auctus.Business.Accounts;
using Auctus.Business.Contracts;
using Auctus.Business.Funds;
using Auctus.Business.Security;
using Auctus.DataAccess;
using Auctus.EthereumProxy;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business
{
    public abstract class BaseBusiness<T, D> where D : BaseData<T>, new()
    {
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;
        protected readonly Cache MemoryCache;
        
        protected D Data => new D();

        private PensionFundBusiness _pensionFundBusiness;
        private PensionFundOptionBusiness _pensionFundOptionBusiness;
        private SmartContractBusiness _smartContractBusiness;
        private PensionFundTransactionBusiness _pensionFundTransactionBusiness;
        private PensionFundContractBusiness _pensionFundContractBusiness;
        private PensionFundReferenceContractBusiness _pensionFundReferenceContractBusiness;
        private WalletBusiness _walletBusiness;
        private EmployeeBusiness _employeeBusiness;
        private CompanyBusiness _companyBusiness;
        private BonusDistributionBusiness _bonusDistributionBusiness;
        private UserBusiness _userBusiness;
        private ReferenceContractBusiness _referenceContractBusiness;

        protected BaseBusiness(ILoggerFactory loggerFactory, Cache cache)
        {
            MemoryCache = cache;
            LoggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        public IEnumerable<T> ListAll()
        {
            return Data.SelectAll();
        }

        public void Insert(T obj)
        {
            Data.Insert(obj);
        }

        public void Update(T obj)
        {
            Data.Update(obj);
        }

        public void Delete(T obj)
        {
            Data.Delete(obj);
        }

        protected PoolInfo GetPoolInfo()
        {
            PoolInfo poolInfo = EthereumManager.GetPoolInfo();
            if (poolInfo.Queued.Count > 0)
                Logger.LogError(string.Format("Pool problem. {0} queued messages.", poolInfo.Queued.Count));
            if (poolInfo.Pending.Count > 120)
                Logger.LogError("Pool critical use. More then 120 pending messages.");
            return poolInfo;
        }

        protected PensionFundBusiness PensionFundBusiness
        {
            get
            {
                if (_pensionFundBusiness == null)
                    _pensionFundBusiness = new PensionFundBusiness(LoggerFactory, MemoryCache);
                return _pensionFundBusiness;
            }
        }

        protected PensionFundOptionBusiness PensionFundOptionBusiness
        {
            get
            {
                if (_pensionFundOptionBusiness == null)
                    _pensionFundOptionBusiness = new PensionFundOptionBusiness(LoggerFactory, MemoryCache);
                return _pensionFundOptionBusiness;
            }
        }

        protected SmartContractBusiness SmartContractBusiness
        {
            get
            {
                if (_smartContractBusiness == null)
                    _smartContractBusiness = new SmartContractBusiness(LoggerFactory, MemoryCache);
                return _smartContractBusiness;
            }
        }

        protected PensionFundTransactionBusiness PensionFundTransactionBusiness
        {
            get
            {
                if (_pensionFundTransactionBusiness == null)
                    _pensionFundTransactionBusiness = new PensionFundTransactionBusiness(LoggerFactory, MemoryCache);
                return _pensionFundTransactionBusiness;
            }
        }

        protected PensionFundContractBusiness PensionFundContractBusiness
        {
            get
            {
                if (_pensionFundContractBusiness == null)
                    _pensionFundContractBusiness = new PensionFundContractBusiness(LoggerFactory, MemoryCache);
                return _pensionFundContractBusiness;
            }
        }

        protected PensionFundReferenceContractBusiness PensionFundReferenceContractBusiness
        {
            get
            {
                if (_pensionFundReferenceContractBusiness == null)
                    _pensionFundReferenceContractBusiness = new PensionFundReferenceContractBusiness(LoggerFactory, MemoryCache);
                return _pensionFundReferenceContractBusiness;
            }
        }

        protected WalletBusiness WalletBusiness
        {
            get
            {
                if (_walletBusiness == null)
                    _walletBusiness = new WalletBusiness(LoggerFactory, MemoryCache);
                return _walletBusiness;
            }
        }

        protected EmployeeBusiness EmployeeBusiness
        {
            get
            {
                if (_employeeBusiness == null)
                    _employeeBusiness = new EmployeeBusiness(LoggerFactory, MemoryCache);
                return _employeeBusiness;
            }
        }

        protected CompanyBusiness CompanyBusiness
        {
            get
            {
                if (_companyBusiness == null)
                    _companyBusiness = new CompanyBusiness(LoggerFactory, MemoryCache);
                return _companyBusiness;
            }
        }

        protected BonusDistributionBusiness BonusDistributionBusiness
        {
            get
            {
                if (_bonusDistributionBusiness == null)
                    _bonusDistributionBusiness = new BonusDistributionBusiness(LoggerFactory, MemoryCache);
                return _bonusDistributionBusiness;
            }
        }

        protected UserBusiness UserBusiness
        {
            get
            {
                if (_userBusiness == null)
                    _userBusiness = new UserBusiness(LoggerFactory, MemoryCache);
                return _userBusiness;
            }
        }

        protected ReferenceContractBusiness ReferenceContractBusiness
        {
            get
            {
                if (_referenceContractBusiness == null)
                    _referenceContractBusiness = new ReferenceContractBusiness(LoggerFactory, MemoryCache);
                return _referenceContractBusiness;
            }
        }
    }
}
