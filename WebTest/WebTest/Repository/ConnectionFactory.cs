using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebTest.Repository
{
    public class ConnectionFactory
    {
        private ConnType connType;
        private string connStr;

        public ConnectionFactory(string _connStr, ConnType _connType = ConnType.MYSQL)
        {
            connStr = _connStr;
            connType = _connType;
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            DbConnection dbConnection = null;
            switch (connType)
            {
                case ConnType.MYSQL:
                    dbConnection = new MySqlConnection(connStr);
                    break;
                case ConnType.SQLSERVER:
                    break;
                case ConnType.ORACLE:
                    break;
                default:
                    throw new Exception("invalid db type");
            }
            dbConnection.Open();
            return dbConnection;
        }
    }

    /// <summary>
    /// 连接数据库类型
    /// </summary>
    public enum ConnType
    {
        MYSQL,
        SQLSERVER,
        ORACLE
    }
}
