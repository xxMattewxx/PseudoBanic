using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace PseudoBanic.Data
{
    public class DatabaseConnection
    {
        public static UserInfo GetUserInfoByToken(string Token)
        {
            UserInfo ret = null;

            using (var conn = new MySqlConnection(Global.builder.ConnectionString))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT userid,username FROM users WHERE token = @token";
                    command.Parameters.AddWithValue("@token", Token);

                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    if (!reader.HasRows)
                        return null;

                    ret = new UserInfo();
                    ret.Token = Token;
                    ret.UserID = reader.GetInt32(0);
                    ret.Username = reader.GetString(1);
                    ret.AdminLevel = reader.GetInt32(2);
                }
            }
            return ret;
        }
    }
}
