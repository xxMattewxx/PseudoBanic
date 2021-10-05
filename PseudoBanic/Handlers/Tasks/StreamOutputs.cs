using Npgsql;
using PseudoBanic.Data;
using PseudoBanic.Requests;
using PseudoBanic.Responses;
using System;
using System.IO;
using System.Net;

namespace PseudoBanic.Handlers.Tasks
{
    public class StreamOutputsByAppID
    {
        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            string jsonStr = reader.ReadToEnd();
            string APIKey = context.Request.Headers.Get("Authorization");

            if (APIKey == null || APIKey.Length != 32)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Missing or invalid API key." }.ToJson());
                return;
            }

            StreamOutputsByAppIDRequest request = StreamOutputsByAppIDRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            UserInfo user = UserInfo.GetByAPIKey(APIKey);
            if (user == null || user.AdminLevel < AdminLevels.Basic)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Not authorized." }.ToJson());
                return;
            }

            TaskMeta meta = TaskMeta.GetByID(request.MetaID.Value);
            if (meta == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                writer.Write(new BaseResponse { Message = "Metadata not found." }.ToJson());
                return;
            }

            StreamOutputs(request.MetaID.Value, writer);
            return;
        }

        private static void StreamOutputs(int metaid, StreamWriter writer)
        {
            Int32 taskid = 0;

            while (true)
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(Global.DBConnectionStr.ConnectionString))
                {
                    conn.Open();

                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT \"ID\",\"Consensus\" " +
                            "FROM \"Tasks\" " +
                            "WHERE \"MetadataID\" = @metaid AND \"Status\" = 2 AND \"Tasks\".\"ID\" > @taskid LIMIT 100;";

                        command.Parameters.AddWithValue("@metaid", metaid);
                        command.Parameters.AddWithValue("@taskid", taskid);

                        using (var reader = command.ExecuteReader())
                        {
                            reader.Read();

                            if (!reader.HasRows)
                                break;

                            do
                            {
                                taskid = reader.GetInt32(0);
                                writer.WriteLine("<task><id>{0}</id>", taskid);
                                writer.Write(reader.GetString(1));
                                writer.WriteLine("</task>");
                            }
                            while (reader.Read());

                            writer.Flush();
                        }
                    }
                }
            }
            writer.Flush();
        }
    }
}