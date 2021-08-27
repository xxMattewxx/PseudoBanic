using PseudoBanic.Data;
using PseudoBanic.Requests;
using PseudoBanic.Responses;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PseudoBanic.Handlers.Accounts
{
    public class RegenByUsername
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

            RegenByUsernameRequest request = RegenByUsernameRequest.FromJson(jsonStr);
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

            UserInfo target = DatabaseConnection.GetUserInfoByUsername(request.Username);
            if (target == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                writer.Write(new BaseResponse { Message = "Username does not exist." }.ToJson());
                return;
            }

            string apikey = Utils.GenerateAPIKey();
            if (!DatabaseConnection.UpdateUserToken(target.UserID, apikey))
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                writer.Write(new BaseResponse { Message = "Failure updating user data in DB." }.ToJson());
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            writer.Write(
                new RegisterResponse
                {
                    Success = true,
                    Message = "User API key regenerated.",
                    APIKey = apikey
                }.ToJson()
            );
            return;
        }
    }
}