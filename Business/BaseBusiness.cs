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
    public abstract class BaseBusiness<T, D> where D : BaseData<T> , new()
    {
        protected readonly Cache MemoryCache;
        protected D Data => new D();

        private PensionFundBusiness _pensionFundBusiness;
        private PensionFundOptionBusiness _pensionFundOptionBusiness;
        private SmartContractBusiness _smartContractBusiness;
        private PensionFundTransactionBusiness _pensionFundTransactionBusiness;
        private PensionFundContractBusiness _pensionFundContractBusiness;
        private WalletBusiness _walletBusiness;
        private EmployeeBusiness _employeeBusiness;
        private CompanyBusiness _companyBusiness;
        private BonusDistributionBusiness _bonusDistributionBusiness;
        private UserBusiness _userBusiness;

        protected BaseBusiness(Cache cache)
        {
            MemoryCache = cache;
        }

        public IEnumerable<T> ListAll()
        {
            return Data.SelectAll<T>();
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

        protected PensionFundBusiness PensionFundBusiness { get { return _pensionFundBusiness ?? new PensionFundBusiness(MemoryCache); } }
        protected PensionFundOptionBusiness PensionFundOptionBusiness { get { return _pensionFundOptionBusiness ?? new PensionFundOptionBusiness(MemoryCache); } }
        protected SmartContractBusiness SmartContractBusiness { get { return _smartContractBusiness ?? new SmartContractBusiness(MemoryCache); } }
        protected PensionFundTransactionBusiness PensionFundTransactionBusiness { get { return _pensionFundTransactionBusiness ?? new PensionFundTransactionBusiness(MemoryCache); } }
        protected PensionFundContractBusiness PensionFundContractBusiness { get { return _pensionFundContractBusiness ?? new PensionFundContractBusiness(MemoryCache); } }
        protected WalletBusiness WalletBusiness { get { return _walletBusiness ?? new WalletBusiness(MemoryCache); } }
        protected EmployeeBusiness EmployeeBusiness { get { return _employeeBusiness ?? new EmployeeBusiness(MemoryCache); } }
        protected CompanyBusiness CompanyBusiness { get { return _companyBusiness ?? new CompanyBusiness(MemoryCache); } }
        protected BonusDistributionBusiness BonusDistributionBusiness { get { return _bonusDistributionBusiness ?? new BonusDistributionBusiness(MemoryCache); } }
        protected UserBusiness UserBusiness { get { return _userBusiness ?? new UserBusiness(MemoryCache); } }
    }
}
