using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class Withdrawal : BaseTransaction
    {
        public double? EmployeeBonus { get; set; }
        public double? EmployeeAbsoluteBonus { get; set; }
        public double? EmployerTokenCashback { get; set; }
        public double? EmployeeTokenCashback { get; set; }
        public double? EmployerSzaboCashback { get; set; }
        public double? EmployeeSzaboCashback { get; set; }
        public bool Completed { get { return BlockNumber.HasValue; } }
    }
}
