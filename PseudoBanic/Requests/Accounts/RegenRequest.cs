using Newtonsoft.Json;
using PseudoBanic.Data;
using System;
namespace PseudoBanic.Requests
{
    class RegenRequest
    {
        public Int64 DiscordID = Int64.MinValue;
        public int UserID = Int32.MinValue;
        public string Username = null;

        public bool IsValid()
        {
            if (DiscordID != Int64.MinValue) return true;
            if (UserID != Int32.MinValue) return true;
            if (Username != null && Username.Length != 0) return true;

            return false;
        }

        public static RegenRequest FromJson(string str)
        {
            RegenRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<RegenRequest>(str);
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
