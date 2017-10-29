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

        private const string SQL_PENDING_MINING_CONTRACTS = "select * from PensionFundContract where Address IS NULL";

        public PensionFundContract GetPensionFundContract(string transactionHash)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("TransactionHash", transactionHash, DbType.AnsiStringFixedLength);
            return SelectByParameters<PensionFundContract>(parameters).SingleOrDefault();
        }

        public List<PensionFundContract> ListPendingMiningContracts()
        {
            return Query<PensionFundContract>(SQL_PENDING_MINING_CONTRACTS).ToList();
        }
    }
}
