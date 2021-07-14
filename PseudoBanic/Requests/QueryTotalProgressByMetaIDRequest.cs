using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic.Requests
{
    class QueryTotalProgressByMetaIDRequest
    {
        public int ID = -1;

        public bool IsValid()
        {
            if (ID < 1) return false;

            return true;
        }

        public static QueryTotalProgressByMetaIDRequest FromJson(string str)
        {
            QueryTotalProgressByMetaIDRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<QueryTotalProgressByMetaIDRequest>(str);
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
