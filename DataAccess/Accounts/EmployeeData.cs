using Auctus.DomainObjects.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Accounts
{
    public class EmployeeData : BaseData<Employee>
    {
        public override string TableName => "Employee"; 
    }
}
