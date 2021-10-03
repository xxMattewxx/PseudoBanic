using PseudoBanic.Data;
using System.Collections.Generic;

namespace PseudoBanic.Responses
{
    class GetAllHistoricalDataResponse : BaseResponse
    {
        public List<HistoricalLeaderboardPoint> HistoricalLeaderboard;
    }
}
