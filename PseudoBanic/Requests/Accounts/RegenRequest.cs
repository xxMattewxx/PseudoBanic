using Newtonsoft.Json;
using PseudoBanic.Data;
using System;
namespace PseudoBanic.Requests
{
    class RegenRequest
    {
        public string Username = null;

        public bool IsValid()
        {
            if (Username == null || Username.Length == 0) return false;

            return true;
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
