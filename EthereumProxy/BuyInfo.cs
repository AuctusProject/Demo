using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.EthereumProxy
{
    public class BuyInfo : BaseEventInfo
    {
        public int Period { get; set; }
        public string Responsable { get; set; }
        public double TokenAmount { get; set; }
        public double SzaboInvested { get; set; }
        public double LatePenalty { get; set; }
        public int DaysOverdue { get; set; }
        public double PensionFundFee { get; set; }
        public double AuctusFee { get; set; }
    }
}
