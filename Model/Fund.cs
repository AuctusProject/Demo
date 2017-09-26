using System;
using System.Collections.Generic;

namespace Auctus.Model
{
    public class Fund
    {
        public String Name { get; set; }
        public Double Fee { get; set; }
        public Double LatePaymentFee { get; set; }
        public Double GoldPercentage { get; set; }
        public Double SPPercentage { get; set; }
        public Double BONDSPercentage { get; set; }
        public Double MSCIPercentage { get; set; }
        public Double BitcoinPercentage { get; set; }
    }
}

