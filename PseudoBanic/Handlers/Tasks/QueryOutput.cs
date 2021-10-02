/* TO IMPLEMENT using PseudoBanic.Data;
using PseudoBanic.Requests;
using PseudoBanic.Responses;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace PseudoBanic.Handlers.Tasks
{
    public class QueryOutput
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

            QueryOutputsRequest request = QueryOutputsRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            UserInfo user = DatabaseConnection.GetUserInfoByAPIKey(APIKey);
            if (user == null || user.AdminLevel < AdminLevels.Basic)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Not authorized." }.ToJson());
                return;
            }

            List<string> outputs = DatabaseConnection.GetOutputsByTaskID(request.TaskID);
            if (outputs == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                writer.Write(new BaseResponse { Message = "Metadata not found." }.ToJson());
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            writer.Write(
                new QueryOutputsResponse
                {
                    Success = true,
                    Message = "Outputs retrieved successfully.",
                    Outputs = outputs
                }.ToJson()
            );
            return;
        }
    }
}*/