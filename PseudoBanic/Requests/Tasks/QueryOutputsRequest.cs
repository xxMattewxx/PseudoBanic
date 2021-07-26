using Newtonsoft.Json;
using System;
namespace PseudoBanic.Requests
{
    class QueryOutputsRequest
    {
        public int TaskID = -1;

        public bool IsValid()
        {
            if (TaskID < 1) return false;

            return true;
        }

        public static QueryOutputsRequest FromJson(string str)
        {
            QueryOutputsRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<QueryOutputsRequest>(str);
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
