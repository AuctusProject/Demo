using Auctus.Util.DapperAttributes;
using Auctus.DomainObjects.Unprocessed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Dapper;

namespace Auctus.DataAccess.Unprocessed
{
    public class UPensionFundData : BaseData<UPensionFund>
    {
        public override string TableName => "UPensionFund";

        private const string SQL_U_PENSION_FUND_DATA = @"select upf.*, uc.*, ue.* from 
	                                                    UPensionFund upf
	                                                    inner join UCompany uc on uc.UPensionFundId = upf.Id
	                                                    inner join UEmployee ue on ue.UCompanyId = uc.Id
                                                        where {0}";


        public List<UPensionFund> ListUnprocessed()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("processed", 0, System.Data.DbType.UInt32);
            return List(string.Format(SQL_U_PENSION_FUND_DATA, "upf.Processed = @processed"), param);
        }

        public UPensionFund Get(int uPensionFundId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("Id", uPensionFundId, System.Data.DbType.UInt32);
            return SelectByParameters<UPensionFund>(param).SingleOrDefault();
        }

        private List<UPensionFund> List(string sql, DynamicParameters param)
        {
            return Query<UPensionFund, UCompany, UEmployee, UPensionFund>(sql,
                   (upf, uc, ue) =>
                   {
                       upf.Company = uc;
                       upf.Company.Employee = ue;
                       return upf;
                   }, "Id,Id", param).AsList();
        }
    }
}
