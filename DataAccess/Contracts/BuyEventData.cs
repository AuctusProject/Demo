using Auctus.DomainObjects.Contracts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Auctus.DataAccess.Contracts
{
    public class BuyEventData : BaseData<BuyEvent>
    {
        public override string TableName => "BuyEvent";

        private readonly string SQL_LIST_BY_CONTRACT = @"SELECT be.* 
                                                FROM 
                                                BuyEvent be inner join 
                                                PensionFundTransaction pft on pft.Id = be.PensionFundTransactionId
                                                where pft.PensionFundContractHash = @PensionFundContractHash";

        public List<BuyEvent> List(string contractHash)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("PensionFundContractHash", contractHash, System.Data.DbType.AnsiStringFixedLength);
            return Query<BuyEvent>(SQL_LIST_BY_CONTRACT, param).ToList();
        }
    }
}
