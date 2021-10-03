using PseudoBanic.Data;
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

            List<HistoricalLeaderboardPoint> ret = HistoricalLeaderboardPoint.GetAllData();
            writer.Write(new GetAllHistoricalDataResponse {
                Success = true,
                Message = "Leaderboard successfully retrieved.",
                HistoricalLeaderboard = ret
            }.ToJson());
            return;
        }
    }
}
