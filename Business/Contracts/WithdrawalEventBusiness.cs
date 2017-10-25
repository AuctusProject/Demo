using Auctus.DataAccess.Contracts;
using Auctus.DomainObjects.Contracts;
using Auctus.DomainObjects.Funds;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Auctus.EthereumProxy;
using Microsoft.Extensions.Logging;
using System.Threading;
using Auctus.Model;

namespace Auctus.Business.Contracts
{
    public class WithdrawalEventBusiness : BaseBusiness<WithdrawalEvent, WithdrawalEventData>
    {
        public WithdrawalEventBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        internal void Save(int pensionFundTransactionId, WithdrawalInfo withdrawalInfo)
        {
            var withdrawalEvent = new WithdrawalEvent()
            {
                PensionFundTransactionId = pensionFundTransactionId,
                EmployeeAbsoluteBonus = withdrawalInfo.EmployeeAbsoluteBonus,
                EmployeeBonus = withdrawalInfo.EmployeeBonus,
                EmployeeSzaboCashback = withdrawalInfo.EmployeeSzaboCashback,
                EmployeeTokenCashback = withdrawalInfo.EmployeeTokenCashback,
                EmployerSzaboCashback = withdrawalInfo.EmployerSzaboCashback,
                EmployerTokenCashback = withdrawalInfo.EmployerTokenCashback,
                Period = withdrawalInfo.Period,
                Responsable = withdrawalInfo.Responsable

            };
            Data.Insert(withdrawalEvent);
        }

        private List<WithdrawalEvent> List(string contractAddress)
        {
            return Data.List(contractAddress);
        }

        internal WithdrawalEvent Get(string contractAddress)
        {
            return List(contractAddress).FirstOrDefault();
        }
    }
}
