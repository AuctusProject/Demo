using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Contracts
{
    public class SmartContractData : BaseData<SmartContract>
    {
        public override string TableName => "SmartContract";
    }
}
