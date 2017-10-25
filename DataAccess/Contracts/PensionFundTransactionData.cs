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

        private readonly string SQL_LIST_FOR_PROCESSING =
            @" SELECT PensionFundTransaction.*, PensionFundContract.Address PensionFundContractAddress FROM PensionFundTransaction
            INNER JOIN PensionFundContract on PensionFundContract.TransactionHash = PensionFundTransaction.PensionFundContractHash
            WHERE PensionFundTransaction.TransactionStatus = @TransactionStatus
            AND PensionFundTransaction.NodeProcessorId= @NodeProcessorId ";

        private readonly string SQL_LIST =
            @" SELECT PensionFundTransaction.*, BuyEvent.*, WithdrawalEvent.* FROM PensionFundTransaction 
            LEFT JOIN BuyEvent on BuyEvent.PensionFundTransactionId = PensionFundTransaction.Id
            LEFT JOIN WithdrawalEvent on WithdrawalEvent.PensionFundTransactionId = PensionFundTransaction.Id
            WHERE PensionFundTransaction.PensionFundContractHash = @PensionFundContractHash ";

        public List<PensionFundTransaction> List(string pensionFundContractHash)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("PensionFundContractHash", pensionFundContractHash, System.Data.DbType.AnsiStringFixedLength);
            return SelectByParameters<PensionFundTransaction>(param).ToList();
        }

        public List<PensionFundTransaction> List2(string pensionFundContractHash)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("PensionFundContractHash", pensionFundContractHash, System.Data.DbType.AnsiStringFixedLength);
            return Query<PensionFundTransaction, BuyEvent, WithdrawalEvent, PensionFundTransaction>(SQL_LIST,
                    (p, be, we) =>
                    {
                        p.BuyEvent = be;
                        p.WithdrawalEvent = we;
                        return p;
                    }, "PensionFundTransactionId,PensionFundTransactionId", param).ToList();
        }

        public List<PensionFundTransaction> ListForProcessing(TransactionStatus status, int nodeId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("TransactionStatus", status, System.Data.DbType.Byte);
            param.Add("NodeProcessorId", nodeId, System.Data.DbType.Int32);
            return Query<PensionFundTransaction>(SQL_LIST_FOR_PROCESSING, param).ToList();
        }
    }
}
