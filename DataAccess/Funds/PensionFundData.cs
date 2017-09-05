using Auctus.DomainObjects.Accounts;
using Auctus.DomainObjects.Contracts;
using Auctus.DomainObjects.Funds;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Auctus.DataAccess.Funds
{
    public class PensionFundData : BaseData<PensionFund>
    {
        public override string TableName => "PensionFund";

        private const string SQL_PENSION_FUND_DATA = @"select p.*, pfo.*, pfc.*, c.*, e.* from 
                                                        PensionFund p
                                                        inner join PensionFundOption pfo on pfo.PensionFundId = p.Id
                                                        inner join PensionFundContract pfc on pfc.PensionFundOptionAddress = pfo.Address
                                                        inner join Company c on c.PensionFundOptionAddress = pfo.Address
                                                        inner join Employee e on e.CompanyAddress = c.Address
                                                        where {0}";


        public PensionFund GetByContract(string pensionFundContractAddress)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("address", pensionFundContractAddress, System.Data.DbType.AnsiStringFixedLength);
            return Get(string.Format(SQL_PENSION_FUND_DATA, "pfc.Address = @address"), param);
        }

        public PensionFund GetByTransaction(string pensionFundContractHash)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("hash", pensionFundContractHash, System.Data.DbType.AnsiStringFixedLength);
            return Get(string.Format(SQL_PENSION_FUND_DATA, "pfc.TransactionHash = @hash"), param);
        }

        private PensionFund Get(string sql, DynamicParameters param)
        {
            return Query<PensionFund, PensionFundOption, PensionFundContract, Company, Employee, PensionFund>(sql,
                    (p, pfo, pfc, c, e) =>
                    {
                        p.Option = pfo;
                        p.Option.PensionFundContract = pfc;
                        p.Option.Company = c;
                        p.Option.Company.Employee = e;
                        return p;
                    }, "Address,TransactionHash,Address,Address", param).SingleOrDefault();
        }
    }
}
