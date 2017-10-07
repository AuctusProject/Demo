using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class Progress
    {
        public double TotalInvested { get; set; }
        public double TotalVested { get; set; }
        public double TotalToken { get; set; }
        public double TotalPensinonFundFee { get; set; }
        public double TotalAuctusFee { get; set; }
        public double CurrentVestingBonus { get; set; }
        public double NextVestingBonus { get; set; }
        public string NextVestingDate { get; set; }
        public long StartTime { get; set; }
        public int LastPeriod { get; set; }
        public List<ProgressValue> Values { get; set; }
        public List<TransactionHistory> TransactionHistory { get; set; }
    }

    public class ProgressValue
    {
        public int Period { get; set; }
        public long Date { get; set; }
        public double Invested { get; set; }
        public double Vested { get; set; }
        public double Total { get; set; }
        public double PensinonFundFee { get; set; }
        public double AuctusFee { get; set; }
        public double Token { get; set; }
    }

    public class TransactionHistory
    {
        public long CreationDate { get; set; }
        public long? PaymentDate { get; set; }
        public string CompanyTransactionHash { get; set; }
        public int? CompanyBlockNumber { get; set; }
        public string EmployeeTransactionHash { get; set; }
        public int? EmployeeBlockNumber { get; set; }
        public double? EmployeeToken { get; set; }
        public double? CompanyToken { get; set; }
    }
}
