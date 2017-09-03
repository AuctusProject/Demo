using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.EthereumProxy
{
    public class PoolInfo
    {
        public List<string> Pending { get; set; } = new List<string>();
        public List<string> Queued { get; set; } = new List<string>();
    }
}
