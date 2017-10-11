using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class ReferenceType 
    {
        /* ROPSTEN
        public static readonly ReferenceType Gold = new ReferenceType("0x2121b20c077b7de1cb9fd3ec184408f246d0f12a");
        public static readonly ReferenceType SP500 = new ReferenceType("0xd9340654db260f5f69df1a6dd64ffe6be3632844");
        public static readonly ReferenceType MSCIWorld = new ReferenceType("0x62e77625fd1ea74eefa35e040574c723d2275cd1");
        public static readonly ReferenceType VWEHX = new ReferenceType("0xce4b9b40a88cf1cbd8958b153cbe25325e4ce4f2");
        public static readonly ReferenceType Bitcoin = new ReferenceType("0x3fabbaa13605b47823690de0f84d16ffaba05ba2");
        */

        /* RINKEBY */
        public static readonly ReferenceType Gold = new ReferenceType("0x568bcdfeb148dd28146cc9284abdb3c7f6bd1bb6");
        public static readonly ReferenceType SP500 = new ReferenceType("0xb1683904aa36dad60054f97674d52888f923132b");
        public static readonly ReferenceType MSCIWorld = new ReferenceType("0x52b2b54b6d7abdbf83ec18545a34a45651965952");
        public static readonly ReferenceType VWEHX = new ReferenceType("0x60deb3409ca4ab6244802325649d1fe5bf59a84e");
        public static readonly ReferenceType Bitcoin = new ReferenceType("0x5b5053ba3c6a2c5d46d7d75136b7fc1730a8720c");

        public string Address { get; private set; }

        private ReferenceType(string address)
        {
            Address = address;
        }

        public static ReferenceType Get(string address)
        {
            switch (address)
            {
                case "0x568bcdfeb148dd28146cc9284abdb3c7f6bd1bb6":
                    return Gold;
                case "0xb1683904aa36dad60054f97674d52888f923132b":
                    return SP500;
                case "0x52b2b54b6d7abdbf83ec18545a34a45651965952":
                    return MSCIWorld;
                case "0x60deb3409ca4ab6244802325649d1fe5bf59a84e":
                    return VWEHX;
                case "0x5b5053ba3c6a2c5d46d7d75136b7fc1730a8720c":
                    return Bitcoin;
                default:
                    throw new Exception("Invalid reference type.");
            }
        }
    }
}
