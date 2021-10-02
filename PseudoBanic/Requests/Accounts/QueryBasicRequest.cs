using Newtonsoft.Json;
using System;
namespace PseudoBanic.Requests
{
    class QueryBasicRequest
    {
        public int UserID = Int32.MinValue;
        public string Username = null;
        public Int64 DiscordID = Int32.MinValue;

        public bool IsValid()
        {
            if (Username != null && Utils.IsValidUsername(Username)) return true;
            if (UserID != Int32.MinValue) return true;
            if (DiscordID != Int32.MinValue) return true;

            return true;
        }

        public static QueryBasicRequest FromJson(string str)
        {
            QueryBasicRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<QueryBasicRequest>(str);
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
