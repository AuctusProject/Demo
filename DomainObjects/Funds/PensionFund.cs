using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Funds
{
    public class PensionFund
    {
        public Int32 Id { get; set; }
        public Int32 OwnerUserId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
    }
}
