using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class BaseTransaction
    {
        public DateTime CreatedDate { get; set; }
        public string TransactionHash { get; set; }
        public int? BlockNumber { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public int? Period { get; set; }
        public string Responsable { get; set; }
    }
}
