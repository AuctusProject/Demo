using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.EthereumProxy
{
    public class BaseEventInfo
    {
        public string TransactionHash { get; set; }
        public int BlockNumber { get; set; }
    }
}
