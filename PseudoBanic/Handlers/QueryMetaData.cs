using System;
using System.IO;
using System.Net;
using System.Text;
using MySqlConnector;
using System.Security.Cryptography;

using PseudoBanic.Data;
using PseudoBanic.Requests;
using PseudoBanic.Responses;

namespace PseudoBanic.Handlers
{
    public class QueryMetadata
    {
        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            string jsonStr = reader.ReadToEnd();
            QueryMetadataRequest request = QueryMetadataRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            UserInfo user = DatabaseConnection.GetUserInfoByAPIKey(request.APIKey);
            if (user == null || user.AdminLevel < AdminLevels.Basic)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Not authorized." }.ToJson());
                return;
            }

            TaskMeta meta = DatabaseConnection.GetTaskMetaByID(request.ID);
            if (meta == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                writer.Write(new BaseResponse { Message = "Metadata not found." }.ToJson());
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            writer.Write(
                new BaseResponse
                {
                    Success = true,
                    Message = "User deleted successfully."
                }.ToJson()
            );
            return;
        }
    }
}