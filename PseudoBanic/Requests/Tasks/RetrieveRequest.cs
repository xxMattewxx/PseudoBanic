using Newtonsoft.Json;
using System;

namespace PseudoBanic.Requests
{
    class RetrieveRequest
    {
        public string APIKey = null;

        public bool IsValid()
        {
            if (APIKey == null || APIKey.Length != 32) return false;

            return true;
        }

        public static RetrieveRequest FromJson(string str)
        {
            RetrieveRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<RetrieveRequest>(str);
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
