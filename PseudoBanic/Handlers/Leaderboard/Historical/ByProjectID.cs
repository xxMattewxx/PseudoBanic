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
using Microsoft.EntityFrameworkCore;

namespace PseudoBanic.Handlers.Leaderboard.Historical
{
    class ByProjectID
    {
        static void StreamData(StreamWriter writer, long projectID)
        {
            using var dbContext = new HistoricalLeaderboardDbContext();
            foreach(var aux in dbContext.HistoricalLeaderboard
                .Where(x => x.MetadataID == projectID))
            {
                writer.WriteLine("{0} {1} {2} {3} {4}", aux.UserID, aux.Points, aux.ValidatedPoints, aux.InvalidatedPoints, Utils.ConvertToUnixTimestamp(aux.SnapshotTime));
            }
            writer.Flush();
        }

        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            string jsonStr = reader.ReadToEnd();

            GetHistoricalLeaderboardByProjectIDRequest request = GetHistoricalLeaderboardByProjectIDRequest.FromJson(jsonStr);
            if (request == null || !request.IsValid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                writer.Write(new BaseResponse { Message = "Invalid request." }.ToJson());
                return;
            }

            StreamData(writer, request.MetadataID.Value);
        }
    }
}
