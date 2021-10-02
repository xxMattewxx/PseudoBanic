﻿using Newtonsoft.Json;
using System;
namespace PseudoBanic.Requests
{
    class StreamOutputsByAppIDRequest
    {
        public int? MetaID = -1;

        public bool IsValid()
        {
            if (MetaID == null || MetaID.Value < 1) return false;

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
