using Auctus.DomainObjects.Security;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Funds
{
    public class PensionFund
    {
        [DapperKey]
        public Int32 Id { get; set; }
        public String Name { get; set; }

        public IEnumerable<PensionFundOption> Options { get; set; }
    }
}
