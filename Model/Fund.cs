using System;
using System.Collections.Generic;

namespace Auctus.Model
{
    public class Fund
    {
        public String Name { get; set; }
        public Double Fee { get; set; }
        public Double LatePaymentFee { get; set; }

        public IEnumerable<AssetAllocation> AssetAllocations { get; set; }
    }
}

