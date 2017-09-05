using Auctus.Business.Accounts;
using Auctus.Business.Contracts;
using Auctus.Business.Funds;
using Auctus.Business.Security;
using Auctus.DataAccess;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business
{
    public abstract class BaseBusiness<T, D> where D : BaseData<T>, new()
    {
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

        protected BaseBusiness(Cache cache)
        {
            MemoryCache = cache;
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

        protected PensionFundBusiness PensionFundBusiness
        {
            get
            {
                if (_pensionFundBusiness == null)
                    _pensionFundBusiness = new PensionFundBusiness(MemoryCache);
                return _pensionFundBusiness;
            }
        }

        protected PensionFundOptionBusiness PensionFundOptionBusiness
        {
            get
            {
                if (_pensionFundOptionBusiness == null)
                    _pensionFundOptionBusiness = new PensionFundOptionBusiness(MemoryCache);
                return _pensionFundOptionBusiness;
            }
        }

        protected SmartContractBusiness SmartContractBusiness
        {
            get
            {
                if (_smartContractBusiness == null)
                    _smartContractBusiness = new SmartContractBusiness(MemoryCache);
                return _smartContractBusiness;
            }
        }

        protected PensionFundTransactionBusiness PensionFundTransactionBusiness
        {
            get
            {
                if (_pensionFundTransactionBusiness == null)
                    _pensionFundTransactionBusiness = new PensionFundTransactionBusiness(MemoryCache);
                return new PensionFundTransactionBusiness(MemoryCache);
            }
        }

        protected PensionFundContractBusiness PensionFundContractBusiness
        {
            get
            {
                if (_pensionFundContractBusiness == null)
                    _pensionFundContractBusiness = new PensionFundContractBusiness(MemoryCache);
                return _pensionFundContractBusiness;
            }
        }

        protected PensionFundReferenceContractBusiness PensionFundReferenceContractBusiness
        {
            get
            {
                if (_pensionFundReferenceContractBusiness == null)
                    _pensionFundReferenceContractBusiness = new PensionFundReferenceContractBusiness(MemoryCache);
                return _pensionFundReferenceContractBusiness;
            }
        }

        protected WalletBusiness WalletBusiness
        {
            get
            {
                if (_walletBusiness == null)
                    _walletBusiness = new WalletBusiness(MemoryCache);
                return _walletBusiness;
            }
        }

        protected EmployeeBusiness EmployeeBusiness
        {
            get
            {
                if (_employeeBusiness == null)
                    _employeeBusiness = new EmployeeBusiness(MemoryCache);
                return _employeeBusiness;
            }
        }

        protected CompanyBusiness CompanyBusiness
        {
            get
            {
                if (_companyBusiness == null)
                    _companyBusiness = new CompanyBusiness(MemoryCache);
                return _companyBusiness;
            }
        }

        protected BonusDistributionBusiness BonusDistributionBusiness
        {
            get
            {
                if (_bonusDistributionBusiness == null)
                    _bonusDistributionBusiness = new BonusDistributionBusiness(MemoryCache);
                return new BonusDistributionBusiness(MemoryCache);
            }
        }

        protected UserBusiness UserBusiness
        {
            get
            {
                if (_userBusiness == null)
                    _userBusiness = new UserBusiness(MemoryCache);
                return _userBusiness;
            }
        }

        protected ReferenceContractBusiness ReferenceContractBusiness
        {
            get
            {
                if (_referenceContractBusiness == null)
                    _referenceContractBusiness = new ReferenceContractBusiness(MemoryCache);
                return _referenceContractBusiness;
            }
        }
    }
}
