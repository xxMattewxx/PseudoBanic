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
		public static TaskInfo RetrieveTask(string token) {
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
                                    "SELECT @userid := userID FROM users WHERE token = @token LIMIT 1;" +
                                    "SELECT @task_id_retrieved := task_id FROM tasks WHERE quorum_left > 0 AND " +
                                            "(SELECT COUNT(*) FROM assignments WHERE task_id = tasks.task_id AND userid = @userid) = 0 LIMIT 1; " +
                                    "INSERT INTO assignments (task_id, userid, deadline, state, output) VALUES (@task_id_retrieved, @userid, @deadline, @state, @output);" +
                                    "UPDATE tasks SET quorum_left = quorum_left - 1 WHERE task_id = @task_id_retrieved;";

                                command.Parameters.AddWithValue("@token", token);
                                command.Parameters.AddWithValue("@deadline", DateTime.Now);
                                command.Parameters.AddWithValue("@state", 0);
                                command.Parameters.AddWithValue("@output", null);
                                command.Transaction = transaction;

                                var reader = command.ExecuteReader();
                                reader.Read();

                                ret = new TaskInfo
                                {
                                    ID = reader.GetInt32(0),
                                    GroupID = reader.GetInt32(1),
                                    Name = reader.GetString(2),
                                    Status = reader.GetInt32(3),
                                    QuorumLeft = reader.GetInt32(4),
                                    BinaryURL = reader.GetString(5),
                                    InputData = reader.GetString(6)
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
                            catch
                            {
                                Console.WriteLine("[ERROR] Could not reverse assignment transaction!!!");
                            }
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
			if(request == null || request.Token == null) {
				writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
				return;
			}

			TaskInfo task = RetrieveTask(request.Token);
            if(task == null) {
                writer.Write(new BaseResponse { Message = "No tasks available." }.ToJson());
                return;
            }

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