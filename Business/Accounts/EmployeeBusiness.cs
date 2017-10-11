using Auctus.DataAccess.Accounts;
using Auctus.DomainObjects.Accounts;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Accounts
{
    public class EmployeeBusiness : BaseBusiness<Employee, EmployeeData>
    {
        public EmployeeBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        internal static void Validate(Model.Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException("employee");
            if (employee.ContributionPercentage <= 0)
                throw new ArgumentException("Contribution Percentage must be greater then zero.");
            if (employee.ContributionPercentage > 100)
                throw new ArgumentException("Contribution Percentage must be lesser or equal to 100.");
            if (employee.Salary <= 0)
                throw new ArgumentException("Salary must be greater then zero.");
            if (employee.Salary > 10000)
                throw new ArgumentException("Salary must be lesser or equal to 10000.");
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
