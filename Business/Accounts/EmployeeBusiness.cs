using Auctus.DataAccess.Accounts;
using Auctus.DomainObjects.Accounts;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Accounts
{
    public class EmployeeBusiness : BaseBusiness<Employee, EmployeeData>
    {
        public EmployeeBusiness(Cache cache) : base(cache) { }

        internal static void Validate(Model.Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException("employee");
            if (String.IsNullOrWhiteSpace(employee.Name))
                throw new ArgumentNullException("employee.Name");
            if (employee.ContributionPercentage < 0 || employee.ContributionPercentage > 100)
                throw new ArgumentException("Contribution Percentage should be a value between 0 and 100.");
            if (employee.Salary < 0 || employee.Salary > 10000)
                throw new ArgumentException("Salary should be a value bewteen 0 and 10000.");
        }

        internal Employee Create(String address, String name, double salary, double contribution, String companyAddress)
        {
            var employee = new Employee
            {
                Address = address,
                Name = name,
                Salary = salary,
                Contribution = contribution,
                CompanyAddress = companyAddress
            };
            Insert(employee);
            return employee;
        }
    }
}
