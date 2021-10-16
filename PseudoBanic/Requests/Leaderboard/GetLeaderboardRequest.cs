using Newtonsoft.Json;
using PseudoBanic.Data;
using System;

#pragma warning disable 0649

namespace PseudoBanic.Requests
{
    class GetLeaderboardRequest
    {
        public long? MetadataID { get; set; }
        public DateTime? SnapshotTime { get; set; }

        public bool IsValid()
        {
            if (MetadataID == null || MetadataID.Value < 1) return false;
            if (SnapshotTime == null) return false;

            return true;
        }

        public static GetLeaderboardRequest FromJson(string str)
        {
            GetLeaderboardRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<GetLeaderboardRequest>(str);
            }
            catch (Exception) { }

            return ret;
        }
    }
}
