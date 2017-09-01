using Auctus.DomainObjects.Contracts;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Auctus.DataAccess.Contracts
{
    public class PensionFundContractData : BaseData<PensionFundContract>
    {
        public override string TableName => "PensionFundContract";

        public PensionFundContract GetPensionFundContract(int pensionFundContractId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", pensionFundContractId, DbType.UInt32);
            return SelectByParameters<PensionFundContract>(parameters).FirstOrDefault();
        }
    }
}
