using System;

namespace Auctus.Web.Model.Home
{
    public class Fund
    {
        public String Name { get; set; }
        public Decimal Fee { get; set; }
        public Decimal LatePaymentFee { get; set; }
    }
}

