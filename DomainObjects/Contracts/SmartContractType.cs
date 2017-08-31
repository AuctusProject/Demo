using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class SmartContractType
    {
        public static readonly SmartContractType PensionFund = new SmartContractType(1);
        public static readonly SmartContractType ReferenceValue = new SmartContractType(2);

        public int Type { get; private set; }

        private SmartContractType(int type)
        {
            Type = type;
        }

        public static SmartContractType Get(int type)
        {
            switch (type)
            {
                case 1:
                    return PensionFund;
                case 2:
                    return ReferenceValue;
                default:
                    throw new Exception("Invalid smart contract type.");
            }
        }
    }
}
