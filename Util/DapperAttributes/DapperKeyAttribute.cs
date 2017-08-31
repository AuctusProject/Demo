using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Util.DapperAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DapperKeyAttribute : Attribute
    {
        public bool Identity { get; private set; }

        public DapperKeyAttribute(bool identity = false)
        {
            Identity = identity;
        }
    }
}
