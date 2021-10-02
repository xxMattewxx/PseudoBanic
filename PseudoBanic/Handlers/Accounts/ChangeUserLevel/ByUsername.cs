using PseudoBanic.Data;
using PseudoBanic.Requests;
using PseudoBanic.Responses;
using System.IO;
using System.Net;

namespace PseudoBanic.Handlers.Accounts
{
    public class ChangeUserLevelByUsername
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

            ChangeUserLevelRequest request = ChangeUserLevelRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            UserInfo user = UserInfo.GetByAPIKey(APIKey);
            if (user == null || user.AdminLevel < AdminLevels.Manager)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Not authorized." }.ToJson());
                return;
            }

            UserInfo target = UserInfo.GetByUsername(request.Username);
            if (target == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                writer.Write(new BaseResponse { Message = "User not found." }.ToJson());
                return;
            }

            if (target.AdminLevel == request.Level)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                writer.Write(new BaseResponse { Message = "Target already has the specified level." }.ToJson());
                return;
            }

            if (target.AdminLevel >= user.AdminLevel)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                writer.Write(new BaseResponse { Message = "Target user has a higher or equal amount of level." }.ToJson());
                return;
            }

            if (!target.SetAdminLevel(request.Level))
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                writer.Write(new BaseResponse { Message = "Could not modify level in DB." }.ToJson());
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            writer.Write(
                new BaseResponse
                {
                    Success = true,
                    Message = "User level changed successfully."
                }.ToJson()
            );
            return;
        }
    }
}