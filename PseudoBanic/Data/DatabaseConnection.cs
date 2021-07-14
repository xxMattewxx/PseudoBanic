using System;
using System.Collections.Generic;
using System.IO;
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
        public static Cache<Int64, Tuple<bool, UserInfo>> UserByDiscordIDCache = new Cache<Int64, Tuple<bool, UserInfo>>();
        public static Cache<int, Tuple<bool, TaskMeta>> MetadataByIDCache = new Cache<int, Tuple<bool, TaskMeta>>();
        public static Cache<int, Tuple<int, TotalProgress>> ProjectProgressCache = new Cache<int, Tuple<int, TotalProgress>>();
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
                    UserByDiscordIDCache.Store(ret.DiscordID, Tuple.Create(true, ret), TimeSpan.FromMinutes(5));
                    return ret;
                }
            }
        }

        public static UserInfo GetUserInfoByAPIKey(string APIKey, bool usecache = true)
        {
            if (APIKey == null || APIKey.Length != 32) return null;

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
                    UserByDiscordIDCache.Store(ret.DiscordID, Tuple.Create(true, ret), TimeSpan.FromMinutes(5));
                    UserByNicknameCache.Store(ret.Username, Tuple.Create(true, ret), TimeSpan.FromMinutes(5));
                    return ret;
                }
            }
        }

        public static UserInfo GetUserInfoByDiscordID(Int64 discordid, bool usecache = true)
        {
            Tuple<bool, UserInfo> cache = UserByDiscordIDCache.Get(discordid);
            if (cache != null && cache.Item1 && usecache)
            {
                return cache.Item2;
            }

            using (var conn = new MySqlConnection(Global.builder.ConnectionString))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT token,userid,username,admin_level,discord_id FROM users WHERE discord_id = @discordid";
                    command.Parameters.AddWithValue("@discordid", discordid);

                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    if (!reader.HasRows)
                    {
                        UserByDiscordIDCache.Store(discordid, Tuple.Create<bool, UserInfo>(true, null), TimeSpan.FromSeconds(30));
                        return null;
                    }

                    UserInfo ret = new UserInfo();
                    ret.APIKey = reader.GetString(0);
                    ret.UserID = reader.GetInt32(1);
                    ret.Username = reader.GetString(2);
                    ret.AdminLevel = reader.GetInt32(3);
                    ret.DiscordID = discordid;

                    UserByDiscordIDCache.Store(discordid, Tuple.Create(true, ret), TimeSpan.FromMinutes(5));
                    UserByNicknameCache.Store(ret.Username, Tuple.Create(true, ret), TimeSpan.FromMinutes(5));
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
        public static TotalProgress QueryTotalProgress(int id, bool usecache = true)
        {
            Tuple<int, TotalProgress> cache = ProjectProgressCache.Get(id);
            if (cache != null && cache.Item1 > 0 && usecache)
            {
                return cache.Item2;
            }

            using (var conn = new MySqlConnection(Global.builder.ConnectionString))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT tasks_metadata.`task_name`, count(*) FROM tasks_metadata join tasks on tasks_metadata.`task_metaid` = tasks.`task_metaid` WHERE tasks.`task_metaid` = @task_metaid and quorum_left = 0";
                    command.Parameters.AddWithValue("@task_metaid", id);

                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    if (!reader.HasRows)
                    {
                        ProjectProgressCache.Store(id, Tuple.Create<int, TotalProgress>(0, null), TimeSpan.FromMinutes(0));
                        return null;
                    }
                    TotalProgress ret = new TotalProgress();
                    ret.ID = id;
                    ret.Name = reader.GetString(0);
                    ret.TotalDone = reader.GetInt32(1);
                    reader.Close();
                    command.CommandText = "SELECT count(*) from tasks where task_metaid = @task_metaid";

                    reader = command.ExecuteReader();
                    reader.Read();
                    if (!reader.HasRows)
                    {
                        ProjectProgressCache.Store(id, Tuple.Create<int, TotalProgress>(0, null), TimeSpan.FromMinutes(0));
                        return null;
                    }
                    ret.TotalExisting = reader.GetInt32(0);
                    ProjectProgressCache.Store(id, Tuple.Create(id, ret), TimeSpan.FromHours(0));
                    return ret;
                }
            }
        }
        public static List<String> GetOutputsByTaskID(int taskid)
        {
            using (var conn = new MySqlConnection(Global.builder.ConnectionString))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT output FROM assignments WHERE task_id = @taskid";
                    command.Parameters.AddWithValue("@taskid", taskid);

                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    if (!reader.HasRows)
                        return null;

                    List<string> ret = new List<string>();
                    do
                    {
                        ret.Add(reader.GetString(0));
                    }
                    while (reader.Read());

                    return ret;
                }
            }
        }

        public static void StreamOutputsByAppID(int metaid, StreamWriter writer)
        {
            using (var conn = new MySqlConnection(Global.builder.ConnectionString))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "" +
                        "SELECT output FROM assignments JOIN tasks ON assignments.task_id = tasks.task_id " +
                        "WHERE output != '' AND tasks.task_metaid = @metaid;";
                    command.Parameters.AddWithValue("@metaid", metaid);

                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    if (!reader.HasRows)
                        return;

                    List<string> ret = new List<string>();
                    do
                    {
                        writer.Write(reader.GetString(0));
                    }
                    while (reader.Read());

                    writer.Flush();
                    return;
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

        public static long AddTaskMetadata(TaskMeta metadata)
        {
            try
            {
                using (var conn = new MySqlConnection(Global.builder.ConnectionString))
                {
                    conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO tasks_metadata (task_name, binary_url) VALUES (@task_name, @binary_url);";
                        command.Parameters.AddWithValue("@task_name", metadata.Name);
                        command.Parameters.AddWithValue("@binary_url", metadata.BinaryURL);
                        command.ExecuteNonQuery();
                        return command.LastInsertedId;
                    }
                }
            }
            catch (Exception)
            {
                return -1;
            }
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
                        command.CommandText = "DELETE FROM users WHERE userID = @userid;";
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

        //TODO: RETURN ERROR CODES FROM DB
        //-> IS USER OWNER OF THE TASK?
        //-> WAS THE DEADLINE ACHIEVED?
        //-> DOES THAT TASK EVEN EXIST?
        public static bool AttributeResult(int UserID, int TaskID, string Results)
        {
            try
            {
                using (var conn = new MySqlConnection(Global.builder.ConnectionString))
                {
                    conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "CALL UpdateResults(@UserID, @TaskID, @Results);";
                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@TaskID", TaskID);
                        command.Parameters.AddWithValue("@Results", Results);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
