using MySqlConnector;
using PseudoBanic.Data;
using PseudoBanic.Requests;
using PseudoBanic.Responses;
using System;
using System.IO;
using System.Net;

namespace PseudoBanic.Handlers.Tasks
{
    public class Retrieve
    {
        //Atomic retrieve task and create assignment in DB
        public static TaskInfo RetrieveTask(UserInfo user)
        {
            TaskInfo ret = null;
            try
            {
                using MySqlConnection conn = new MySqlConnection(Global.builder.ConnectionString);
                conn.Open();

                using MySqlCommand command = conn.CreateCommand();
                command.CommandText = "CALL RetrieveAtomic(@userid);";
                command.Parameters.AddWithValue("@userid", user.UserID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;

                    reader.Read();

                    ret = new TaskInfo
                    {
                        ID = reader.GetInt32(0),
                        Status = reader.GetInt32(2),
                        QuorumLeft = reader.GetInt32(3),
                        InputData = reader.GetString(4),
                        Metadata = new TaskMeta
                        {
                            ID = reader.GetInt32(1),
                            Name = reader.GetString(5),
                            BinaryURL = reader.GetString(6),
                            FileHash = reader.GetString(7)
                        }
                    };
                }
            }
            catch (Exception) { }

            return ret;
        }

        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            string jsonStr = reader.ReadToEnd();
            RetrieveRequest request = RetrieveRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid())
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
            if (task == null)
            {
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