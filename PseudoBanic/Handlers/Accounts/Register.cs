using PseudoBanic.Data;
using PseudoBanic.Requests;
using PseudoBanic.Responses;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PseudoBanic.Handlers.Accounts
{
    public class Register
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

            RegisterRequest request = RegisterRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            UserInfo user = UserInfo.GetByAPIKey(APIKey);
            if (user == null || user.AdminLevel < AdminLevels.Moderator)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Not authorized." }.ToJson());
                return;
            }

            UserInfo userByUsername = UserInfo.GetByUsername(request.User.Username);
            if (userByUsername != null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                writer.Write(new BaseResponse { Message = "Username already exists." }.ToJson());
                return;
            }

            UserInfo userByDiscordID = UserInfo.GetByDiscordID(request.User.DiscordID);
            if (userByDiscordID != null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                writer.Write(new BaseResponse { Message = "Discord ID already in DB." }.ToJson());
                return;
            }

            request.User.APIKey = Utils.GenerateAPIKey();
            if (!UserInfo.AddToDatabase(request.User))
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                writer.Write(new BaseResponse { Message = "Failure adding user to DB." }.ToJson());
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            writer.Write(
                new RegisterResponse
                {
                    Success = true,
                    Message = "User and key generated.",
                    APIKey = request.User.APIKey
                }.ToJson()
            );
            return;
        }
    }
}