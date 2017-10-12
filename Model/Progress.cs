using System;
using System.Collections.Generic;
using System.Linq;

namespace Auctus.Model
{
    public class Progress
    {
        public double TotalInvested { get; set; } = 0;
        public double TotalVested { get; set; } = 0;
        public double TotalToken { get; set; } = 0;
        public double TotalPensinonFundFee { get; set; } = 0;
        public double TotalAuctusFee { get; set; } = 0;
        public double CurrentVestingBonus { get; set; } = 0;
        public double NextVestingBonus { get; set; } = 0;
        public string NextVestingDate { get; set; }
        public long StartTime { get; set; }
        public int LastPeriod { get; set; }
        public List<ProgressValue> Values { get; set; }
        public List<TransactionHistory> TransactionHistory { get; set; }

        public bool Completed
        {
            get
            {
                return !TransactionHistory.Any(c => !c.CompanyBlockNumber.HasValue || !c.EmployeeBlockNumber.HasValue);
            }
        }
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
        public string PaymentDate { get; set; }
        public string CompanyTransactionHash { get; set; }
        public int? CompanyBlockNumber { get; set; }
        public string EmployeeTransactionHash { get; set; }
        public int? EmployeeBlockNumber { get; set; }
        public double? EmployeeToken { get; set; }
        public double? CompanyToken { get; set; }

        public string Status
        {
            get
            {
                if (CompanyBlockNumber.HasValue && EmployeeBlockNumber.HasValue)
                {
                    return "Confirmed";
                }
                return "Pending";
            }
        }
    }
}
