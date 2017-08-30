using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Reflection;
using Auctus.Util.DapperAttributes;
using MySql.Data.MySqlClient;

namespace Auctus.DataAccess.Core
{
    public abstract class DapperRepositoryBase
    {
        private readonly string _connectionString;

        public abstract string TableName { get; }

        #region Constructor

        protected DapperRepositoryBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region Standard Dapper functionality

        // Example: GetBySql<Activity>( "SELECT * 
        //FROM Activities WHERE Id = @activityId", new {activityId = 15} ).FirstOrDefault();
        protected IEnumerable<T> GetItems<T>(CommandType commandType, string sql, object parameters = null)
        {
            using (var connection = GetOpenConnection())
            {
                return connection.Query<T>(sql, parameters, commandType: commandType);
            }
        }

        protected int Execute(CommandType commandType, string sql, object parameters = null)
        {
            using (var connection = GetOpenConnection())
            {
                return connection.Execute(sql, parameters, commandType: commandType);
            }
        }

        protected MySqlConnection GetOpenConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        #endregion

        #region Automated methods for: Insert, Update, Delete

        protected IEnumerable<T> Select<T>(object criteria = null)
        {
            var properties = ParseProperties(criteria);
            var sqlPairs = GetSqlPairs(properties.AllNames, " AND ");
            var sql = string.Format("SELECT * FROM {0} ", TableName);
            if (!String.IsNullOrEmpty(sqlPairs))
            {
                sql += " WHERE " + sqlPairs;
            }
            return GetItems<T>(CommandType.Text, sql, properties.AllPairs);
        }

        protected void Insert<T>(T obj)
        {
            var propertyContainer = ParseProperties(obj);
            var sql = string.Format("INSERT INTO {0} ({1}) VALUES(@{2}); SELECT LAST_INSERT_ID();",
                TableName,
                string.Join(", ", propertyContainer.ValueNames),
                string.Join(", @", propertyContainer.ValueNames));

            using (var connection = GetOpenConnection())
            {
                var id = connection.Query<int>
                (sql, propertyContainer.ValuePairs, commandType: CommandType.Text).First();
                SetId(obj, id, propertyContainer.IdPairs);
            }
        }

        protected void Update<T>(T obj)
        {
            var propertyContainer = ParseProperties(obj);
            var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
            var sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames);
            var sql = string.Format("UPDATE {0} SET {1} WHERE {2} ", TableName, sqlValuePairs, sqlIdPairs);
            Execute(CommandType.Text, sql, propertyContainer.AllPairs);
        }

        protected void Delete<T>(T obj)
        {
            var propertyContainer = ParseProperties(obj);
            var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
            var sql = string.Format("DELETE FROM {0} WHERE {1} ", TableName, sqlIdPairs);
            Execute(CommandType.Text, sql, propertyContainer.IdPairs);
        }

        #endregion

        #region Reflection support
        private static PropertyContainer ParseProperties<T>(T obj)
        {
            var propertyContainer = new PropertyContainer();

            var typeName = typeof(T).Name;
            var validKeyNames = new[] { "Id",
            string.Format("{0}Id", typeName), string.Format("{0}_Id", typeName) };

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                // Skip reference types (but still include string!)
                if (property.PropertyType.GetTypeInfo().IsInterface || (property.PropertyType.GetTypeInfo().IsClass && property.PropertyType != typeof(string)))
                    continue;

                // Skip methods without a public setter
                if (property.GetSetMethod() == null)
                    continue;

                // Skip methods specifically ignored
                if (property.IsDefined(typeof(DapperIgnoreAttribute), false))
                    continue;

                var name = property.Name;
                var value = typeof(T).GetProperty(property.Name).GetValue(obj, null);

                if (property.IsDefined(typeof(DapperKeyAttribute), false) || validKeyNames.Contains(name))
                {
                    propertyContainer.AddId(name, value);
                }
                else
                {
                    propertyContainer.AddValue(name, value);
                }
            }

            return propertyContainer;
        }

        /// <summary>
        /// Create a commaseparated list of value pairs on 
        /// the form: "key1=@value1, key2=@value2, ..."
        /// </summary>
        private static string GetSqlPairs
        (IEnumerable<string> keys, string separator = ", ")
        {
            var pairs = keys.Select(key => string.Format("{0}=@{0}", key)).ToList();
            return string.Join(separator, pairs);
        }

        private void SetId<T>(T obj, int id, IDictionary<string, object> propertyPairs)
        {
            if (propertyPairs.Count == 1)
            {
                var propertyName = propertyPairs.Keys.First();
                var propertyInfo = obj.GetType().GetProperty(propertyName);
                if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(obj, id, null);
                }
            }
        }

        #endregion

        private class PropertyContainer
        {
            private readonly Dictionary<string, object> _ids;
            private readonly Dictionary<string, object> _values;

            #region Properties

            internal IEnumerable<string> IdNames
            {
                get { return _ids.Keys; }
            }

            internal IEnumerable<string> ValueNames
            {
                get { return _values.Keys; }
            }

            internal IEnumerable<string> AllNames
            {
                get { return _ids.Keys.Union(_values.Keys); }
            }

            internal IDictionary<string, object> IdPairs
            {
                get { return _ids; }
            }

            internal IDictionary<string, object> ValuePairs
            {
                get { return _values; }
            }

            internal IEnumerable<KeyValuePair<string, object>> AllPairs
            {
                get { return _ids.Concat(_values); }
            }

            #endregion

            #region Constructor

            internal PropertyContainer()
            {
                _ids = new Dictionary<string, object>();
                _values = new Dictionary<string, object>();
            }

            #endregion

            #region Methods

            internal void AddId(string name, object value)
            {
                _ids.Add(name, value);
            }

            internal void AddValue(string name, object value)
            {
                _values.Add(name, value);
            }

            #endregion
        }
    }
}