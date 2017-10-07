using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static Auctus.EthereumProxy.Solc;
using static Auctus.EthereumProxy.Web3;
using Auctus.Util.NotShared;

namespace Auctus.EthereumProxy
{
    public class Test
    {
        #region Methods for Web Api simple Test
        private static string smartContractABI;
        public static string DeployContratc(string owner, string password, int gasLimit, int gwei)
        {
            try
            {
                SCCompiled compiled = Solc.Compile("Test", smartContractStringified).Single(c => c.Name == "Test");
                smartContractABI = compiled.ABI;
                return Web3.DeployContract(compiled, gasLimit, gwei, new KeyValuePair<string, string>(owner, password));
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static double GetBalance(string address)
        {
            return Web3.GetBalance(address);
        }

        public static string Send(string from, string password, string to, double value, int gasLimit, int gwei)
        {
            try
            {
                return Web3.Send(to, Web3.ETHER(value), gasLimit, gwei, new KeyValuePair<string, string>(from, password));
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static double GetBalanceFromSmartContract(string from, string smartContractAddress)
        {
            return ((BigNumber)Web3.CallConstFunction(new CompleteVariableType(VariableType.BigNumber, 0), smartContractAddress, smartContractABI, "balanceOf", new Variable(VariableType.Address, from)).Value).Value;
        }

        public static string DrainFromContract(string owner, string password, int gasLimit, int gwei, string smartContractAddress)
        {
            try
            {
                return Web3.CallFunction(smartContractAddress, smartContractABI, "drain", Web3.ETHER(0), gasLimit, gwei, new KeyValuePair<string, string>(owner, password));
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static TransactionInfo GetTransactionInformation(string transactionHash)
        {
            try
            {
                Transaction trans = Web3.GetTransaction(transactionHash, 5);
                if (trans != null)
                {
                    return new TransactionInfo()
                    {
                        BlockNumber = trans.BlockNumber,
                        ContractAddress = trans.ContractAddress,
                        From = trans.From,
                        GasUsed = trans.GasUsed,
                        TransactionHash = trans.TransactionHash
                    };
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                return new TransactionInfo() { ContractAddress = e.ToString() };
            }
        }

        [Serializable]
        public class TransactionInfo
        {
            public string ContractAddress { get; set; }
            public int BlockNumber { get; set; }
            public string TransactionHash { get; set; }
            public string From { get; set; }
            public int GasUsed { get; set; }
        }

        [Serializable]
        public class WalletInfo
        {
            public string Address { get; set; }
            public string FileName { get; set; }
            public byte[] File { get; set; }
        }
        #endregion

        #region Deploy Assets

        #region Assets Contracts
        private const string scGold = @"pragma solidity ^0.4.13;


contract ReferenceValue {
	mapping(uint64 => uint256) public values;
	uint64 public pointsBetweenValues;

	function ReferenceValue() {
		pointsBetweenValues = 30;

		values[1] = 159391;
		values[2] = 162603;
		values[3] = 174445;
		values[4] = 174701;
		values[5] = 172114;
		values[6] = 168853;
		values[7] = 167095;
		values[8] = 162759;
		values[9] = 159286;
		values[10] = 148508;
		values[11] = 141350;
		values[12] = 134236;
		values[13] = 128672;
		values[14] = 134710;
		values[15] = 134880;
		values[16] = 131618;
		values[17] = 127582;
		values[18] = 122540;
		values[19] = 124480;
		values[20] = 130098;
		values[21] = 133608;
		values[22] = 129900;
		values[23] = 128753;
		values[24] = 127910;
		values[25] = 131097;
		values[26] = 129599;
		values[27] = 123882;
		values[28] = 122249;
		values[29] = 117630;
		values[30] = 120229;
		values[31] = 125185;
		values[32] = 122719;
		values[33] = 117863;
		values[34] = 119791;
		values[35] = 119905;
		values[36] = 118150;
		values[37] = 113004;
		values[38] = 111747;
		values[39] = 112453;
		values[40] = 115925;
		values[41] = 108570;
		values[42] = 106825;
		values[43] = 109737;
		values[44] = 119991;
		values[45] = 124634;
		values[46] = 124226;
		values[47] = 125940;
		values[48] = 127640;
		values[49] = 133733;
		values[50] = 134109;
		values[51] = 132603;
		values[52] = 126657;
		values[53] = 123598;
		values[54] = 115140;
		values[55] = 119262;
		values[56] = 123436;
		values[57] = 123109;
		values[58] = 126563;
		values[59] = 124500;
		values[60] = 126026;
        values[61] = 127241;
	}

	function getValueAt(uint64 period, uint64 daysOverdue) constant returns (uint256) {
        if(period < 1) {
            period = 1;
        } else if(period > 61) {
            period = 61;
        }
		uint256 value = getBaseValue(period);
		if(daysOverdue > 0) {
			uint256 nextValue = getBaseValue(period + 1);
			if(nextValue != value) {
				uint256 difference;
				if (nextValue > value) 
					difference = nextValue - value;
				else 
					difference = value - nextValue;

				uint256 weight = (difference * daysOverdue);
				if(weight > pointsBetweenValues) {
					if (nextValue > value) 
						value = value + (weight / pointsBetweenValues);
					else
						value = value - (weight / pointsBetweenValues);
				}
				else
					value = nextValue;
			}
		}
		return value;
	}
	
	function getBaseValue(uint64 period) constant internal returns (uint256) {
		return (values[1] * 1 szabo) / values[period];
	}
}";

        private const string scSP500 = @"pragma solidity ^0.4.13;


contract ReferenceValue {
	mapping(uint64 => uint256) public values;
	uint64 public pointsBetweenValues;

	function ReferenceValue() {
		pointsBetweenValues = 30;

		values[1] = 144067;
		values[2] = 141216;
		values[3] = 141618;
		values[4] = 142619;
		values[5] = 149811;
		values[6] = 151468;
		values[7] = 156919;
		values[8] = 159757;
		values[9] = 163074;
		values[10] = 160628;
		values[11] = 168573;
		values[12] = 163297;
		values[13] = 168155;
		values[14] = 175654;
		values[15] = 180581;
		values[16] = 184836;
		values[17] = 178259;
		values[18] = 185945;
		values[19] = 187234;
		values[20] = 188395;
		values[21] = 192357;
		values[22] = 196023;
		values[23] = 193067;
		values[24] = 200337;
		values[25] = 197229;
		values[26] = 201805;
		values[27] = 206756;
		values[28] = 205890;
		values[29] = 199499;
		values[30] = 210450;
		values[31] = 206789;
		values[32] = 208551;
		values[33] = 210739;
		values[34] = 206311;
		values[35] = 210384;
		values[36] = 197218;
		values[37] = 192003;
		values[38] = 207936;
		values[39] = 208041;
		values[40] = 204394;
		values[41] = 194024;
		values[42] = 193223;
		values[43] = 205974;
		values[44] = 206530;
		values[45] = 209695;
		values[46] = 209886;
		values[47] = 217360;
		values[48] = 217095;
		values[49] = 216827;
		values[50] = 212615;
		values[51] = 219881;
		values[52] = 223883;
		values[53] = 227887;
		values[54] = 236364;
		values[55] = 236272;
		values[56] = 238420;
		values[57] = 241180;
		values[58] = 242341;
		values[59] = 247030;
		values[60] = 244424;
        values[61] = 246139;
	}

	function getValueAt(uint64 period, uint64 daysOverdue) constant returns (uint256) {
        if(period < 1) {
            period = 1;
        } else if(period > 61) {
            period = 61;
        }
		uint256 value = getBaseValue(period);
		if(daysOverdue > 0) {
			uint256 nextValue = getBaseValue(period + 1);
			if(nextValue != value) {
				uint256 difference;
				if (nextValue > value) 
					difference = nextValue - value;
				else 
					difference = value - nextValue;

				uint256 weight = (difference * daysOverdue);
				if(weight > pointsBetweenValues) {
					if (nextValue > value) 
						value = value + (weight / pointsBetweenValues);
					else
						value = value - (weight / pointsBetweenValues);
				}
				else
					value = nextValue;
			}
		}
		return value;
	}
	
	function getBaseValue(uint64 period) constant internal returns (uint256) {
		return (values[1] * 1 szabo) / values[period];
	}
}";

        private const string scBTC = @"pragma solidity ^0.4.13;


contract ReferenceValue {
	mapping(uint64 => uint256) public values;
	uint64 public pointsBetweenValues;

	function ReferenceValue() {
		pointsBetweenValues = 30;

		values[1] = 1162;
		values[2] = 1173;
		values[3] = 1149;
		values[4] = 1336;
		values[5] = 1561;
		values[6] = 2604;
		values[7] = 5750;
		values[8] = 13034;
		values[9] = 11996;
		values[10] = 10782;
		values[11] = 8565;
		values[12] = 10363;
		values[13] = 12340;
		values[14] = 15072;
		values[15] = 53500;
		values[16] = 80174;
		values[17] = 85718;
		values[18] = 66216;
		values[19] = 59121;
		values[20] = 46023;
		values[21] = 48483;
		values[22] = 61234;
		values[23] = 61557;
		values[24] = 53535;
		values[25] = 44294;
		values[26] = 36153;
		values[27] = 36501;
		values[28] = 34050;
		values[29] = 24829;
		values[30] = 23390;
		values[31] = 26823;
		values[32] = 23497;
		values[33] = 23688;
		values[34] = 23799;
		values[35] = 27871;
		values[36] = 25039;
		values[37] = 23350;
		values[38] = 26513;
		values[39] = 34884;
		values[40] = 42452;
		values[41] = 41014;
		values[42] = 40349;
		values[43] = 41476;
		values[44] = 43452;
		values[45] = 46167;
		values[46] = 64348;
		values[47] = 66148;
		values[48] = 57874;
		values[49] = 60455;
		values[50] = 64116;
		values[51] = 72472;
		values[52] = 82478;
		values[53] = 91126;
		values[54] = 106438;
		values[55] = 112979;
		values[56] = 121832;
		values[57] = 188428;
		values[58] = 265755;
		values[59] = 253326;
		values[60] = 385360;
        values[61] = 390925;
	}

	function getValueAt(uint64 period, uint64 daysOverdue) constant returns (uint256) {
        if(period < 1) {
            period = 1;
        } else if(period > 61) {
            period = 61;
        }
		uint256 value = getBaseValue(period);
		if(daysOverdue > 0) {
			uint256 nextValue = getBaseValue(period + 1);
			if(nextValue != value) {
				uint256 difference;
				if (nextValue > value) 
					difference = nextValue - value;
				else 
					difference = value - nextValue;

				uint256 weight = (difference * daysOverdue);
				if(weight > pointsBetweenValues) {
					if (nextValue > value) 
						value = value + (weight / pointsBetweenValues);
					else
						value = value - (weight / pointsBetweenValues);
				}
				else
					value = nextValue;
			}
		}
		return value;
	}
	
	function getBaseValue(uint64 period) constant internal returns (uint256) {
		return (values[1] * 1 szabo) / values[period];
	}
}";

        private const string scMSCI = @"pragma solidity ^0.4.13;


contract ReferenceValue {
	mapping(uint64 => uint256) public values;
	uint64 public pointsBetweenValues;

	function ReferenceValue() {
		pointsBetweenValues = 30;

		values[1] = 131150;
		values[2] = 130152;
		values[3] = 131549;
		values[4] = 133850;
		values[5] = 140547;
		values[6] = 140518;
		values[7] = 143451;
		values[8] = 147614;
		values[9] = 147193;
		values[10] = 143355;
		values[11] = 150791;
		values[12] = 147274;
		values[13] = 154367;
		values[14] = 160286;
		values[15] = 162842;
		values[16] = 166107;
		values[17] = 159846;
		values[18] = 167540;
		values[19] = 167387;
		values[20] = 168774;
		values[21] = 171518;
		values[22] = 174342;
		values[23] = 171433;
		values[24] = 174867;
		values[25] = 169841;
		values[26] = 170809;
		values[27] = 173950;
		values[28] = 170967;
		values[29] = 167754;
		values[30] = 177286;
		values[31] = 174081;
		values[32] = 177840;
		values[33] = 177931;
		values[34] = 173561;
		values[35] = 176560;
		values[36] = 164543;
		values[37] = 158192;
		values[38] = 170580;
		values[39] = 169440;
		values[40] = 166279;
		values[41] = 156218;
		values[42] = 154717;
		values[43] = 164812;
		values[44] = 167080;
		values[45] = 167461;
		values[46] = 165323;
		values[47] = 172179;
		values[48] = 171952;
		values[49] = 172567;
		values[50] = 169092;
		values[51] = 171209;
		values[52] = 175122;
		values[53] = 179240;
		values[54] = 183870;
		values[55] = 185369;
		values[56] = 187828;
		values[57] = 191174;
		values[58] = 191643;
		values[59] = 196110;
		values[60] = 194628;
        values[61] = 197326;
	}

	function getValueAt(uint64 period, uint64 daysOverdue) constant returns (uint256) {
        if(period < 1) {
            period = 1;
        } else if(period > 61) {
            period = 61;
        }
		uint256 value = getBaseValue(period);
		if(daysOverdue > 0) {
			uint256 nextValue = getBaseValue(period + 1);
			if(nextValue != value) {
				uint256 difference;
				if (nextValue > value) 
					difference = nextValue - value;
				else 
					difference = value - nextValue;

				uint256 weight = (difference * daysOverdue);
				if(weight > pointsBetweenValues) {
					if (nextValue > value) 
						value = value + (weight / pointsBetweenValues);
					else
						value = value - (weight / pointsBetweenValues);
				}
				else
					value = nextValue;
			}
		}
		return value;
	}
	
	function getBaseValue(uint64 period) constant internal returns (uint256) {
		return (values[1] * 1 szabo) / values[period];
	}
}";

        private const string scVWEHX = @"pragma solidity ^0.4.13;


contract ReferenceValue {
	mapping(uint64 => uint256) public values;
	uint64 public pointsBetweenValues;

	function ReferenceValue() {
		pointsBetweenValues = 30;

		values[1] = 598;
		values[2] = 602;
		values[3] = 605;
		values[4] = 606;
		values[5] = 611;
		values[6] = 612;
		values[7] = 611;
		values[8] = 613;
		values[9] = 620;
		values[10] = 612;
		values[11] = 591;
		values[12] = 599;
		values[13] = 590;
		values[14] = 593;
		values[15] = 605;
		values[16] = 604;
		values[17] = 603;
		values[18] = 604;
		values[19] = 613;
		values[20] = 611;
		values[21] = 612;
		values[22] = 615;
		values[23] = 616;
		values[24] = 606;
		values[25] = 613;
		values[26] = 599;
		values[27] = 608;
		values[28] = 602;
		values[29] = 597;
		values[30] = 598;
		values[31] = 607;
		values[32] = 600;
		values[33] = 603;
		values[34] = 602;
		values[35] = 591;
		values[36] = 590;
		values[37] = 582;
		values[38] = 567;
		values[39] = 580;
		values[40] = 569;
		values[41] = 554;
		values[42] = 546;
		values[43] = 546;
		values[44] = 559;
		values[45] = 571;
		values[46] = 568;
		values[47] = 570;
		values[48] = 579;
		values[49] = 586;
		values[50] = 587;
		values[51] = 586;
		values[52] = 578;
		values[53] = 583;
		values[54] = 586;
		values[55] = 592;
		values[56] = 588;
		values[57] = 593;
		values[58] = 596;
		values[59] = 595;
		values[60] = 599;
        values[61] = 603;
	}

	function getValueAt(uint64 period, uint64 daysOverdue) constant returns (uint256) {
        if(period < 1) {
            period = 1;
        } else if(period > 61) {
            period = 61;
        }
		uint256 value = getBaseValue(period);
		if(daysOverdue > 0) {
			uint256 nextValue = getBaseValue(period + 1);
			if(nextValue != value) {
				uint256 difference;
				if (nextValue > value) 
					difference = nextValue - value;
				else 
					difference = value - nextValue;

				uint256 weight = (difference * daysOverdue);
				if(weight > pointsBetweenValues) {
					if (nextValue > value) 
						value = value + (weight / pointsBetweenValues);
					else
						value = value - (weight / pointsBetweenValues);
				}
				else
					value = nextValue;
			}
		}
		return value;
	}
	
	function getBaseValue(uint64 period) constant internal returns (uint256) {
		return (values[1] * 1 szabo) / values[period];
	}
}";
        #endregion

        public static Dictionary<string, string> DeployAssets(string account, string password)
        {
            KeyValuePair<string, string> owner = new KeyValuePair<string, string>(account, password);

            string hashGold = DeployAsset(scGold, owner);
            string hashSP500 = DeployAsset(scSP500, owner);
            string hashBTC = DeployAsset(scBTC, owner);
            string hashMSCI = DeployAsset(scMSCI, owner);
            string hashVWEHX = DeployAsset(scVWEHX, owner);

            Dictionary<string, string> result = new Dictionary<string, string>();
            result["gold"] = GetContractAddress(hashGold);
            result["sp500"] = GetContractAddress(hashSP500);
            result["btc"] = GetContractAddress(hashBTC);
            result["msci"] = GetContractAddress(hashMSCI);
            result["vwehx"] = GetContractAddress(hashVWEHX);

            return result;
        }

        private static string DeployAsset(string scStringified, KeyValuePair<string, string> owner)
        {
            SCCompiled sc = Solc.Compile("ReferenceValue", scStringified).Single(c => c.Name == "ReferenceValue");
            return Web3.DeployContract(sc, 2500000, EthereumManager.GetGweiPrice(), owner);
        }

        private static string GetContractAddress(string transactionHash)
        {
            Transaction trans = Web3.GetTransaction(transactionHash, 10);
            return trans?.ContractAddress;
        }
        #endregion

        #region Test 

        #region SC source
        private const string smartContractStringified = @"
        pragma solidity ^0.4.13;


        library SafeMath {
	        function times(uint256 x, uint256 y) internal returns (uint256) {
		        uint256 z = x * y;
		        assert(x == 0 || (z / x == y));
		        return z;
	        }
	
	        function divided(uint256 x, uint256 y) internal returns (uint256) {
		        assert(y != 0);
		        return x / y;
	        }
	
	        function plus(uint256 x, uint256 y) internal returns (uint256) {
		        uint256 z = x + y;
		        assert(z >= x && z >= y);
		        return z;
	        }
	
	        function minus(uint256 x, uint256 y) internal returns (uint256) {
		        assert(x >= y);
		        return x - y;
	        }
        }


        contract Test {
	        using SafeMath for uint256;
	
	        uint256 public tokensPerEther = 10;
	        uint256 public finishedCap = 200 ether;
	        address public owner;
	
	        mapping(address => uint256) public balances;
	        mapping(address => uint256) public invested; 
	
	        mapping(address => bool) public happy; 
	
	        uint256 private weiRaised = 0;
	        uint256 private distributedAmount = 0;
	
	        bool private halted = false;
	
	        event Buy(address indexed recipient, uint256 amount);
	        event Transfer(address indexed from, address indexed to, uint256 _value, uint64 time);
	        event Happy(bool indexed yes);
	        event Ping();
    
	        modifier onlyOwner() {
		        require(msg.sender == owner);
		        _;
	        }
	
	        modifier buyPeriod() {
		        require(weiRaised < finishedCap);
		        _;
	        }
	
	        modifier isNotHalted() {
		        require(!halted);
		        _;
	        }
	
	        modifier validPayload(uint256 size) { 
		        require(msg.data.length >= (size + 4));
		        _;
	        }
	
	        function Test() {
		        owner = msg.sender;
	        }
	
	        function etherRaised() constant returns (uint256) {
		        return weiRaised;
	        }
	
	        function tokenDistributed() constant returns (uint256) {
		        return distributedAmount;
	        }
	
	        function isHalted() constant returns (bool) {
		        return halted;
	        }
	
	        function balanceOf(address who) constant returns (uint256) {
		        return balances[who];
	        }
	
	        function investementOf(address who) constant returns (uint256) {
		        return invested[who];
	        }
	
	        function isHappy(address who) constant returns (bool) {
		        return happy[who];
	        }
	
	        function()
		        payable 
		        buyPeriod 
		        isNotHalted
	        {		
		        require(msg.value > 0); 
		        uint256 tokenAmount = msg.value.times(tokensPerEther).divided(1 ether);
		        balances[msg.sender] = balances[msg.sender].plus(tokenAmount);
		        invested[msg.sender] = invested[msg.sender].plus(msg.value);
		        distributedAmount = distributedAmount.plus(tokenAmount);
		        weiRaised = weiRaised.plus(msg.value);
		
		        Buy(msg.sender, tokenAmount);
	        }
	
	        function transfer(address to, uint256 value) returns (bool success) { 
     	        balances[msg.sender] = balances[msg.sender].minus(value);
     	        balances[to] = balances[to].plus(value);
     	        Transfer(msg.sender, to, value, uint64(now));
     	        return true;
            }
    
            function IAmHappy(bool yes)
            {
                happy[msg.sender] = yes;
                Happy(yes);
            }
    
            function ping()
            {
                Ping();
            }
		
	        function drain() onlyOwner 
	        {
		        require(msg.sender.send(this.balance));
	        }
	
	        function setHalted(bool halt) onlyOwner {
		        halted = halt;
	        }
        }";
        #endregion

        #region Reference SC
        private const string referenceSmartContractCode1 = @"pragma solidity ^0.4.13;


contract ReferenceValue {
	mapping(uint64 => uint256) public values;
	uint64 public pointsBetweenValues;

	function ReferenceValue() {
		pointsBetweenValues = 30;

		values[1] = 2000;
		values[2] = 1000;
		values[3] = 3000;
		values[4] = 4500;
		values[5] = 6000;
		values[6] = 169101;
		values[7] = 169003;
		values[8] = 169278;
		values[9] = 169511;
		values[10] = 169942;
		values[11] = 170366;
		values[12] = 171478;
		values[13] = 171612;
		values[14] = 172105;
		values[15] = 172503;
		values[16] = 172002;
		values[17] = 172589;
		values[18] = 173044;
		values[19] = 173678;
		values[20] = 174172;
		values[21] = 174980;
		values[22] = 175831;
		values[23] = 175440;
		values[24] = 176268;
		values[25] = 176075;
		values[26] = 176991;
		values[27] = 177628;
		values[28] = 178346;
		values[29] = 178805;
		values[30] = 179553;
		values[31] = 179722;
		values[32] = 179947;
		values[33] = 180123;
		values[34] = 180754;
		values[35] = 181861;
		values[36] = 181998;
	}

	function getValueAt(uint64 period, uint64 daysOverdue) constant returns (uint256) {
		uint256 value = getBaseValue(period);
		if(daysOverdue > 0) {
			uint256 nextValue = getBaseValue(period + 1);
			if(nextValue != value) {
				uint256 difference;
				if (nextValue > value) 
					difference = nextValue - value;
				else 
					difference = value - nextValue;

				uint256 weight = (difference * daysOverdue);
				if(weight > pointsBetweenValues) {
					if (nextValue > value) 
						value = value + (weight / pointsBetweenValues);
					else
						value = value - (weight / pointsBetweenValues);
				}
				else
					value = nextValue;
			}
		}
		return value;
	}
	
	function getBaseValue(uint64 period) constant internal returns (uint256) {
		return (values[1] * 1 szabo) / values[period];
	}
}";

        private const string referenceSmartContractCode2 = @"pragma solidity ^0.4.13;


contract ReferenceValue {
	mapping(uint64 => uint256) public values;
	uint64 public pointsBetweenValues;

	function ReferenceValue() {
		pointsBetweenValues = 3;

		values[1] = 150;
		values[2] = 300;
		values[3] = 400;
		values[4] = 450;
		values[5] = 500;
		values[6] = 1140;
		values[7] = 1182;
		values[8] = 1182;
		values[9] = 1204;
		values[10] = 1189;
		values[11] = 1211;
		values[12] = 1245;
	}

	function getValueAt(uint64 period, uint64 daysOverdue) constant returns (uint256) {
		uint256 value = getBaseValue(period);
		if(daysOverdue > 0) {
			uint256 nextValue = getBaseValue(period + 1);
			if(nextValue != value) {
				uint256 difference;
				if (nextValue > value) 
					difference = nextValue - value;
				else 
					difference = value - nextValue;

				uint256 weight = (difference * daysOverdue);
				if(weight > pointsBetweenValues) {
					if (nextValue > value) 
						value = value + (weight / pointsBetweenValues);
					else
						value = value - (weight / pointsBetweenValues);
				}
				else
					value = nextValue;
			}
		}
		return value;
	}
	
	function getBaseValue(uint64 period) constant internal returns (uint256) {
		return (values[1] * 1 szabo) / values[period];
	}
}";
        #endregion

        #region Demo ABI
        private const string demoABI = "[{\"constant\":true,\"inputs\":[{\"name\":\"who\",\"type\":\"address\"}],\"name\":\"investedPeriodsBy\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"\",\"type\":\"address\"}],\"name\":\"investedPeriods\",\"outputs\":[{\"name\":\"\",\"type\":\"uint64\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"name\":\"\",\"type\":\"string\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"employerAddress\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"pensionFundAccount\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"employee\",\"type\":\"address\"},{\"name\":\"daysOverdue\",\"type\":\"uint64\"}],\"name\":\"getEmployeeInvestment\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"employee\",\"type\":\"address\"},{\"name\":\"daysOverdue\",\"type\":\"uint64\"}],\"name\":\"employerInvestment\",\"outputs\":[],\"payable\":true,\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"employee\",\"type\":\"address\"},{\"name\":\"employeeContribution\",\"type\":\"uint256\"},{\"name\":\"employerContributionBonus\",\"type\":\"uint256\"},{\"name\":\"employeeSalary\",\"type\":\"uint256\"}],\"name\":\"setContributionInformation\",\"outputs\":[],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"name\":\"\",\"type\":\"uint8\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"fundFee\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"\",\"type\":\"address\"}],\"name\":\"addressBalance\",\"outputs\":[{\"name\":\"amount\",\"type\":\"uint256\"},{\"name\":\"invested\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"who\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"who\",\"type\":\"address\"}],\"name\":\"investedBy\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"latePenaltyFeePerDay\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"referenceAddress\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"employee\",\"type\":\"address\"},{\"name\":\"daysOverdue\",\"type\":\"uint64\"}],\"name\":\"employeeInvestment\",\"outputs\":[],\"payable\":true,\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"employee\",\"type\":\"address\"},{\"name\":\"daysOverdue\",\"type\":\"uint64\"}],\"name\":\"getEmployerInvestment\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"employee\",\"type\":\"address\"}],\"name\":\"getWithdrawalValues\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"},{\"name\":\"\",\"type\":\"uint256\"},{\"name\":\"\",\"type\":\"uint256\"},{\"name\":\"\",\"type\":\"uint256\"},{\"name\":\"\",\"type\":\"uint256\"},{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"employee\",\"type\":\"address\"}],\"name\":\"sell\",\"outputs\":[],\"payable\":true,\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"maxSalaryBonus\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"auctusParticipationFee\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"payable\":false,\"type\":\"function\"},{\"inputs\":[],\"payable\":false,\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"period\",\"type\":\"uint64\"},{\"indexed\":true,\"name\":\"responsable\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"tokenAmount\",\"type\":\"uint256\"},{\"indexed\":false,\"name\":\"invested\",\"type\":\"uint256\"},{\"indexed\":false,\"name\":\"latePenalty\",\"type\":\"uint256\"},{\"indexed\":false,\"name\":\"daysOverdue\",\"type\":\"uint64\"},{\"indexed\":false,\"name\":\"pensionFundFee\",\"type\":\"uint256\"},{\"indexed\":false,\"name\":\"auctusFee\",\"type\":\"uint256\"}],\"name\":\"Buy\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"period\",\"type\":\"uint64\"},{\"indexed\":true,\"name\":\"employee\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"employerBonus\",\"type\":\"uint256\"},{\"indexed\":false,\"name\":\"employerAbsoluteBonus\",\"type\":\"uint256\"},{\"indexed\":false,\"name\":\"employerTokenCashback\",\"type\":\"uint256\"},{\"indexed\":false,\"name\":\"employeeTokenCashback\",\"type\":\"uint256\"},{\"indexed\":false,\"name\":\"employerSzaboCashback\",\"type\":\"uint256\"},{\"indexed\":false,\"name\":\"employeeSzaboCashback\",\"type\":\"uint256\"}],\"name\":\"Withdrawal\",\"type\":\"event\"}]";
        #endregion

        public static void SimpleTest()
        {
            #region Initialization
            int gwei = 21;
            string password = Security.Encrypt("test");
            string mainAddress = "0xfa4ef7de49b1460d4114a8385af5cd638cf55b43";
            string secondAddress = "0xc795b00c8a9e5a0413f48424183dae789d7555d5";
            string thirdAddress = "0x3a56c7e6a97842fdc343de56c960db7c33e33c22";
            KeyValuePair<string, string> mainAccount = new KeyValuePair<string, string>(mainAddress, password);
            KeyValuePair<string, string> secondAccount = new KeyValuePair<string, string>(secondAddress, password);
            KeyValuePair<string, string> thirdAccount = new KeyValuePair<string, string>(thirdAddress, password);
            Wallet account = Web3.CreateAccount(password);
            if (account == null)
                throw new Exception();
            string createdAddress = account.Address;
            KeyValuePair<string, string> createdAccount = new KeyValuePair<string, string>(createdAddress, password);
            #endregion


            #region Send
            ValidateTrasaction(Web3.Send(createdAddress, Web3.ETHER(15), 21000, gwei));
            ValidateTrasaction(Web3.Send(createdAddress, Web3.ETHER(0.000000001), 21000, gwei, secondAccount));
            bool okSendInsuficientFunds = false;
            try
            {
                ValidateTrasaction(Web3.Send(createdAddress, Web3.ETHER(99999999999999999999999999.9), 50000, gwei, thirdAccount));//Will fail (invalid amount) 
            }
            catch
            { okSendInsuficientFunds = true; }
            if (!okSendInsuficientFunds)
                throw new Exception();
            if (Web3.GetBalance(createdAddress) != 15.000000001)
                throw new Exception();
            #endregion


            #region Contract Creation
            SCCompiled scCompiled = Solc.Compile("Test", smartContractStringified).Single(c => c.Name == "Test");
            Transaction contractTransaction = Web3.GetTransaction(Web3.DeployContract(scCompiled, 5100000, gwei));
            if (contractTransaction == null)
                throw new Exception();
            string scAddress = contractTransaction.ContractAddress;
            string scABI = scCompiled.ABI;
            #endregion


            #region Buy
            ValidateTrasaction(Web3.Send(scAddress, Web3.ETHER(80), 210000, gwei));
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "setHalted", Web3.ETHER(0), 120000, gwei, mainAccount, new Variable(VariableType.Bool, true)));
            ValidateTrasaction(Web3.Send(scAddress, Web3.ETHER(20), 210000, gwei));//Will Fail (halted)
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "setHalted", Web3.ETHER(0), 120000, gwei, secondAccount, new Variable(VariableType.Bool, false)));
            ValidateTrasaction(Web3.Send(scAddress, Web3.ETHER(30), 210000, gwei, secondAccount));//Will Fail (halted)
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "setHalted", Web3.ETHER(0), 120000, gwei, mainAccount, new Variable(VariableType.Bool, false)));
            ValidateTrasaction(Web3.Send(scAddress, Web3.ETHER(65.5), 210000, gwei, secondAccount));
            ValidateTrasaction(Web3.Send(scAddress, Web3.ETHER(45.5), 210000, gwei, thirdAccount));
            ValidateTrasaction(Web3.Send(scAddress, Web3.ETHER(5), 210000, gwei, createdAccount));
            ValidateTrasaction(Web3.Send(scAddress, Web3.ETHER(4), 210000, gwei, createdAccount));
            ValidateTrasaction(Web3.Send(scAddress, Web3.ETHER(15), 210000, gwei, thirdAccount));//Will Fail (buy period end)

            ValidateBalanceInvestiment(scAddress, scABI, mainAddress, 800, 80);
            ValidateBalanceInvestiment(scAddress, scABI, secondAddress, 655, 65.5);
            ValidateBalanceInvestiment(scAddress, scABI, thirdAddress, 455, 45.5);
            ValidateBalanceInvestiment(scAddress, scABI, createdAddress, 90, 9);

            List<CompleteVariableType> eventBuy = new List<CompleteVariableType>();
            eventBuy.Add(new CompleteVariableType(VariableType.Address, 0, true));
            eventBuy.Add(new CompleteVariableType(VariableType.BigNumber, 0));
            List<Event> allBuyEvents = Web3.ReadEvent(scAddress, "Buy", eventBuy);
            if (allBuyEvents.Count != 5 && allBuyEvents.Where(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == createdAddress)).Count() != 2 &&
                allBuyEvents.Where(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == createdAddress)).
                    SelectMany(c => c.Data.Where(l => l.Type == VariableType.BigNumber).Select(k => (double)k.Value)).Sum() != 90 &&
                allBuyEvents.Where(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == mainAddress)).
                    SelectMany(c => c.Data.Where(l => l.Type == VariableType.BigNumber).Select(k => (double)k.Value)).Sum() != 800 &&
                allBuyEvents.Where(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == secondAddress)).
                    SelectMany(c => c.Data.Where(l => l.Type == VariableType.BigNumber).Select(k => (double)k.Value)).Sum() != 655 &&
                allBuyEvents.Where(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == thirdAddress)).
                    SelectMany(c => c.Data.Where(l => l.Type == VariableType.BigNumber).Select(k => (double)k.Value)).Sum() != 455)
                throw new Exception();
            List<Event> createdAccountBuyEvents = Web3.ReadEvent(scAddress, "Buy", eventBuy, new EventParameter(VariableType.Address, createdAddress));
            if (createdAccountBuyEvents.Count != 2 && createdAccountBuyEvents.SelectMany(c => c.Data.Where(l => l.Type == VariableType.BigNumber).Select(k => (double)k.Value)).Sum() != 90)
                throw new Exception();
            #endregion


            #region Transfer
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "transfer", Web3.ETHER(0), 200000, gwei, mainAccount, new Variable(VariableType.Address, secondAddress), new Variable(VariableType.BigNumber, new BigNumber(100, 0))));
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "transfer", Web3.ETHER(0), 200000, gwei, thirdAccount, new Variable(VariableType.Address, secondAddress), new Variable(VariableType.BigNumber, new BigNumber(400, 0))));
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "transfer", Web3.ETHER(0), 200000, gwei, mainAccount, new Variable(VariableType.Address, createdAddress), new Variable(VariableType.BigNumber, new BigNumber(150, 0))));
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "transfer", Web3.ETHER(0), 200000, gwei, secondAccount, new Variable(VariableType.Address, createdAddress), new Variable(VariableType.BigNumber, new BigNumber(50, 0))));
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "transfer", Web3.ETHER(0), 200000, gwei, thirdAccount, new Variable(VariableType.Address, secondAddress), new Variable(VariableType.BigNumber, new BigNumber(56, 0)))); //Will Fail (invalid amout)

            ValidateBalance(scAddress, scABI, mainAddress, 550);
            ValidateBalance(scAddress, scABI, secondAddress, 1105);
            ValidateBalance(scAddress, scABI, thirdAddress, 55);
            ValidateBalance(scAddress, scABI, createdAddress, 290);

            List<CompleteVariableType> eventTransfer = new List<CompleteVariableType>();
            eventTransfer.Add(new CompleteVariableType(VariableType.Address, 0, true));
            eventTransfer.Add(new CompleteVariableType(VariableType.Address, 0, true));
            eventTransfer.Add(new CompleteVariableType(VariableType.BigNumber, 0));
            eventTransfer.Add(new CompleteVariableType(VariableType.Number));
            List<Event> allTransferEvents = Web3.ReadEvent(scAddress, "Transfer", eventTransfer);
            if (allTransferEvents.Count != 4 && allTransferEvents.Where(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == mainAddress)).Count() != 2 &&
                allTransferEvents.Where(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == secondAddress)).Count() != 3 &&
                allTransferEvents.Where(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == createdAddress)).Count() != 2 &&
                allTransferEvents.Where(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == thirdAddress)).Count() != 1 &&
                allTransferEvents.SelectMany(c => c.Data.Where(l => l.Type == VariableType.BigNumber).Select(k => (double)k.Value)).Sum() != 700 &&
                allTransferEvents.Where(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == thirdAddress)).
                    SelectMany(c => c.Data.Where(l => l.Type == VariableType.Number).Select(k => (double)k.Value)).Sum() <
                allTransferEvents.Where(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == createdAddress)).
                    SelectMany(c => c.Data.Where(l => l.Type == VariableType.Number).Select(k => (double)k.Value)).Take(1).Sum())
                throw new Exception();
            List<Event> thirdAccountTransferEvents = Web3.ReadEvent(scAddress, "Transfer", eventTransfer, new EventParameter(VariableType.Address, thirdAddress));
            if (thirdAccountTransferEvents.Count != 1 && thirdAccountTransferEvents.Any(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == createdAddress))
                && thirdAccountTransferEvents.SelectMany(c => c.Data.Where(l => l.Type == VariableType.BigNumber).Select(k => (double)k.Value)).Sum() != 400)
                throw new Exception();
            List<Event> secondAccountToTransferEvents = Web3.ReadEvent(scAddress, "Transfer", eventTransfer, new EventParameter(VariableType.Null, null), new EventParameter(VariableType.Address, secondAddress));
            if (secondAccountToTransferEvents.Count != 2 && secondAccountToTransferEvents.Any(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == mainAddress))
                && secondAccountToTransferEvents.Any(c => c.Data.Any(l => l.Type == VariableType.Address && (string)l.Value == thirdAddress))
                && secondAccountToTransferEvents.SelectMany(c => c.Data.Where(l => l.Type == VariableType.BigNumber).Select(k => (double)k.Value)).Sum() != 500)
                throw new Exception();
            #endregion


            #region Happy
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "IAmHappy", Web3.ETHER(0), 200000, gwei, mainAccount, new Variable(VariableType.Bool, true)));
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "IAmHappy", Web3.ETHER(0), 200000, gwei, secondAccount, new Variable(VariableType.Bool, true)));
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "IAmHappy", Web3.ETHER(0), 200000, gwei, mainAccount, new Variable(VariableType.Bool, false)));

            if ((bool)Web3.CallConstFunction(new CompleteVariableType(VariableType.Bool), scAddress, scABI, "isHappy", new Variable(VariableType.Address, mainAddress)).Value)
                throw new Exception();
            if (!(bool)Web3.CallConstFunction(new CompleteVariableType(VariableType.Bool), scAddress, scABI, "isHappy", new Variable(VariableType.Address, secondAddress)).Value)
                throw new Exception();
            if ((bool)Web3.CallConstFunction(new CompleteVariableType(VariableType.Bool), scAddress, scABI, "isHappy", new Variable(VariableType.Address, createdAddress)).Value)
                throw new Exception();

            List<CompleteVariableType> eventHappy = new List<CompleteVariableType>();
            eventHappy.Add(new CompleteVariableType(VariableType.Bool, 0, true));
            List<Event> allHappyEvents = Web3.ReadEvent(scAddress, "Happy", eventHappy);
            if (allHappyEvents.Count != 3 || allHappyEvents.Where(c => c.Data.Any(l => (bool)l.Value)).Count() != 2)
                throw new Exception();
            List<Event> unhappyEvents = Web3.ReadEvent(scAddress, "Happy", eventHappy, new EventParameter(VariableType.Bool, true));
            if (unhappyEvents.Count != 2)
                throw new Exception();
            #endregion


            #region Ping
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "ping", Web3.ETHER(0), 30000, gwei));
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "ping", Web3.ETHER(0), 30000, gwei, secondAccount));
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "ping", Web3.ETHER(0), 30000, gwei, createdAccount));

            if (Web3.ReadEvent(scAddress, "Ping", null).Count != 3)
                throw new Exception();
            #endregion


            #region Balance
            double createdAccountBalance = Web3.GetBalance(createdAddress);
            if (createdAccountBalance > 6 || createdAccountBalance < 5.9)
                throw new Exception();

            double mainAccountBalanceBeforeDrain = Web3.GetBalance(mainAddress);
            ValidateTrasaction(Web3.CallFunction(scAddress, scABI, "drain", Web3.ETHER(0), 40000, gwei));
            double mainAccountBalanceAfterDrain = Web3.GetBalance(mainAddress);
            if ((mainAccountBalanceBeforeDrain + 200) <= mainAccountBalanceAfterDrain || (mainAccountBalanceBeforeDrain + 200) > (mainAccountBalanceAfterDrain + 0.01))
                throw new Exception();
            #endregion
        }

        public static void DefaultDemoContractTest()
        {
            string password = Security.Encrypt("test");
            Wallet pensionFundAccount = Web3.CreateAccount(password);
            if (pensionFundAccount == null)
                throw new Exception();
            Wallet employerAccount = Web3.CreateAccount(password);
            if (employerAccount == null)
                throw new Exception();
            Wallet employeeAccount = Web3.CreateAccount(password);
            if (employeeAccount == null)
                throw new Exception();
            string pensionFundAddress = pensionFundAccount.Address;
            string employerAddress = employerAccount.Address;
            string employeeAddress = employeeAccount.Address;

            SCCompiled scCompiled1 = Solc.Compile("ReferenceValue", referenceSmartContractCode1).Single(c => c.Name == "ReferenceValue");
            Transaction referenceContractTransaction1 = Web3.GetTransaction(Web3.DeployContract(scCompiled1, 2500000, EthereumManager.GetGweiPrice()), 10);
            if (referenceContractTransaction1 == null)
                throw new Exception();
            SCCompiled scCompiled2 = Solc.Compile("ReferenceValue", referenceSmartContractCode2).Single(c => c.Name == "ReferenceValue");
            Transaction referenceContractTransaction2 = Web3.GetTransaction(Web3.DeployContract(scCompiled2, 2500000, EthereumManager.GetGweiPrice()), 10);
            if (referenceContractTransaction2 == null)
                throw new Exception();
            string referenceAddress1 = referenceContractTransaction1.ContractAddress;
            string referenceAddress2 = referenceContractTransaction2.ContractAddress;
            
            Dictionary<int, double> bonusDistribution = new Dictionary<int, double>();
            bonusDistribution[2] = 10;
            bonusDistribution[4] = 40;
            bonusDistribution[6] = 100;
            Dictionary<string, double> reference = new Dictionary<string, double>();
            reference[referenceAddress1] = 40;
            reference[referenceAddress2] = 60;
            string demoTransaction = EthereumManager.DeployDefaultPensionFund(3500000, pensionFundAddress, employerAddress, employeeAddress,
                3, 0.06, 20, 5, 6, 80, 300, reference, bonusDistribution).Key;
            Transaction demoContractTransaction = Web3.GetTransaction(demoTransaction, 10);
            if (demoContractTransaction == null)
                throw new Exception();
            string demoAddress = demoContractTransaction.ContractAddress;

            int gasLimitBuy = 200000;
            string buyEmployee1 = EthereumManager.EmployeeBuyFromDefaultPensionFund(employeeAddress, demoAddress, demoABI, 0, gasLimitBuy);
            string buyEmployer1 = EthereumManager.EmployerBuyFromDefaultPensionFund(employeeAddress, demoAddress, demoABI, 0, gasLimitBuy);
            if (Web3.GetTransaction(buyEmployee1, 8) == null || Web3.GetTransaction(buyEmployer1, 8) == null)
                throw new Exception();

            double demo1 = GetBalance(demoAddress);
            double pension1 = GetBalance(pensionFundAddress);
            Variable token1 = Web3.CallConstFunction(new CompleteVariableType(VariableType.BigNumber), demoAddress, demoABI, "totalSupply");

            string buyEmployee2 = EthereumManager.EmployeeBuyFromDefaultPensionFund(employeeAddress, demoAddress, demoABI, 0, gasLimitBuy);
            string buyEmployer2 = EthereumManager.EmployerBuyFromDefaultPensionFund(employeeAddress, demoAddress, demoABI, 0, gasLimitBuy);
            if (Web3.GetTransaction(buyEmployee2, 8) == null || Web3.GetTransaction(buyEmployer2, 8) == null)
                throw new Exception();

            double demo2 = GetBalance(demoAddress);
            double pension2 = GetBalance(pensionFundAddress);
            Variable token2 = Web3.CallConstFunction(new CompleteVariableType(VariableType.BigNumber), demoAddress, demoABI, "totalSupply");

            string buyEmployee3 = EthereumManager.EmployeeBuyFromDefaultPensionFund(employeeAddress, demoAddress, demoABI, 0, gasLimitBuy);
            string buyEmployer3 = EthereumManager.EmployerBuyFromDefaultPensionFund(employeeAddress, demoAddress, demoABI, 0, gasLimitBuy);
            if (Web3.GetTransaction(buyEmployee3, 8) == null || Web3.GetTransaction(buyEmployer3, 8) == null)
                throw new Exception();

            double demo3 = GetBalance(demoAddress);
            double pension3 = GetBalance(pensionFundAddress);
            Variable token3 = Web3.CallConstFunction(new CompleteVariableType(VariableType.BigNumber), demoAddress, demoABI, "totalSupply");

            string buyEmployee4 = EthereumManager.EmployeeBuyFromDefaultPensionFund(employeeAddress, demoAddress, demoABI, 0, gasLimitBuy);
            string buyEmployer4 = EthereumManager.EmployerBuyFromDefaultPensionFund(employeeAddress, demoAddress, demoABI, 0, gasLimitBuy);
            if (Web3.GetTransaction(buyEmployee4, 8) == null || Web3.GetTransaction(buyEmployer4, 8) == null)
                throw new Exception();

            double demo4 = GetBalance(demoAddress);
            double pension4 = GetBalance(pensionFundAddress);
            Variable token4 = Web3.CallConstFunction(new CompleteVariableType(VariableType.BigNumber), demoAddress, demoABI, "totalSupply");

            string withdrawal = EthereumManager.WithdrawalFromDefaultPensionFund(employeeAddress, demoAddress, demoABI, 200000);
            if (Web3.GetTransaction(withdrawal, 8) == null)
                throw new Exception();

            List<BuyInfo> buy = EthereumManager.ReadBuyFromDefaultPensionFund(demoAddress);
            WithdrawalInfo sell = EthereumManager.ReadWithdrawalFromDefaultPensionFund(demoAddress);
            
            double employee = GetBalance(employeeAddress);
            double employer = GetBalance(employerAddress);
            double pension = GetBalance(pensionFundAddress);
            double demo = GetBalance(demoAddress);
        }

        private static void ValidateTrasaction(string trasactionHash)
        {
            Transaction transactionToSend = Web3.GetTransaction(trasactionHash, 25);
            if (transactionToSend == null)
                throw new Exception();
        }

        private static void ValidateBalance(string scAddress, string scABI, string address, double balance)
        {
            double token = ((BigNumber)Web3.CallConstFunction(new CompleteVariableType(VariableType.BigNumber, 0), scAddress, scABI, "balanceOf", new Variable(VariableType.Address, address)).Value).Value;
            if (token != balance)
                throw new Exception();
        }

        private static void ValidateBalanceInvestiment(string scAddress, string scABI, string address, double balance, double invested)
        {
            ValidateBalance(scAddress, scABI, address, balance);
            double ether = ((BigNumber)Web3.CallConstFunction(Web3.EtherType, scAddress, scABI, "investementOf", new Variable(VariableType.Address, address)).Value).Value;
            if (ether != invested)
                throw new Exception();
        }
        #endregion
    }
}
