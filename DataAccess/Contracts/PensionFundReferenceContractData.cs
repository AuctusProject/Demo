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

        private const string DELETE_BY_CONTRACT_HASH = "delete from PensionFundReferenceContract where PensionFundContractHash = @PensionFundContractHash";
        
        public List<PensionFundReferenceContract> List(string pensionFundContractHash)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("PensionFundContractHash", pensionFundContractHash, System.Data.DbType.AnsiStringFixedLength);
            return SelectByParameters<PensionFundReferenceContract>(param).ToList();
        }

        public void Delete(string pensionFundContractHash)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("PensionFundContractHash", pensionFundContractHash, System.Data.DbType.AnsiStringFixedLength);
            Execute(DELETE_BY_CONTRACT_HASH, param);
        }
    }
}
