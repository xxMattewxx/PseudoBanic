using Newtonsoft.Json;
using PseudoBanic.Data;
using System;

#pragma warning disable 0649

namespace PseudoBanic.Requests
{
    class GetHistoricalLeaderboardByProjectIDRequest
    {
        public long? MetadataID { get; set; }

        public bool IsValid()
        {
            if (MetadataID == null || MetadataID.Value < 1) return false;

            return true;
        }

        public static GetHistoricalLeaderboardByProjectIDRequest FromJson(string str)
        {
            GetHistoricalLeaderboardByProjectIDRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<GetHistoricalLeaderboardByProjectIDRequest>(str);
            }
            catch (Exception) { }

            return ret;
        }
    }
}
