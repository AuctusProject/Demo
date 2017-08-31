using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Accounts
{
    public class Wallet
    {
        [DapperKey]
        [DapperType(System.Data.DbType.AnsiStringFixedLength)]
        public String Address { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime CreationDate { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public String Password { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public String FileName { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public String File { get; set; }
    }
}