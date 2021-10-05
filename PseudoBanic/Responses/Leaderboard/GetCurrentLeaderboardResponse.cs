using PseudoBanic.Data;
using System.Collections.Generic;

namespace PseudoBanic.Responses
{
    class GetCurrentLeaderboardResponse : BaseResponse
    {
        public List<LeaderboardPoint> Leaderboard { get; set; }
    }
}
