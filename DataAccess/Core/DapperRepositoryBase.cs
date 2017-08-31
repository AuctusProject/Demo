using Auctus.Util.DapperAttributes;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Auctus.DataAccess.Core
{
    public abstract class DapperRepositoryBase
    {
        private readonly string _connectionString;
        private const int _timeout = 30;

        public abstract string TableName { get; }

        #region Constructor
        protected DapperRepositoryBase(string connectionString)
        {
            _connectionString = connectionString;
        }
        #endregion

        #region Connection
        protected MySqlConnection GetOpenConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
        #endregion

        #region Query
        protected IEnumerable<T> Query<T>(string sql, dynamic param = null, int commandTimeout = _timeout, IDbTransaction transaction = null)
        {
            using (var connection = GetOpenConnection())
            {
                return SqlMapper.Query<T>(connection, sql, param, transaction, true, commandTimeout, CommandType.Text);
            }
        }

        protected IEnumerable<IDictionary<string, object>> Query(string sql, dynamic param = null, int commandTimeout = _timeout, IDbTransaction transaction = null)
        {
            using (var connection = GetOpenConnection())
            {
                return SqlMapper.Query(connection, sql, param, transaction, true, commandTimeout, CommandType.Text);
            }
        }

        protected SqlMapper.GridReader QueryMultiple(string sql, dynamic param = null, int commandTimeout = _timeout, IDbTransaction transaction = null)
        {
            using (var connection = GetOpenConnection())
            {
                return SqlMapper.QueryMultiple(connection, sql, param, transaction, commandTimeout, CommandType.Text);
            }
        }

        protected IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, string splitOn, dynamic param = null, int commandTimeout = _timeout, IDbTransaction transaction = null)
        {
            using (var connection = GetOpenConnection())
            {
                return SqlMapper.Query<TFirst, TSecond, TReturn>(connection, sql, map, param, transaction, true, splitOn, commandTimeout, CommandType.Text);
            }
        }

        protected IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn, dynamic param = null, int commandTimeout = _timeout, IDbTransaction transaction = null)
        {
            using (var connection = GetOpenConnection())
            {
                return SqlMapper.Query<TFirst, TSecond, TThird, TReturn>(connection, sql, map, param, transaction, true, splitOn, commandTimeout, CommandType.Text);
            }
        }

        protected IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string splitOn, dynamic param = null, int commandTimeout = _timeout, IDbTransaction transaction = null)
        {
            using (var connection = GetOpenConnection())
            {
                return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TReturn>(connection, sql, map, param, transaction, true, splitOn, commandTimeout, CommandType.Text);
            }
        }
        #endregion

        #region Execute
        protected int Execute(string sql, dynamic param = null, int commandTimeout = _timeout, IDbTransaction transaction = null)
        {
            using (var connection = GetOpenConnection())
            {
                return SqlMapper.Execute(connection, sql, param, transaction, commandTimeout, CommandType.Text);
            }
        }

        protected int ExecuteReturningIdentity(string sql, dynamic param = null, int commandTimeout = _timeout, IDbTransaction transaction = null)
        {
            using (var connection = GetOpenConnection())
            {
                sql = string.Concat(sql, "; SELECT LAST_INSERT_ID();");
                return SqlMapper.ExecuteScalar<int>(connection, sql, param, transaction, commandTimeout, CommandType.Text);
            }
        }
        #endregion

        protected IEnumerable<T> SelectAll<T>()
        {
            return Select<T>(null, null);
        }

        protected IEnumerable<T> SelectByObject<T>(T criteria)
        {
            string pairs = null;
            PropertyContainer properties = null;
            if (criteria != null)
            {
                properties = ParseProperties(criteria);
                pairs = GetSqlPairs(properties.AllNames, " AND ");
            }
            return Select<T>(pairs, properties?.AllParameters);
        }

        protected IEnumerable<T> SelectByParameters<T>(DynamicParameters criteria)
        {
            string pairs = null;
            if (criteria != null && criteria.ParameterNames.Count() > 0)
                pairs = GetSqlPairs(criteria.ParameterNames, " AND ");

            return Select<T>(pairs, criteria);
        }

        private IEnumerable<T> Select<T>(string pairs, DynamicParameters criteria)
        {
            string sql = string.Format("SELECT * FROM {0} ", TableName);
            if (!string.IsNullOrEmpty(pairs))
                sql += " WHERE " + pairs;

            return Query<T>(sql, criteria);
        }

        protected void Insert<T>(T obj)
        {
            PropertyContainer propertyContainer = ParseProperties(obj);
            bool identity = !string.IsNullOrEmpty(propertyContainer.Identity);
            IEnumerable<string> columns = identity ? propertyContainer.ValueNames : propertyContainer.AllNames;
            var sql = string.Format("INSERT INTO {0} ({1}) VALUES(@{2})",
                TableName,
                string.Join(", ", columns),
                string.Join(", @", columns));

            if (identity)
            {
                int id = ExecuteReturningIdentity(sql, propertyContainer.ValueParameters);
                SetIdentity<T>(obj, id, propertyContainer.Identity);
            }
            else
                Execute(sql, propertyContainer.AllParameters);
        }

        protected void Update<T>(T obj)
        {
            PropertyContainer propertyContainer = ParseProperties(obj);
            string sqlKeyPairs = GetSqlPairs(propertyContainer.KeyNames, " AND ");
            string sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames, ", ");
            string sql = string.Format("UPDATE {0} SET {1} WHERE {2} ", TableName, sqlValuePairs, sqlKeyPairs);
            Execute(sql, propertyContainer.AllParameters);
        }

        protected void Delete<T>(T obj)
        {
            PropertyContainer propertyContainer = ParseProperties(obj);
            string sqlKeyPairs = GetSqlPairs(propertyContainer.KeyNames, " AND ");
            string sql = string.Format("DELETE FROM {0} WHERE {1} ", TableName, sqlKeyPairs);
            Execute(sql, propertyContainer.KeyParameters);
        }
        
        private static PropertyContainer ParseProperties<T>(T obj)
        {
            PropertyContainer propertyContainer = new PropertyContainer();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                // Skip reference types (but still include string!)
                if (property.PropertyType.GetTypeInfo().IsInterface || (property.PropertyType.GetTypeInfo().IsClass && property.PropertyType != typeof(string)))
                    continue;

                // Skip methods without a public setter
                if (property.GetSetMethod() == null)
                    continue;
                
                // Skip methods without db type defined
                if (!property.IsDefined(typeof(DapperTypeAttribute), false))
                    continue;
                
                object value = typeof(T).GetProperty(property.Name).GetValue(obj, null);
                DbType dbType = ((DapperTypeAttribute)property.GetCustomAttribute(typeof(DapperTypeAttribute))).DbType;
                if (property.IsDefined(typeof(DapperKeyAttribute), false))
                    propertyContainer.AddKey(property.Name, value, dbType, ((DapperKeyAttribute)property.GetCustomAttribute(typeof(DapperKeyAttribute))).Identity);
                else
                    propertyContainer.AddValue(property.Name, value, dbType);
            }
            return propertyContainer;
        }
        
        private static string GetSqlPairs(IEnumerable<string> keys, string separator)
        {
            return string.Join(separator, keys.Select(key => string.Format("{0}=@{0}", key)));
        }

        private void SetIdentity<T>(T obj, int id, string identity)
        {
            if (!string.IsNullOrEmpty(identity))
            {
                PropertyInfo propertyInfo = obj.GetType().GetProperty(identity);
                if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(obj, id, null);
                }
            }
        }
        
        private class PropertyContainer
        {
            private string _identity;
            private readonly DynamicParameters _keyParameters;
            private readonly DynamicParameters _valueParameters;
            private readonly DynamicParameters _allParameters;

            internal PropertyContainer()
            {
                _keyParameters = new DynamicParameters();
                _valueParameters = new DynamicParameters();
                _allParameters = new DynamicParameters();
            }

            internal string Identity
            {
                get { return _identity; }
            }

            internal IEnumerable<string> KeyNames
            {
                get { return _keyParameters.ParameterNames; }
            }

            internal IEnumerable<string> ValueNames
            {
                get { return _valueParameters.ParameterNames; }
            }

            internal IEnumerable<string> AllNames
            {
                get { return _allParameters.ParameterNames; }
            }

            internal DynamicParameters KeyParameters
            {
                get { return _keyParameters; }
            }

            internal DynamicParameters ValueParameters
            {
                get { return _valueParameters; }
            }

            internal DynamicParameters AllParameters
            {
                get { return _allParameters; }
            }
            
            internal void AddKey(string name, object value, DbType type, bool identity)
            {
                if (identity)
                {
                    if (!string.IsNullOrEmpty(_identity))
                        throw new Exception("Incorrect identity parameter.");
                    _identity = name;
                }
                Add(_keyParameters, name, value, type);
                Add(_allParameters, name, value, type);
            }

            internal void AddValue(string name, object value, DbType type)
            {
                Add(_valueParameters, name, value, type);
                Add(_allParameters, name, value, type);
            }

            private void Add(DynamicParameters param, string name, object value, DbType type)
            {
                param.Add(name, value, type);
            }
        }
    }
}