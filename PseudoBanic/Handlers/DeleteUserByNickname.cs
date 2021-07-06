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
    public class DeleteUserByNickname
    {
        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            string jsonStr = reader.ReadToEnd();
            DeleteUserByNicknameRequest request = DeleteUserByNicknameRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid()) {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            UserInfo user = DatabaseConnection.GetUserInfoByAPIKey(request.APIKey);
            if (user == null || user.AdminLevel < AdminLevels.Administrator) {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Not authorized." }.ToJson());
                return;
            }
            
            UserInfo target = DatabaseConnection.GetUserInfoByUsername(request.Username);
            if (target == null) {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                writer.Write(new BaseResponse { Message = "User not found." }.ToJson());
                return;
            }

            if (!DatabaseConnection.DeleteUserByID(target.UserID)) {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                writer.Write(new BaseResponse { Message = "Failure deleting user from DB." }.ToJson());
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            writer.Write(
                new BaseResponse
                {
                    Success = true,
                    Message = "User deleted successfully."
                }.ToJson()
            );
            return;
        }
    }
}