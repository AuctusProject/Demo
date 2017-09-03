using Auctus.DomainObjects.Contracts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Auctus.DataAccess.Contracts
{
    public class PensionFundTransactionData : BaseData<PensionFundTransaction>
    {
        public override string TableName => "PensionFundTransaction";
        
        public List<PensionFundTransaction> List(string pensionFundContractHash)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("PensionFundContractHash", pensionFundContractHash, System.Data.DbType.AnsiStringFixedLength);
            return SelectByParameters<PensionFundTransaction>(param).ToList();
        }
    }
}
