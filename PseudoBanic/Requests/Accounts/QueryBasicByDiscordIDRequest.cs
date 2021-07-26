using Newtonsoft.Json;
using System;
namespace PseudoBanic.Requests
{
    class QueryBasicByDiscordIDRequest
    {
        public Int64 DiscordID = Int64.MinValue;

        public bool IsValid()
        {
            if (DiscordID == Int64.MinValue) return false;

            return true;
        }

        public static QueryBasicByDiscordIDRequest FromJson(string str)
        {
            QueryBasicByDiscordIDRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<QueryBasicByDiscordIDRequest>(str);
            }
            catch (Exception) { }

            return ret;
        }

        public string ToJson()
        {
            string ret = "";

            try
            {
                ret = JsonConvert.SerializeObject(this);
            }
            catch (Exception) { }

            return ret;
        }
    }
}
