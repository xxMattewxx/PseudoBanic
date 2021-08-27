using Newtonsoft.Json;
using PseudoBanic.Data;
using System;
namespace PseudoBanic.Requests
{
    class RegenByDiscordIDRequest
    {
        public Int64 DiscordID = 0;

        public bool IsValid()
        {
            if (DiscordID == 0) return false;

            return true;
        }

        public static RegenByDiscordIDRequest FromJson(string str)
        {
            RegenByDiscordIDRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<RegenByDiscordIDRequest>(str);
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
