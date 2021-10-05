using Microsoft.EntityFrameworkCore;
using PseudoBanic.Data;
using PseudoBanic.Requests;
using PseudoBanic.Responses;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Net;

namespace PseudoBanic.Handlers.Tasks
{
    public class Retrieve
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

            UserInfo user = UserInfo.GetByAPIKey(APIKey);
            if (user == null || user.AdminLevel < AdminLevels.None)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Not authorized." }.ToJson());
                return;
            }

            TaskInfo task = TaskInfo.RetrieveAtomic(user);
            if (task == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                writer.Write(new BaseResponse { Message = "No tasks available." }.ToJson());
                return;
            }

            Console.WriteLine("Task {0} assigned to user {1}", task.ID, user.ID);

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