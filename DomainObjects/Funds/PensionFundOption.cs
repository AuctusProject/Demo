using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Funds
{
    public class PensionFundOption
    {
        public Int32 Id { get; set; }
        public Int32 PensionFundId { get; set; }
        public Int32 OptionType { get; set; }
        public Decimal Fee { get; set; }
        public String WalletAddress { get; set; }
    }
}
