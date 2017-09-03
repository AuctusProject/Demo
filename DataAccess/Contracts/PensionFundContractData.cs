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

        public PensionFundContract GetPensionFundContract(string transactionHash)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@TransactionHash", transactionHash, DbType.AnsiStringFixedLength);
            return SelectByParameters<PensionFundContract>(parameters).SingleOrDefault();
        }
    }
}
