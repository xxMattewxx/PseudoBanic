using System;
using System.IO;
using System.Net;
using MySqlConnector;

using PseudoBanic.Data;
using PseudoBanic.Requests;
using PseudoBanic.Responses;

namespace PseudoBanic.Handlers
{
	public class Retrieve
	{
        //Atomic retrieve task and create assignment in DB
		public static TaskInfo RetrieveTask(UserInfo user) {
            TaskInfo ret = null;
            try
            {
                using (var conn = new MySqlConnection(Global.builder.ConnectionString))
                {
                    conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            using (var command = conn.CreateCommand())
                            {
                                command.CommandText = "" +
                                    "SELECT @task_id_retrieved := task_id FROM tasks JOIN tasks_metadata ON tasks.task_metaid = tasks_metadata.task_metaid WHERE quorum_left > 0 AND task_status = 1 AND " +
                                        "(SELECT COUNT(*) FROM assignments WHERE task_id = tasks.task_id AND userid = @userid) = 0 LIMIT 1;" +
                                    "INSERT INTO assignments (task_id, userid, deadline, state, output) VALUES (@task_id_retrieved, @userid, @deadline, @state, @output);" +
                                    "UPDATE tasks SET quorum_left = quorum_left - 1 WHERE task_id = @task_id_retrieved;";

                                command.Parameters.AddWithValue("@userid", user.UserID);
                                command.Parameters.AddWithValue("@deadline", DateTime.Now.AddDays(1));
                                command.Parameters.AddWithValue("@state", 0);
                                command.Parameters.AddWithValue("@output", null);
                                command.Transaction = transaction;

                                var reader = command.ExecuteReader();
                                reader.Read();

                                ret = new TaskInfo
                                {
                                    ID = reader.GetInt32(0),
                                    Status = reader.GetInt32(2),
                                    QuorumLeft = reader.GetInt32(3),
                                    InputData = reader.GetString(4),
                                    MetaData = new TaskMeta
                                    {
                                        ID = reader.GetInt32(1),
                                        Name = reader.GetString(7),
                                        BinaryURL = reader.GetString(8)
                                    }
                                };
                            }
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch {}
                            return null;
                        }
                    }
                }
            }
            catch(Exception) {}

            return ret;
        }

		public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader) {
			string jsonStr = reader.ReadToEnd();
			RetrieveRequest request = RetrieveRequest.FromJson(jsonStr);
			if(request == null || request.APIKey == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
				return;
			}

            UserInfo user = DatabaseConnection.GetUserInfoByAPIKey(request.APIKey);
            if (user == null || user.AdminLevel < AdminLevels.None)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Not authorized." }.ToJson());
                return;
            }

            TaskInfo task = RetrieveTask(user);
            if(task == null) {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                writer.Write(new BaseResponse { Message = "No tasks available." }.ToJson());
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            writer.Write(
                new RetrieveResponse
                {
                    Success = true,
                    Message = "Task retrieved.",
                    Task = task
                }.ToJson()
            );
            return;
        }
	}
}