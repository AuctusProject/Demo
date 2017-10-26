using Auctus.DomainObjects.Contracts;
using Auctus.Util.DapperAttributes;
using Auctus.DomainObjects.Unprocessed;
using System;
using System.Collections.Generic;
using System.Text;
using Auctus.DataAccess.Unprocessed;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using Auctus.Model;

namespace Auctus.Business.Unprocessed
{
    public class UEmployeeBusiness : BaseBusiness<UEmployee, UEmployeeData>
    {
        public UEmployeeBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache)
        {
        }

        internal UEmployee Create(Employee employee, int uCompanyId)
        {
            var uEmployee = new UEmployee();
            uEmployee.Name = employee.Name;
            uEmployee.Salary = employee.Salary;
            uEmployee.Contribution = employee.ContributionPercentage;
            uEmployee.UCompanyId = uCompanyId;
            Insert(uEmployee);
            return uEmployee;
        }
    }
}
