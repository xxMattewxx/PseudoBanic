using PseudoBanic.Data;
using PseudoBanic.Requests;
using PseudoBanic.Responses;
using System.IO;
using System.Net;

namespace PseudoBanic.Handlers.Accounts
{
    public class QueryBasicByUserID
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

            QueryBasicRequest request = QueryBasicRequest.FromJson(jsonStr);
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

            UserInfo target = UserInfo.GetByUserID(request.UserID);
            if (target == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                writer.Write(new BaseResponse { Message = "Associated target not found." }.ToJson());
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            writer.Write(
                new QueryBasicByDiscordIDResponse
                {
                    Success = true,
                    Message = "Basic user data retrieved successfully.",
                    User = new UserInfo
                    {
                        ID = target.ID,
                        Username = target.Username,
                        DiscordID = target.DiscordID,
                        AdminLevel = target.AdminLevel
                    }
                }.ToJson()
            );
            return;
        }
    }
}