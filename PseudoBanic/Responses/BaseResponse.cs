using System;
using Newtonsoft.Json;

namespace PseudoBanic.Responses
{
    class BaseResponse
    {
        public bool Success;
        public string Message;

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
