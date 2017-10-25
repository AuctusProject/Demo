
using Auctus.DomainObjects.Contracts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Auctus.DataAccess.Contracts
{
    public class WithdrawalEventData : BaseData<WithdrawalEvent>
    {
        public override string TableName => "WithdrawalEvent";

        private readonly string SQL_LIST_BY_CONTRACT = @"SELECT we.* 
                                                FROM 
                                                WithdrawalEvent we inner join 
                                                PensionFundTransaction pft on pft.Id = we.PensionFundTransactionId
                                                where pft.PensionFundContractHash = @PensionFundContractHash";

        public List<WithdrawalEvent> List(string contractHash)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("PensionFundContractHash", contractHash, System.Data.DbType.AnsiStringFixedLength);
            return Query<WithdrawalEvent>(SQL_LIST_BY_CONTRACT, param).ToList();
        }
    }
}
