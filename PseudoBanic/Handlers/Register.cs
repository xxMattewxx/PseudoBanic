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
    public class Register
    {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private static string GenerateAPIKey()
        {
            byte[] buffer = new byte[16];
            rng.GetNonZeroBytes(buffer);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            string jsonStr = reader.ReadToEnd();
            RegisterRequest request = RegisterRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid()) {
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            UserInfo user = DatabaseConnection.GetUserInfoByAPIKey(request.APIKey);
            Console.WriteLine(Utils.SHA256String(request.APIKey));
            if (user == null || user.AdminLevel < AdminLevels.Moderator) {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Not authorized." }.ToJson());
                return;
            }
            
            UserInfo aux = DatabaseConnection.GetUserInfoByUsername(request.User.Username);
            if (aux != null) {
                writer.Write(new BaseResponse { Message = "User already exists." }.ToJson());
                return;
            }

            request.User.APIKey = GenerateAPIKey();

            if (!DatabaseConnection.AddUserInfo(request.User)) {
                writer.Write(new BaseResponse { Message = "Failure adding user to DB." }.ToJson());
                return;
            }

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