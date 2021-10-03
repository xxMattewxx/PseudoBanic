using PseudoBanic.Data.Types;
using System.Collections.Generic;

namespace PseudoBanic.Responses
{
    class GetAllDataResponse : BaseResponse
    {
        public List<LeaderboardPoint> Leaderboard;
    }
}
