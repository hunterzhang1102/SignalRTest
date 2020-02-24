using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebTest.Repository
{
    public class UserRepository
    {
        public static string connStr = string.Empty;
        private static ConnectionFactory connectionFactory;

        public UserRepository()
        {
            connectionFactory = new ConnectionFactory(connStr);
        }

        public void Insert(UserEntity userEntity)
        {
            string sql = "INSERT INTO `user` (`name`, `avator`) VALUES (@name, @avator)";
            connectionFactory.GetConnection().ExecuteCommand(sql, new { name= userEntity.name, avator  = userEntity.avator });
        }

        public UserEntity GetUser(string name)
        {
            string sql = "SELECT * FROM `user` WHERE name = @name";
            return connectionFactory.GetConnection().GetModel<UserEntity>(sql, new { name = name });
        }
    }
}
