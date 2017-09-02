using Auctus.DomainObjects.Contracts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Auctus.DataAccess.Contracts
{
    public class PensionFundReferenceContractData : BaseData<PensionFundReferenceContract>
    {
        public override string TableName => "PensionFundReferenceContract";

        private const string SQL_REFERENCE_CONTRACT = @"select * from PensionFundReferenceContract where PensionFundContractId = @id";

        public List<PensionFundReferenceContract> List(int pensionFundContractId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("id", pensionFundContractId, System.Data.DbType.UInt32);
            return Query<PensionFundReferenceContract>(SQL_REFERENCE_CONTRACT, param).ToList();
        }
    }
}
