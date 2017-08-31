using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Auctus.Util.DapperAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DapperTypeAttribute : Attribute
    {
        public DbType DbType { get; private set; }

        public DapperTypeAttribute(DbType dbType)
        {
            DbType = dbType;
        }
    }
}
