using Newtonsoft.Json;
using System;
namespace PseudoBanic.Requests
{
    class DeleteUserByDiscordIDRequest
    {
        public Int64 DiscordID = Int64.MinValue;

        public bool IsValid()
        {
            if (DiscordID == Int64.MinValue) return false;

            return true;
        }

        public static DeleteUserByDiscordIDRequest FromJson(string str)
        {
            DeleteUserByDiscordIDRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<DeleteUserByDiscordIDRequest>(str);
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
