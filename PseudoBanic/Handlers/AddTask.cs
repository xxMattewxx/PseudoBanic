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
    public class AddTask
    {
        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            string jsonStr = reader.ReadToEnd();
            AddTaskRequest request = AddTaskRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            UserInfo user = DatabaseConnection.GetUserInfoByAPIKey(request.APIKey);
            if (user == null || user.AdminLevel < AdminLevels.Administrator)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Not authorized." }.ToJson());
                return;
            }

            if (!DatabaseConnection.AddTask(request.Task))
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                writer.Write(new BaseResponse { Message = "Failure adding task to DB." }.ToJson());
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            writer.Write(
                new BaseResponse
                {
                    Success = true,
                    Message = "Task added to DB."
                }.ToJson()
            );
            return;
        }
    }
}