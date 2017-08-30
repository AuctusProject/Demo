using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.EthereumProxy
{
    public class Transaction
    {
        [JsonProperty(PropertyName = "contractAddress")]
        public string ContractAddress { get; set; }
        [JsonProperty(PropertyName = "blockNumber")]
        public int BlockNumber { get; set; }
        [JsonProperty(PropertyName = "transactionHash")]
        public string TransactionHash { get; set; }
        [JsonProperty(PropertyName = "from")]
        public string From { get; set; }
        [JsonProperty(PropertyName = "gasUsed")]
        public int GasUsed { get; set; }
    }
}
