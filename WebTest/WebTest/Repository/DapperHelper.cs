using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace WebTest.Repository
{
    public static class DapperHelper
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="sql">查询的sql</param>
        /// <param name="param">替换参数</param>
        /// <returns></returns>
        public static List<T> GetModelList<T>(this DbConnection dbConnection, string sql, object param = null)
        {
            using (dbConnection)
            {
                return dbConnection.Query<T>(sql, param).ToList();
            }
        }

        /// <summary>
        /// 查询第一个数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T QueryFirst<T>(this DbConnection dbConnection, string sql, object param)
        {
            using (dbConnection)
            {
                return dbConnection.QueryFirst<T>(sql, param);
            }
        }

        /// <summary>
        /// 查询第一个数据没有返回默认值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T GetModel<T>(this DbConnection dbConnection, string sql, object param = null)
        {
            using (dbConnection)
            {
                return dbConnection.QueryFirstOrDefault<T>(sql, param);
            }
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T QuerySingle<T>(this DbConnection dbConnection, string sql, object param)
        {
            using (dbConnection)
            {
                return dbConnection.QuerySingle<T>(sql, param);
            }
        }

        /// <summary>
        /// 查询单条数据没有返回默认值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T QuerySingleOrDefault<T>(this DbConnection dbConnection, string sql, object param)
        {
            using (dbConnection)
            {
                return dbConnection.QuerySingleOrDefault<T>(sql, param);
            }
        }

        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool ExecuteCommand(this DbConnection dbConnection, string sql, object param = null)
        {
            using (dbConnection)
            {
                int result = dbConnection.Execute(sql, param);
                return (result > 0);
            }
        }

        /// <summary>
        /// Reader获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(this DbConnection dbConnection, string sql, object param)
        {
            using (dbConnection)
            {
                return dbConnection.ExecuteReader(sql, param);
            }
        }

        /// <summary>
        /// Scalar获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DbConnection dbConnection, string sql, object param = null)
        {
            using (dbConnection)
            {
                return dbConnection.ExecuteScalar<T>(sql, param);
            }
        }

        /// <summary>
        /// Scalar获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T ExecuteScalarForT<T>(this DbConnection dbConnection, string sql, object param)
        {
            using (dbConnection)
            {
                return dbConnection.ExecuteScalar<T>(sql, param);
            }
        }
    }
}
