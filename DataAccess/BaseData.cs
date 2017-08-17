using Auctus.Util;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Auctus.DataAccess
{
    public abstract class BaseData<T>
    {
        public BaseData()
        { }
        //TODO: add to configuration manager like structure
        private string connString = Util.Config.DbConnString;

        public abstract string TableName { get; }
        public IEnumerable<T> ListAll()
        {
            using (IDbConnection db = new MySqlConnection(connString))
            {
                return db.Query<T> (String.Format("SELECT * FROM {0}", TableName));
            }
        }

        public void Insert(params object[] values)
        {
            using (IDbConnection db = new MySqlConnection(connString))
            {
                db.Execute(String.Format("INSERT INTO {0} VALUES ({1})", TableName, string.Join(",",values.Select(x => x.ToStringWithSingleQuotes()))));
            }
        }

        public void DeleteById(Int32 id)
        {
            using (IDbConnection db = new MySqlConnection(connString))
            {
                db.Execute(String.Format("DELETE FROM {0} WHERE ID = {1} ", TableName, id));
            }
        }

        //TODO: add generic update

    }
}
