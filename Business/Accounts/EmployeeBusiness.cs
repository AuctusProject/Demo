using Auctus.DataAccess.Accounts;
using Auctus.DomainObjects.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Accounts
{
    public class EmployeeBusiness : BaseBusiness<Employee, EmployeeData>
    {
        internal static void Validate(Model.Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException("employee");
            if (employee.ContributionPercentage < 0 || employee.ContributionPercentage > 100)
                throw new ArgumentException("Contribution Percentage should be a value between 0 and 100.");
            if (employee.Salary > 100)
                throw new ArgumentException("Salary should be greather than 0.");
        }
    }
}
