using System;
using System.Collections.Generic;
using System.Text;

namespace DomainObjects.Accounts
{
    public class Employee
    {
        public Int32 Id { get; set; }
        public Int32 CompanyId { get; set; }
        public String Name { get; set; }
    }
}
