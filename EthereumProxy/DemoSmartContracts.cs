using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.EthereumProxy
{
    public static class DemoSmartContracts
    {
        public const string DEFAULT_PENSION_FUND_SC = @"pragma solidity ^0.4.13;


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

	function minus(uint256 x, uint256 y) internal returns (uint256) {
		assert(y <= x);
		return x - y;
	}

	function plus(uint256 x, uint256 y) internal returns (uint256) {
		uint256 z = x + y;
		assert(z >= x && z >= y);
		return z;
	}
	
	function min(uint256 x, uint256 y) internal constant returns (uint256) {
		return x <= y ? x : y;
	}
	
	function max(uint256 x, uint256 y) internal constant returns (uint256) {
		return x > y ? x : y;
	}
}


contract ReferenceValue {
	function getValueAt(uint64 period, uint64 daysOverdue) constant returns (uint256);
}


contract DemoPensionFund {
	using SafeMath for uint256;

	struct Token {
    	uint256 amount;
		uint256 invested;
    }
	
	struct Reference {
    	address referenceAddress;
		uint256 participation; //per 1000000
    }
	
	string public name = ""Pension Fund Demo Token""; 
    uint8 public decimals = 18;
    uint256 public totalSupply = 0;
	address public owner;
	address public pensionFundAccount = {PENSION_FUND_ADDRESS};
    uint256 public fundFee = {PENSION_FUND_FEE}; //per 1000000
    uint256 public latePenaltyFeePerDay = {PENSION_FUND_LATE_PENALTY}; //per 1000000
	uint256 public auctusParticipationFee = {AUCTUS_FEE}; //per 1000000

	Reference[] public reference;
    mapping(address => Token) public addressBalance;
    mapping(address => uint64) public investedPeriods;
	
	uint256 internal decimalsToFraction = 1000000;
	uint256 internal overflowNumberControl = 999999999999999999999999999999999999999999999999;
	
    modifier onlyOwner() {
        require(msg.sender == owner);
        _;
    }

    function balanceOf(address who) constant returns (uint256) {
        return addressBalance[who].amount;
    }

    function investedBy(address who) constant returns (uint256) {
        return addressBalance[who].invested;
    }

    function investedPeriodsBy(address who) constant returns (uint256) {
        return investedPeriods[who];
    }

    function transferOwnership(address newOwner) onlyOwner {
        owner = newOwner;
    }

    function buy(address responsable, uint256 baseInvestment, uint64 daysOverdue) internal returns (uint256) {
	    investedPeriods[responsable] = investedPeriods[responsable] + 1;
	    uint256 amount = getTokenReferenceValue(investedPeriods[responsable], daysOverdue).times(baseInvestment);
        addressBalance[responsable].amount = addressBalance[responsable].amount.plus(amount); 
	    addressBalance[responsable].invested = addressBalance[responsable].invested.plus(baseInvestment);
	    totalSupply = totalSupply.plus(amount);
        return amount;
    }
    
    function withdrawal(address source) internal {
	    totalSupply = totalSupply.minus(addressBalance[source].amount);
	    addressBalance[source].amount = 0;
    }
	
    function getBaseLatePenaltyFee(uint64 daysOverdue) internal returns(uint256, uint64) {
        uint256 baseFee = latePenaltyFeePerDay.plus(decimalsToFraction);
        uint256 latePenalty = baseFee;
        uint64 remainingDivisions = daysOverdue;
        for (uint64 i = 1; i < daysOverdue; ++i) {
            latePenalty = latePenalty * baseFee;
            if (latePenalty > overflowNumberControl) {
                latePenalty = latePenalty.divided(decimalsToFraction);
                remainingDivisions = remainingDivisions - 1;
            }
        }
        return (latePenalty, remainingDivisions);
    }

    function getFee(uint256 baseValue) internal returns(uint256) {
        return baseValue.times(fundFee).divided(decimalsToFraction);
    }

    function getTokenReferenceValue(uint64 period, uint64 daysOverdue) internal returns(uint256) {
		uint256 tokenValue = 0;
		uint256 remainingDivisions = reference.length;
		for(uint64 i = 0; i < reference.length; ++i) {
            ReferenceValue referenceValueContract = ReferenceValue(reference[i].referenceAddress);
			tokenValue = tokenValue + (referenceValueContract.getValueAt(period, daysOverdue) * reference[i].participation);
			if (tokenValue > overflowNumberControl) {
				tokenValue = tokenValue.divided(decimalsToFraction);
                remainingDivisions = remainingDivisions - 1;
			}
		}
		for(uint64 j = 0; j < remainingDivisions; ++j) {
			tokenValue = tokenValue.divided(decimalsToFraction);
		}
		return tokenValue;
    }

    function sendFees(uint256 baseInvestment, uint256 latePenalty) internal returns(uint256, uint256) {
        uint256 auctusFee = getFee(baseInvestment).plus(latePenalty).times(auctusParticipationFee).divided(decimalsToFraction);
        uint256 remainingFundFee = getFee(baseInvestment).plus(latePenalty).minus(auctusFee);
        if (remainingFundFee > 0)
            pensionFundAccount.transfer(remainingFundFee);
        if (auctusFee > 0)
            owner.transfer(auctusFee);

        return (remainingFundFee, auctusFee);
    }
}


contract CompanyContract is DemoPensionFund {

	address public employerAddress = {EMPLOYER_ADDRESS};
	uint256 public maxSalaryBonus = {MAX_SALARY_BONUS}; //per 1000000
	
	struct Investment {
        uint256 employeeContribution; //per 1000000
        uint256 employerContributionBonus; //per 1000000
        uint256 employeeSalary; //in szabo
    }

    struct BonusVesting {
        uint64 period;
        uint256 vestingBonus; //per 1000000
    }

    BonusVesting[] bonusDistribution;

    mapping(address => Investment) investments; 
	
	event Buy(uint64 indexed period, address indexed responsable, uint256 tokenAmount, uint256 invested, uint256 latePenalty, uint64 daysOverdue, uint256 pensionFundFee, uint256 auctusFee);
	event Withdrawal(uint64 indexed period, address indexed employee, uint256 employerBonus, uint256 employerAbsoluteBonus, uint256 employerTokenCashback, uint256 employeeTokenCashback, uint256 employerSzaboCashback, uint256 employeeSzaboCashback);

    modifier validInvestmentInformation(address employee) {
        require(investments[employee].employeeContribution > 0 && investments[employee].employeeSalary > 0);
        _;
    }

    function CompanyContract() {
        owner = msg.sender; //owner is Auctus Account

		{REFERENCE_VALUE_ADDRESS}
		
        {BONUS_DISTRIBUTION}

        setContributionInformation({EMPLOYEE_ADDRESS}, {EMPLOYEE_CONTRIBUTION}, {EMPLOYEE_CONTRIBUTION_BONUS}, {EMPLOYEE_SALARY});
    }

    function getEmployeeInvestment(address employee, uint64 daysOverdue) constant returns (uint256) {
        uint256 baseInvestment = investments[employee].employeeSalary.times(investments[employee].employeeContribution);
        return getInvestmentValue(baseInvestment, daysOverdue);
    }

    function getEmployerInvestment(address employee, uint64 daysOverdue) constant returns (uint256) {
        uint256 baseInvestment = investments[employee].employeeSalary.times(maxSalaryBonus.min(investments[employee].employeeContribution)).times(investments[employee].employerContributionBonus).divided(decimalsToFraction);
        return getInvestmentValue(baseInvestment, daysOverdue);
    }

    function getWithdrawalValues(address employee) constant returns (uint256, uint256, uint256, uint256, uint256, uint256) {
        var (employerBonus, employerAbsoluteBonus) = getBonusValuesForWithdrawal(employee);
        var (employerTokenCashback, employeeTokenCashback) = getTokenCashback(employee, employerAbsoluteBonus);
        var (employerSzaboCashback, employeeSzaboCashback) = getSzaboCashback(employee, employerTokenCashback, employeeTokenCashback);
        return (employerBonus, employerAbsoluteBonus, employerTokenCashback, employeeTokenCashback, employerSzaboCashback, employeeSzaboCashback);
    }

    function setContributionInformation(
	    address employee, 
	    uint256 employeeContribution, 
	    uint256 employerContributionBonus,
	    uint256 employeeSalary
    )
	    onlyOwner
    {
        investments[employee] = Investment(employeeContribution, employerContributionBonus, employeeSalary);
    }

    function employeeInvestment(address employee, uint64 daysOverdue) 
	    onlyOwner
        validInvestmentInformation(employee) 
	    payable
    {
        uint256 requiredInvestment = getEmployeeInvestment(employee, daysOverdue);
        require(requiredInvestment == msg.value);
        uint256 baseInvestment = getEmployeeInvestment(employee, 0);
        executeBuy(employee, baseInvestment, requiredInvestment, daysOverdue);
    }

    function employerInvestment(address employee, uint64 daysOverdue) 
	    onlyOwner
        validInvestmentInformation(employee) 
	    payable
    {
        uint256 requiredInvestment = getEmployerInvestment(employee, daysOverdue);
        require(requiredInvestment == msg.value);
        uint256 baseInvestment = getEmployerInvestment(employee, 0);
        executeBuy(employerAddress, baseInvestment, requiredInvestment, daysOverdue);
    }

    function sell(address employee) 
	    onlyOwner
        validInvestmentInformation(employee) 
	    payable
    {
        require(addressBalance[employee].amount > 0);

        var (employerBonus, employerAbsoluteBonus, employerTokenCashback, employeeTokenCashback, employerSzaboCashback, employeeSzaboCashback) = getWithdrawalValues(employee);

        withdrawal(employerAddress);
        withdrawal(employee);

        if (employerSzaboCashback > 0)
            employerAddress.transfer(employerSzaboCashback);

        if (employeeSzaboCashback > 0)
            employee.transfer(employeeSzaboCashback);

        Withdrawal(investedPeriods[employee] + 1, employee, employerBonus, employerAbsoluteBonus, employerTokenCashback, employeeTokenCashback, employerSzaboCashback, employeeSzaboCashback);
    }

    function executeBuy(address responsable, uint256 baseInvestment, uint256 requiredInvestment, uint64 daysOverdue) private {
        uint256 amount = buy(responsable, baseInvestment, daysOverdue);
        uint256 latePenalty = requiredInvestment.minus(baseInvestment);
        var(totalFundFee, totalAuctusFee) = sendFees(baseInvestment, latePenalty);
        owner.transfer(this.balance); //Not waste gas for demonstration
        Buy(investedPeriods[responsable], responsable, amount, baseInvestment, latePenalty, daysOverdue, totalFundFee, totalAuctusFee);
    }

    function getEmployerBonusForWithdrawal(address employee) private returns(uint256) {
        uint256 bonusApplied = 0;
        for (uint256 i = 0; i < bonusDistribution.length; i++) {
            if (investedPeriods[employee] >= bonusDistribution[i].period) {
                bonusApplied = bonusApplied.max(bonusDistribution[i].vestingBonus);
            }
        }
        return bonusApplied;
    }

    function getBonusValuesForWithdrawal(address employee) private returns(uint256, uint256) {
        uint256 employerBonus = getEmployerBonusForWithdrawal(employee);
        uint256 employerAbsoluteBonus = addressBalance[employerAddress].amount.times(employerBonus).divided(decimalsToFraction);
        return (employerBonus, employerAbsoluteBonus);
    }

    function getTokenCashback(address employee, uint256 employerAbsoluteBonus) private returns(uint256, uint256) {
        uint256 employerTokenCashback = addressBalance[employerAddress].amount.minus(employerAbsoluteBonus);
        uint256 employeeTokenCashback = addressBalance[employee].amount.plus(employerAbsoluteBonus);
        return (employerTokenCashback, employeeTokenCashback);
    }

    function getSzaboCashback(address employee, uint256 employerTokenCashback, uint256 employeeTokenCashback) private returns(uint256, uint256) {
        uint256 value = getTokenReferenceValue(investedPeriods[employee] + 1, 0);
        uint256 employerSzaboCashback = employerTokenCashback.divided(value);
        uint256 employeeSzaboCashback = employeeTokenCashback.divided(value);
        return (employerSzaboCashback, employeeSzaboCashback);
    }

    function getInvestmentValue(uint256 baseInvestment, uint64 daysOverdue) private returns(uint256) {
        if (baseInvestment > 0 && daysOverdue > 0) {
            var (latePenalty, remainingDivisions) = getBaseLatePenaltyFee(daysOverdue);
            baseInvestment = baseInvestment.times(latePenalty);
            for (uint256 i = 0; i < remainingDivisions; ++i) {
                baseInvestment = baseInvestment.divided(decimalsToFraction);
            }
        }
        return baseInvestment.divided(decimalsToFraction);
    }
}";
        
    }
}
