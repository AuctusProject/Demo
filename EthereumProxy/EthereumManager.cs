using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static Auctus.EthereumProxy.Solc;
using static Auctus.EthereumProxy.Web3;
using System.Reflection;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Globalization;

namespace Auctus.EthereumProxy
{
    public class EthereumManager
    {
        internal const int GWEI_FAST = 21;
        internal const int GWEI_NORMAL = 7;

        public static Wallet CreateAccount(string encryptedPassword)
        {
            return Web3.CreateAccount(encryptedPassword);
        }

        public static Transaction GetTransaction(string transactionHash)
        {
            return Web3.GetTransaction(transactionHash);
        }

        public static bool IsValidAddress(string address)
        {
            return Web3.IsValidAddress(address);
        }

        public static PoolInfo GetPoolInfo()
        {
            return Web3.GetPoolInfo();
        }

        public static KeyValuePair<string, string> DeployDefaultPensionFund(
            int gasLimit,
            string pensionFundAddress, 
            string employerAddress, 
            string employeeAddress, 
            double pensionFundFee,
            double pensionFundLatePenalty,
            double auctusFee,
            double maxSalaryBonus,
            double employeeContribution,
            double employeeContributionBonus,
            double employeeSalary,
            Dictionary<string, double> referenceValues,
            Dictionary<int, double> bonusVestingDistribuition)
        {
            int rateDecimals = 4;
            string smartContractStringified = DemoSmartContracts.DEFAULT_PENSION_FUND_SC
                .Replace("{PENSION_FUND_ADDRESS}", pensionFundAddress)
                .Replace("{EMPLOYER_ADDRESS}", employerAddress)
                .Replace("{EMPLOYEE_ADDRESS}", employeeAddress)
                .Replace("{PENSION_FUND_FEE}", Web3.GetNumberFormatted(pensionFundFee, rateDecimals))
                .Replace("{PENSION_FUND_LATE_PENALTY}", Web3.GetNumberFormatted(pensionFundLatePenalty, rateDecimals))
                .Replace("{AUCTUS_FEE}", Web3.GetNumberFormatted(auctusFee, rateDecimals))
                .Replace("{MAX_SALARY_BONUS}", Web3.GetNumberFormatted(maxSalaryBonus, rateDecimals))
                .Replace("{EMPLOYEE_CONTRIBUTION}", Web3.GetNumberFormatted(employeeContribution, rateDecimals))
                .Replace("{EMPLOYEE_CONTRIBUTION_BONUS}", Web3.GetNumberFormatted(employeeContributionBonus, rateDecimals))
                .Replace("{EMPLOYEE_SALARY}", Web3.GetNumberFormatted(employeeSalary, 12))
                .Replace("{REFERENCE_VALUE_ADDRESS}", string.Join("\n\t\t", referenceValues.Select(c => string.Format("reference.push(Reference({0}, {1}));", c.Key, Web3.GetNumberFormatted(c.Value, rateDecimals)))))
                .Replace("{BONUS_DISTRIBUTION}", string.Join("\n\t\t", bonusVestingDistribuition.Select(c => string.Format("bonusDistribution.push(BonusVesting({0}, {1}));", c.Key.ToString(), Web3.GetNumberFormatted(c.Value, rateDecimals)))));

            SCCompiled scCompiled = Solc.Compile("CompanyContract", smartContractStringified).Single(c => c.Name == "CompanyContract");
            string transactionHash = Web3.DeployContract(scCompiled, gasLimit, GWEI_FAST);
            return new KeyValuePair<string, string>(transactionHash, smartContractStringified);
        }

        public static string WithdrawalFromDefaultPensionFund(string employeeAddress, string smartContractAddress, string abi, int gasLimit)
        {
            WithdrawalInfo withdrawalInfo = GetWithdrawalInfo(employeeAddress, smartContractAddress, abi);
            double szabo = withdrawalInfo.EmployeeSzaboCashback + withdrawalInfo.EmployerSzaboCashback + 0.0000000001;
            return Web3.CallFunction(smartContractAddress, abi, "sell", new BigNumber(szabo, 12), gasLimit, GWEI_NORMAL, 
                default(KeyValuePair<string, string>), new Variable(VariableType.Address, employeeAddress));
        }

        public static WithdrawalInfo GetWithdrawalInfo(string employeeAddress, string smartContractAddress, string abi)
        {
            List<CompleteVariableType> returnTypes = new List<CompleteVariableType>();
            returnTypes.Add(new CompleteVariableType(VariableType.BigNumber, 4));
            returnTypes.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            returnTypes.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            returnTypes.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            returnTypes.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            returnTypes.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            List<Variable> withdrawalValues = Web3.CallConstFunction(returnTypes, smartContractAddress, abi, "getWithdrawalValues", new Variable(VariableType.Address, employeeAddress));
            return new WithdrawalInfo()
            {
                EmployeeBonus = ((BigNumber)withdrawalValues.ElementAt(0).Value).Value,
                EmployeeAbsoluteBonus = ((BigNumber)withdrawalValues.ElementAt(1).Value).Value,
                EmployerTokenCashback = ((BigNumber)withdrawalValues.ElementAt(2).Value).Value,
                EmployeeTokenCashback = ((BigNumber)withdrawalValues.ElementAt(3).Value).Value,
                EmployerSzaboCashback = ((BigNumber)withdrawalValues.ElementAt(4).Value).Value,
                EmployeeSzaboCashback = ((BigNumber)withdrawalValues.ElementAt(5).Value).Value
            };
        }

        public static string EmployeeBuyFromDefaultPensionFund(string employeeAddress, string smartContractAddress, string abi, int daysOverdue, int gasLimit)
        {
            return BuyDefaultPensionFund(employeeAddress, smartContractAddress, abi, gasLimit, daysOverdue, "getEmployeeInvestment", "employeeInvestment");
        }

