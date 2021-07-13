using System;
using Newtonsoft.Json;

using PseudoBanic.Data;
namespace PseudoBanic.Requests
{
    class StreamOutputsByAppIDRequest
    {
        public int MetaID = -1;

        public bool IsValid()
        {
            if (MetaID < 1) return false;

            return true;
        }

        public static StreamOutputsByAppIDRequest FromJson(string str)
        {
            StreamOutputsByAppIDRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<StreamOutputsByAppIDRequest>(str);
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
