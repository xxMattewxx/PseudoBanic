using Newtonsoft.Json;
using System;
namespace PseudoBanic.Requests
{
    class QueryMetadataRequest
    {
        public int ID = -1;

        public bool IsValid()
        {
            if (ID < 1) return false;

            return true;
        }

        public static QueryMetadataRequest FromJson(string str)
        {
            QueryMetadataRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<QueryMetadataRequest>(str);
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
