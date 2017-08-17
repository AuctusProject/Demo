using Auctus.DomainObjects.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Security
{
    public class UserData : BaseData<User>
    {
        public override string TableName => "User";
    }
}
