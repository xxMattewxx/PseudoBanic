using PseudoBanic.Data;
using PseudoBanic.Requests;
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
    class GetCurrentLeaderboard
    {
        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            string jsonStr = reader.ReadToEnd();
            GetCurrentLeaderboardRequest request = GetCurrentLeaderboardRequest.FromJson(jsonStr);
            if(request == null || !request.IsValid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            List<LeaderboardPoint> ret = LeaderboardPoint.GetDataForProject(request.MetadataID.Value);
            writer.Write(new GetCurrentLeaderboardResponse
            {
                Success = true,
                Message = "Leaderboard successfully retrieved.",
                Leaderboard = ret
            }.ToJson());
            return;
        }
    }
}