        public static string EmployerBuyFromDefaultPensionFund(string employeeAddress, string smartContractAddress, string abi, int daysOverdue, int gasLimit)
        {
            return BuyDefaultPensionFund(employeeAddress, smartContractAddress, abi, gasLimit, daysOverdue, "getEmployerInvestment", "employerInvestment");
        }

        public static List<BuyInfo> ReadBuyFromDefaultPensionFund(string smartContractAddress)
        {
            List<CompleteVariableType> buyEventParameters = new List<CompleteVariableType>();
            buyEventParameters.Add(new CompleteVariableType(VariableType.Number, 0, true));
            buyEventParameters.Add(new CompleteVariableType(VariableType.Address, 0, true));
            buyEventParameters.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            buyEventParameters.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            buyEventParameters.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            buyEventParameters.Add(new CompleteVariableType(VariableType.Number));
            buyEventParameters.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            buyEventParameters.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            List<Event> buyEvents = Web3.ReadEvent(smartContractAddress, "Buy", buyEventParameters);

            List<BuyInfo> buyInfo = new List<BuyInfo>();
            foreach (Event e in buyEvents)
            {
                buyInfo.Add(new BuyInfo()
                {
                    TransactionHash = e.TransactionHash,
                    BlockNumber = e.BlockNumber,
                    Period = Convert.ToInt32(e.Data[0].Value),
                    Responsable = (string)e.Data[1].Value,
                    TokenAmount = ((BigNumber)e.Data[2].Value).Value,
                    SzaboInvested = ((BigNumber)e.Data[3].Value).Value,
                    LatePenalty = ((BigNumber)e.Data[4].Value).Value,
                    DaysOverdue = Convert.ToInt32(e.Data[5].Value),
                    PensionFundFee = ((BigNumber)e.Data[6].Value).Value,
                    AuctusFee = ((BigNumber)e.Data[7].Value).Value
                });
            }
            return buyInfo;
        }

        public static WithdrawalInfo ReadWithdrawalFromDefaultPensionFund(string smartContractAddress)
        {
            List<CompleteVariableType> withdrawalEventParameters = new List<CompleteVariableType>();
            withdrawalEventParameters.Add(new CompleteVariableType(VariableType.Number, 0, true));
            withdrawalEventParameters.Add(new CompleteVariableType(VariableType.Address, 0, true));
            withdrawalEventParameters.Add(new CompleteVariableType(VariableType.BigNumber, 4));
            withdrawalEventParameters.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            withdrawalEventParameters.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            withdrawalEventParameters.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            withdrawalEventParameters.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            withdrawalEventParameters.Add(new CompleteVariableType(VariableType.BigNumber, 12));
            Event withdrawalEvent = Web3.ReadEvent(smartContractAddress, "Withdrawal", withdrawalEventParameters).SingleOrDefault();

            return withdrawalEvent == null ? null : new WithdrawalInfo()
            {
                TransactionHash = withdrawalEvent.TransactionHash,
                BlockNumber = withdrawalEvent.BlockNumber,
                Period = Convert.ToInt32(withdrawalEvent.Data[0].Value),
                Responsable = (string)withdrawalEvent.Data[1].Value,
                EmployeeBonus = ((BigNumber)withdrawalEvent.Data[2].Value).Value,
                EmployeeAbsoluteBonus = ((BigNumber)withdrawalEvent.Data[3].Value).Value,
                EmployerTokenCashback = ((BigNumber)withdrawalEvent.Data[4].Value).Value,
                EmployeeTokenCashback = ((BigNumber)withdrawalEvent.Data[5].Value).Value,
                EmployerSzaboCashback = ((BigNumber)withdrawalEvent.Data[6].Value).Value,
                EmployeeSzaboCashback = ((BigNumber)withdrawalEvent.Data[7].Value).Value
            };
        }
        
        private static string BuyDefaultPensionFund(string employeeAddress, string smartContractAddress, string abi, int gasLimit, int daysOverdue, 
            string getRequiredValueMethod, string buyMethod)
        {
            Variable requiredValue = Web3.CallConstFunction(new CompleteVariableType(VariableType.BigNumber), smartContractAddress, abi, getRequiredValueMethod,
                new Variable(VariableType.Address, employeeAddress), new Variable(VariableType.Number, daysOverdue));

            return Web3.CallFunction(smartContractAddress, abi, buyMethod, Web3.ETHER(((BigNumber)requiredValue.Value).Value), gasLimit, GWEI_NORMAL, 
                default(KeyValuePair<string, string>), new Variable(VariableType.Address, employeeAddress), new Variable(VariableType.Number, daysOverdue));
        }
        
        internal static int GetGweiPrice()
        {
            int gwei = 21;
            //try
            //{
            //    using (HttpClient client = new HttpClient())
            //    {
            //        client.Timeout = new TimeSpan(0, 0, 0, 2);
            //        using (HttpResponseMessage response = client.GetAsync(Config.URL_GAS_PRICE).Result)
            //        {
            //            if (response.IsSuccessStatusCode)
            //            {
            //                GasPrice result = Newtonsoft.Json.JsonConvert.DeserializeObject<GasPrice>(response.Content.ReadAsStringAsync().Result);
            //                if (result != null)
            //                    gwei = (int)(System.Numerics.BigInteger.Parse("0" + result.Price.Substring(2), NumberStyles.HexNumber) / 1000000000);
            //            }
            //        }
            //    }
            //}
            //catch { }
            return gwei;
        }

        private class GasPrice
        {
            [JsonProperty(PropertyName = "result")]
            public string Price { get; set; }
        }
    }
}
