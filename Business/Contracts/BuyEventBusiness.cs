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
    public class BuyEventBusiness : BaseBusiness<BuyEvent, BuyEventData>
    {
        public BuyEventBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        internal void Save(int pensionFundTransactionId, BuyInfo buyInfo)
        {
            var buyEvent = new BuyEvent()
            {
                AuctusFee = buyInfo.AuctusFee,
                DaysOverdue = buyInfo.DaysOverdue,
                LatePenalty = buyInfo.LatePenalty,
                PensionFundFee = buyInfo.PensionFundFee,
                PensionFundTransactionId = pensionFundTransactionId,
                Period = buyInfo.Period,
                Responsable = buyInfo.Responsable,
                SzaboInvested = buyInfo.SzaboInvested,
                TokenAmount = buyInfo.TokenAmount

            };
            Insert(buyEvent);
        }

        internal List<BuyEvent> List(string contractHash)
        {
            return Data.List(contractHash);
        }
    }
}
