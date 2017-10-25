using Auctus.DomainObjects.Contracts;
using Auctus.Util.DapperAttributes;
using Auctus.DomainObjects.Unprocessed;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Unprocessed
{
    public class UEmployeeData : BaseData<UEmployee>
    {
        public override string TableName => "UEmployee";
    }
}
