using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.EthereumProxy
{
    public class Wallet
    {
        public string Address { get; set; }
        public string FileName { get; set; }
        public byte[] File { get; set; }
    }
}
