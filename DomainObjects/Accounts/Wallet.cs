using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Accounts
{
    public class Wallet
    {
        public String Address { get; set; }
        public DateTime CreationDate { get; set; }
        public String Password { get; set; }
        public String FileName { get; set; }
        public String File { get; set; }
    }
}