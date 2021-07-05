using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using Newtonsoft.Json;

namespace PseudoBanic.Data
{
    public class DatabaseConnection
    {
        public static Cache<string, UserInfo> UserByNicknameCache = new Cache<string, UserInfo>();
        public static Cache<string, UserInfo> UserByAPIKeyCache = new Cache<string, UserInfo>();

        public static UserInfo GetUserInfoByUsername(string Username, bool usecache = true)
        {
            UserInfo ret = UserByNicknameCache.Get(Username);
            if (ret != null && usecache)
            {
                return ret;
            }

            using (var conn = new MySqlConnection(Global.builder.ConnectionString))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT token,userid,admin_level,discord_id FROM users WHERE username = @username";
                    command.Parameters.AddWithValue("@username", Username);

                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    if (!reader.HasRows)
                        return null;

                    ret = new UserInfo();
                    ret.APIKey = reader.GetString(0);
                    ret.UserID = reader.GetInt32(1);
                    ret.AdminLevel = reader.GetInt32(2);
                    ret.DiscordID = reader.GetInt64(3);

                    UserByNicknameCache.Store(Username, ret, TimeSpan.FromMinutes(2));
                }
            }
            return ret;
        }

        public static UserInfo GetUserInfoByAPIKey(string APIKey, bool usecache = true)
        {
            UserInfo ret = UserByAPIKeyCache.Get(APIKey);
            if (ret != null && usecache)
            {
                return ret;
            }

            using (var conn = new MySqlConnection(Global.builder.ConnectionString))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT userid,username,admin_level,discord_id FROM users WHERE token = @token";
                    command.Parameters.AddWithValue("@token", Utils.SHA256String(APIKey));

                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    if (!reader.HasRows) {
                        UserByAPIKeyCache.Store(APIKey, new UserInfo(), TimeSpan.FromSeconds(30));
                        return null;
                    }

                    ret = new UserInfo();
                    ret.APIKey = APIKey;
                    ret.UserID = reader.GetInt32(0);
                    ret.Username = reader.GetString(1);
                    ret.AdminLevel = reader.GetInt32(2);
                    ret.DiscordID = reader.GetInt64(3);
                    UserByAPIKeyCache.Store(APIKey, ret, TimeSpan.FromMinutes(2));
                }
            }
            return ret;
        }

        public static bool AddUserInfo(UserInfo user)
        {
            try
            {
                using (var conn = new MySqlConnection(Global.builder.ConnectionString))
                {
                    conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO users (username, token, admin_level, discord_id) VALUES (@username, @token, 0, @discordid);";
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.Parameters.AddWithValue("@token", Utils.SHA256String(user.APIKey));
                        command.Parameters.AddWithValue("@discordid", user.DiscordID);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception) {
                return false;
            }

            return true;
        }
    }
}
