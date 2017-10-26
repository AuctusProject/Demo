using Auctus.Util.DapperAttributes;
using Auctus.DomainObjects.Unprocessed;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace Auctus.DataAccess.Unprocessed
{
    public class UPensionFundData : BaseData<UPensionFund>
    {
        public override string TableName => "UPensionFund";

        private const string SQL_U_PENSION_FUND_DATA = @"select upf.*, uc.*, ue.*, uvr.* from 
	                                                    UPensionFund upf
	                                                    inner join UCompany uc on uc.UPensionFundId = upf.Id
	                                                    inner join UEmployee ue on ue.UCompanyId = uc.Id
	                                                    inner join UVestingRule uvr on uvr.UCompanyId = uc.id
                                                        where {0}";


        public List<UPensionFund> ListUnprocessed()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("processed", 0, System.Data.DbType.Byte);
            return List(string.Format(SQL_U_PENSION_FUND_DATA, "upf.Processed = @processed"), param);
        }

        private List<UPensionFund> List(string sql, DynamicParameters param)
        {
            return Query<UPensionFund, UCompany, UEmployee, UVestingRule, UPensionFund>(sql,
                   (upf, uc, ue, uvr) =>
                   {
                       uc.Employees = new List<UEmployee>() { ue };
                       uc.VestingRules = new List<UVestingRule>() { uvr };
                       upf.Companies = new List<UCompany>() { uc };
                       return upf;
                   }, "Id,Id,Id,Id", param).AsList();
        }
    }
}
