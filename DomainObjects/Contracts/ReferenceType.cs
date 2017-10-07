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
        public static readonly ReferenceType Gold = new ReferenceType("0xa6abe0e9ed778c941f05ba48dc28a2d0df2fbeea");
        public static readonly ReferenceType SP500 = new ReferenceType("0x73240cd905919d8026b53959b7c6724501b4ac88");
        public static readonly ReferenceType MSCIWorld = new ReferenceType("0xd3e3f3c1a652d53d43e96164080ed3b7cd2e269d");
        public static readonly ReferenceType VWEHX = new ReferenceType("0xbcc4173c6eb7d2e6bb4df4b9ae6e86c570d07615");
        public static readonly ReferenceType Bitcoin = new ReferenceType("0xae7a8e93a6f9dab37b92b65d3d6d8536da9f6db5");

        public string Address { get; private set; }

        private ReferenceType(string address)
        {
            Address = address;
        }

        public static ReferenceType Get(string address)
        {
            switch (address)
            {
                case "0xa6abe0e9ed778c941f05ba48dc28a2d0df2fbeea":
                    return Gold;
                case "0x73240cd905919d8026b53959b7c6724501b4ac88":
                    return SP500;
                case "0xd3e3f3c1a652d53d43e96164080ed3b7cd2e269d":
                    return MSCIWorld;
                case "0xbcc4173c6eb7d2e6bb4df4b9ae6e86c570d07615":
                    return VWEHX;
                case "0xae7a8e93a6f9dab37b92b65d3d6d8536da9f6db5":
                    return Bitcoin;
                default:
                    throw new Exception("Invalid reference type.");
            }
        }
    }
}
