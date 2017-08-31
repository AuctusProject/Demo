using Auctus.Util;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Auctus.DataAccess.Core;
using Auctus.Util.NotShared;

namespace Auctus.DataAccess
{
    public abstract class BaseData<T> : DapperRepositoryBase
    {
        public BaseData() : base(Config.DbConnString)
        { }
        //TODO: add to configuration manager like structure
        private readonly string connString = Config.DbConnString;

        public new IEnumerable<T> SelectByObject<T>(T criteria)
        {
            return base.SelectByObject<T>(criteria);
        }

        public new IEnumerable<T> SelectAll<T>()
        {
            return base.SelectAll<T>();
        }

        public new void Insert<T>(T obj)
        {
            base.Insert<T>(obj);
        }

        public new void Update<T>(T obj)
        {
            base.Update<T>(obj);
        }

        public new void Delete<T>(T obj)
        {
            base.Delete<T>(obj);
        }
    }
}
