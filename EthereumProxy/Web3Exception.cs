using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.EthereumProxy
{
    public class Web3Exception : Exception
    {
        public Web3Exception(string message) : base(message)
        { }
    }
}
