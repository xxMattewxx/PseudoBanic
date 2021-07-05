using System;
using Newtonsoft.Json;

namespace PseudoBanic.Requests
{
    class RetrieveRequest
    {
        public string Token = null;

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
