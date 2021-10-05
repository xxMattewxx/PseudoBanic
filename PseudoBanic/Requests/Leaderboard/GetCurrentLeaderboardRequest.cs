using Newtonsoft.Json;
using PseudoBanic.Data;
using System;

#pragma warning disable 0649

namespace PseudoBanic.Requests
{
    class GetCurrentLeaderboardRequest
    {
        public long? MetadataID { get; set; }

        public bool IsValid()
        {
            if (MetadataID == null || MetadataID.Value < 1) return false;

            return true;
        }

        public static GetCurrentLeaderboardRequest FromJson(string str)
        {
            GetCurrentLeaderboardRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<GetCurrentLeaderboardRequest>(str);
            }
            catch (Exception) { }

            return ret;
        }
    }
}
