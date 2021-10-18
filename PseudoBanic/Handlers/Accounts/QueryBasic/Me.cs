using PseudoBanic.Data;
using PseudoBanic.Requests;
using PseudoBanic.Responses;
using System.IO;
using System.Net;

namespace PseudoBanic.Handlers.Accounts
{
    public class QueryBasicMe
    {
        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
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

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            writer.Write(
                new QueryBasicByDiscordIDResponse
                {
                    Success = true,
                    Message = "Basic user data retrieved successfully.",
                    User = new UserInfo
                    {
                        AdminLevel = user.AdminLevel,
                        Username = user.Username,
                        ID = user.ID,
                        DiscordID = user.DiscordID
                    }
                }.ToJson()
            );
            return;
        }
    }
}