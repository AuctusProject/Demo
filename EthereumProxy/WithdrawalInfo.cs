using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.EthereumProxy
{
    public class WithdrawalInfo : BaseEventInfo
    {
        public int Period { get; set; }
        public string Responsable { get; set; }
        public double EmployeeBonus { get; set; }
        public double EmployeeAbsoluteBonus { get; set; }
        public double EmployerTokenCashback { get; set; }
        public double EmployeeTokenCashback { get; set; }
        public double EmployerSzaboCashback { get; set; }
        public double EmployeeSzaboCashback { get; set; }
    }
}
