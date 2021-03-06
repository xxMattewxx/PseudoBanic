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
    public class AddTaskMetadata
    {
        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            string jsonStr = reader.ReadToEnd();
            string APIKey = context.Request.Headers.Get("Authorization");

            if(APIKey == null || APIKey.Length != 32)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Missing or invalid API key." }.ToJson());
                return;
            }

            AddTaskMetadataRequest request = AddTaskMetadataRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            UserInfo user = DatabaseConnection.GetUserInfoByAPIKey(APIKey);
            if (user == null || user.AdminLevel < AdminLevels.Manager)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Not authorized." }.ToJson());
                return;
            }

            long metadataID = DatabaseConnection.AddTaskMetadata(request.Metadata);
            if (metadataID == -1)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                writer.Write(new BaseResponse { Message = "Failure adding task metadata to DB." }.ToJson());
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            writer.Write(
                new AddTaskMetadataResponse
                {
                    Success = true,
                    Message = "Task metadata added to DB.",
                    MetadataID = metadataID
                }.ToJson()
            );
            return;
        }
    }
}