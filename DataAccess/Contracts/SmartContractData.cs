using Auctus.DomainObjects.Contracts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Auctus.DataAccess.Contracts
{
    public class SmartContractData : BaseData<SmartContract>
    {
        public override string TableName => "SmartContract";

        private const string SQL_SMART_CONTRACT_DATA = @"select sc.*, cf.* from
                                                        SmartContract sc 
                                                        left join ContractFunction cf on cf.SmartContractId = sc.Id
                                                        where sc.Id = @id";
        
        public SmartContract Get(int id)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("id", id, System.Data.DbType.UInt32);
            return QueryParentChild<SmartContract, ContractFunction, int>(SQL_SMART_CONTRACT_DATA, sc => sc.Id, sc => sc.ContractFunctions, "Id", param).SingleOrDefault();
        }
    }
}
