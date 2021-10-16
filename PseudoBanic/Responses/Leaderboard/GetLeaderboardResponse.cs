using PseudoBanic.Data;
using System.Collections.Generic;

namespace PseudoBanic.Responses
{
    class GetLeaderboardResponse : BaseResponse
    {
        public List<LeaderboardPoint> Leaderboard { get; set; }
    }
}
