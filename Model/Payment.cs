using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class Payment : BaseTransaction
    {
        public double? TokenAmount { get; set; }
        public double? SzaboInvested { get; set; }
        public double? LatePenalty { get; set; }
        public int? DaysOverdue { get; set; }
        public double? PensionFundFee { get; set; }
        public double? AuctusFee { get; set; }
    }
}
