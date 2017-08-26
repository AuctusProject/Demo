using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.EthereumProxy
{
    public class SolcException : Exception
    {
        public SolcException(string message) : base(message)
        { }
    }
}
