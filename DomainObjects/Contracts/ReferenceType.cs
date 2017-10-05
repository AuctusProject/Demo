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
        public static readonly ReferenceType Gold = new ReferenceType("0x913c4e62f4dd699ea7e3f452278edef00e2bee12");
        public static readonly ReferenceType SP500 = new ReferenceType("0x2f10d5a94b926b546f7e33267bcd6318cdf5ed6d");
        public static readonly ReferenceType MSCIWorld = new ReferenceType("0x42a694a6587b9a14c766abfa15acdcca77c90405");
        public static readonly ReferenceType VWEHX = new ReferenceType("0xd221c95b9b800fc84a061715fc91574df17610bb");
        public static readonly ReferenceType Bitcoin = new ReferenceType("0x77b5035202adb157dd828749a91eaf76aa6bdd74");

        public string Address { get; private set; }

        private ReferenceType(string address)
        {
            Address = address;
        }

        public static ReferenceType Get(string address)
        {
            switch (address)
            {
                case "0x913c4e62f4dd699ea7e3f452278edef00e2bee12":
                    return Gold;
                case "0x2f10d5a94b926b546f7e33267bcd6318cdf5ed6d":
                    return SP500;
                case "0x42a694a6587b9a14c766abfa15acdcca77c90405":
                    return MSCIWorld;
                case "0xd221c95b9b800fc84a061715fc91574df17610bb":
                    return VWEHX;
                case "0x77b5035202adb157dd828749a91eaf76aa6bdd74":
                    return Bitcoin;
                default:
                    throw new Exception("Invalid reference type.");
            }
        }
    }
}
