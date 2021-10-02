using PseudoBanic.Data;
using PseudoBanic.Requests;
using PseudoBanic.Responses;
using System.IO;
using System.Net;

namespace PseudoBanic.Handlers.Tasks
{
    public class AddTask
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

            AddTaskRequest request = AddTaskRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            UserInfo user = UserInfo.GetByAPIKey(APIKey);
            if (user == null || user.AdminLevel < AdminLevels.Administrator)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Not authorized." }.ToJson());
                return;
            }

            if (!TaskInfo.AddToDatabase(request.Task))
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