using Newtonsoft.Json;
using PseudoBanic.Data;
using System;
namespace PseudoBanic.Requests
{
    class RegenByUsernameRequest
    {
        public string Username = null;

        public bool IsValid()
        {
            if (Username == null || Username.Length == 0) return false;

            return true;
        }

        public static RegenByUsernameRequest FromJson(string str)
        {
            RegenByUsernameRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<RegenByUsernameRequest>(str);
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
