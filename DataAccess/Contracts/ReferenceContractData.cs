using Auctus.DomainObjects.Contracts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Auctus.DataAccess.Contracts
{
    public class ReferenceContractData : BaseData<ReferenceContract>
    {
        public override string TableName => "ReferenceContract";

        private const string SQL_REFERENCE_CONTRACT_DATA = @"select rc.*, rv.* from
                                                            ReferenceContract rc 
                                                            inner join ReferenceValue rv on rv.ReferenceContractAddress = rc.Address
                                                            where rc.Address = @address";

        public ReferenceContract Get(string address)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("address", address, System.Data.DbType.AnsiStringFixedLength);
            return QueryParentChild<ReferenceContract, ReferenceValue, string>(SQL_REFERENCE_CONTRACT_DATA, rc => rc.Address, rc => rc.ReferenceValue, "ReferenceContractAddress", param).SingleOrDefault();
        }
    }
}
