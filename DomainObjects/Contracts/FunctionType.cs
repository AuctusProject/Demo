using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class FunctionType
    {
        public static readonly FunctionType EmployeeBuy = new FunctionType(1);
        public static readonly FunctionType CompanyBuy = new FunctionType(2);
        public static readonly FunctionType CompleteWithdrawal = new FunctionType(3);

        public int Type { get; private set; }

        private FunctionType(int type)
        {
            Type = type;
        }

        public static FunctionType Get(int type)
        {
            switch (type)
            {
                case 1:
                    return EmployeeBuy;
                case 2:
                    return CompanyBuy;
                case 3:
                    return CompleteWithdrawal;
                default:
                    throw new Exception("Invalid function type.");
            }
        }
    }
}
