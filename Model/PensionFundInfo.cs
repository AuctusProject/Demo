using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class PensionFundInfo
    {
        public String PensionFundName { get; set; }
        public String CompanyName { get; set; }
        public String EmployeeName { get; set; }
        public String PensionFundAddress { get; set; }
        public String CompanyAddress { get; set; }
        public String EmployeeAddress { get; set; }
        public Double PensionFundFee { get; set; }
        public Double AuctusFee { get; set; }
        public Double PensionFundLatePenalty { get; set; }
        public String ContractTransactionHash { get; set; }
        public String ContractAddress { get; set; }
        public Int32 ContractBlockNumber { get; set; }
        public Double EmployeeBaseContribution { get; set; }
        public Double CompanyBaseContribution { get; set; }
        public IEnumerable<Asset> Assets { get; set; }
        public IEnumerable<AssetsReferenceValue> AssetsReferenceValue { get; set; }
        public Withdrawal Withdrawal { get; set; }
        public Progress Progress { get; set; }
    }

    public class Asset
    {
        public String Address { get; set; }
        public String Name { get; set; }
        public Double Percentage { get; set; }
    }

    public class AssetsReferenceValue
    {
        public Int32 Period { get; set; }
        public Double Value { get; set; }
    }
}
