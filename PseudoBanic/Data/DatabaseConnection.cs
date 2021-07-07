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
        public static Cache<string, Tuple<bool, UserInfo>> UserByNicknameCache = new Cache<string, Tuple<bool, UserInfo>>();
        public static Cache<string, Tuple<bool, UserInfo>> UserByAPIKeyCache = new Cache<string, Tuple<bool, UserInfo>>();
        public static Cache<int, Tuple<bool, TaskMeta>> MetadataByIDCache = new Cache<int, Tuple<bool, TaskMeta>>();

        public static bool ChangeUserLevel(int UserID, int Level)
        {
            try
            {
                using (var conn = new MySqlConnection(Global.builder.ConnectionString))
                {
                    conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "UPDATE users SET admin_level = @admin_level WHERE userid = @userid;";
                        command.Parameters.AddWithValue("@userid", UserID);
                        command.Parameters.AddWithValue("@admin_level", Level);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static UserInfo GetUserInfoByUsername(string Username, bool usecache = true)
        {
            Tuple<bool, UserInfo> cache = UserByNicknameCache.Get(Username);
            if (cache != null && cache.Item1 && usecache)
            {
                return cache.Item2;
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

                    if (!reader.HasRows) {
                        UserByNicknameCache.Store(Username, Tuple.Create<bool, UserInfo>(true, null), TimeSpan.FromSeconds(30));
                        return null;
                    }

                    UserInfo ret = new UserInfo();
                    ret.APIKey = reader.GetString(0);
                    ret.UserID = reader.GetInt32(1);
                    ret.AdminLevel = reader.GetInt32(2);
                    ret.DiscordID = reader.GetInt64(3);

                    UserByNicknameCache.Store(Username, Tuple.Create(true, ret), TimeSpan.FromMinutes(5));
                    return ret;
                }
            }
        }

        public static UserInfo GetUserInfoByAPIKey(string APIKey, bool usecache = true)
        {
            Tuple<bool, UserInfo> cache = UserByAPIKeyCache.Get(APIKey);
            if (cache != null && cache.Item1 && usecache)
            {
                return cache.Item2;
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
                        UserByAPIKeyCache.Store(APIKey, Tuple.Create<bool, UserInfo>(true, null), TimeSpan.FromSeconds(30));
                        return null;
                    }

                    UserInfo ret = new UserInfo();
                    ret.APIKey = APIKey;
                    ret.UserID = reader.GetInt32(0);
                    ret.Username = reader.GetString(1);
                    ret.AdminLevel = reader.GetInt32(2);
                    ret.DiscordID = reader.GetInt64(3);

                    UserByAPIKeyCache.Store(APIKey, Tuple.Create(true, ret), TimeSpan.FromMinutes(5));
                    return ret;
                }
            }
        }

        public static TaskMeta GetTaskMetaByID(int id, bool usecache = true)
        {
            Tuple<bool, TaskMeta> cache = MetadataByIDCache.Get(id);
            if (cache != null && cache.Item1 && usecache)
            {
                return cache.Item2;
            }

            using (var conn = new MySqlConnection(Global.builder.ConnectionString))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT task_name,binary_url FROM tasks_metadata WHERE task_metaid = @task_metaid";
                    command.Parameters.AddWithValue("@task_metaid", id);

                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    if (!reader.HasRows)
                    {
                        MetadataByIDCache.Store(id, Tuple.Create<bool, TaskMeta>(true, null), TimeSpan.FromMinutes(2));
                        return null;
                    }

                    TaskMeta ret = new TaskMeta();
                    ret.ID = id;
                    ret.Name = reader.GetString(0);
                    ret.BinaryURL = reader.GetString(1);

                    MetadataByIDCache.Store(id, Tuple.Create(true, ret), TimeSpan.FromHours(2));
                    return ret;
                }
            }
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
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool AddTask(TaskInfo task)
        {
            try
            {
                using (var conn = new MySqlConnection(Global.builder.ConnectionString))
                {
                    conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO tasks (task_metaid, task_status, quorum_left, input_data) VALUES (@task_metaid, @task_status, @quorum_left, @input_data);";
                        command.Parameters.AddWithValue("@task_metaid", task.Metadata.ID);
                        command.Parameters.AddWithValue("@task_status", task.Status);
                        command.Parameters.AddWithValue("@quorum_left", task.QuorumLeft);
                        command.Parameters.AddWithValue("@input_data", task.InputData);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool DeleteUserByID(int UserID)
        {
            try
            {
                using (var conn = new MySqlConnection(Global.builder.ConnectionString))
                {
                    conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM users WHERE userid = @userid";
                        command.Parameters.AddWithValue("@userid", UserID);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
