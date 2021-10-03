using PseudoBanic.Data.Types;
using PseudoBanic.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic.Handlers.Leaderboard
{
    class GetAllData
    {
        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            string APIKey = context.Request.Headers.Get("Authorization");

            if (APIKey == null || APIKey.Length != 32)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                writer.Write(new BaseResponse { Message = "Missing or invalid API key." }.ToJson());
                return;
            }

            List<LeaderboardPoint> ret = LeaderboardPoint.GetAllHistoricalData();
            writer.Write(new GetAllDataResponse {
                Success = true,
                Message = "Leaderboard successfully retrieved.",
                Leaderboard = ret
            }.ToJson());
            return;
        }
    }
}
