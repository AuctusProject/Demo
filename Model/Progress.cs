using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class Progress
    {
        public double CurrentVestingBonus { get; set; }
        public double NextVestingBonus { get; set; }
        public DateTime NextVestingDate { get; set; }
        public DateTime StartTime { get; set; }
        public int LastPeriod { get; set; }
        public List<ProgressValue> Values { get; set; }
        public List<TransactionHistory> TransactionHistory { get; set; }
    }

    public class ProgressValue
    {
        public int Period { get; set; }
        public DateTime Date { get; set; }
        public double Invested { get; set; }
        public double Vested { get; set; }
        public double Total { get; set; }
        public double PensinonFundFee { get; set; }
        public double AuctusFee { get; set; }
        public double Token { get; set; }
    }

    public class TransactionHistory
    {
        public DateTime CreationDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string CompanyTransactionHash { get; set; }
        public int? CompanyBlockNumber { get; set; }
        public string EmployeeTransactionHash { get; set; }
        public int? EmployeeBlockNumber { get; set; }
        public double? EmployeeToken { get; set; }
        public double? CompanyToken { get; set; }
    }
}
