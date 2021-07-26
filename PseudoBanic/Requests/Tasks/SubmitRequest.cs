using Newtonsoft.Json;
using System;

namespace PseudoBanic.Requests
{
    class SubmitRequest
    {
        public string APIKey = null;
        public int TaskID = -1;
        public string Results = null;

        public bool IsValid()
        {
            if (APIKey == null || APIKey.Length != 32) return false;
            if (TaskID < 1) return false;

            return true;
        }

        public static SubmitRequest FromJson(string str)
        {
            SubmitRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<SubmitRequest>(str);
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
