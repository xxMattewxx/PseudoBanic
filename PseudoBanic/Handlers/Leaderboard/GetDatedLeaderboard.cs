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
    class GetDatedLeaderboard
    {
        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            /*string jsonStr = reader.ReadToEnd();
            GetLeaderboardRequest request = GetLeaderboardRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            List<LeaderboardPoint> ret = LeaderboardPoint.GetHistoricalDataForProject(request.MetadataID.Value, request.SnapshotTime.Value);
            writer.Write(new GetLeaderboardResponse
            {
                Success = true,
                Message = "Leaderboard successfully retrieved.",
                Leaderboard = ret
            }.ToJson());
            return;*/
        }
    }
}
