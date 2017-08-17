using Auctus.DomainObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Contracts
{
    public class ContractData : BaseData<Contract>
    {
        public override string TableName => "Contract";
    }
}
