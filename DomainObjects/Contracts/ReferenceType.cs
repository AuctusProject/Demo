using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public class ReferenceType 
    {
        public static readonly ReferenceType Gold = new ReferenceType("0x42a694a6587b9a14c766abfa15acdcca77c90405");
        public static readonly ReferenceType SP500 = new ReferenceType("0xd221c95b9b800fc84a061715fc91574df17610bb");
        public static readonly ReferenceType MSCIWorld = new ReferenceType("0x5cdf2373a02362fef0e035edfc64292cb7ea33ea");
        public static readonly ReferenceType VWEHX = new ReferenceType("0x3af1be6f13a840f60de22d1441fd66cac567722a");
        public static readonly ReferenceType Bitcoin = new ReferenceType("0x4e69dac3a9d4b91c48d17f805bf9237484f9e040");

        public string Address { get; private set; }

        private ReferenceType(string address)
        {
            Address = address;
        }

        public static ReferenceType Get(string address)
        {
            switch (address)
            {
                case "0x42a694a6587b9a14c766abfa15acdcca77c90405":
                    return Gold;
                case "0xd221c95b9b800fc84a061715fc91574df17610bb":
                    return SP500;
                case "0x5cdf2373a02362fef0e035edfc64292cb7ea33ea":
                    return MSCIWorld;
                case "0x3af1be6f13a840f60de22d1441fd66cac567722a":
                    return VWEHX;
                case "0x4e69dac3a9d4b91c48d17f805bf9237484f9e040":
                    return Bitcoin;
                default:
                    throw new Exception("Invalid reference type.");
            }
        }
    }
}
