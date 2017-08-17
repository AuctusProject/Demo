using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class SmartContract
    {
        public Int32 Id { get; set; }
        public String Hash { get; set; }
        public byte[] ABI { get; set; }
    }
}
