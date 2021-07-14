using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic.Requests
{
    class QueryTotalProgressByTaskIDRequest
    {
        public int TaskID = -1;

        public bool IsValid()
        {
            if (TaskID < 1) return false;

            return true;
        }

        public static QueryTotalProgressByTaskIDRequest FromJson(string str)
        {
            QueryTotalProgressByTaskIDRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<QueryTotalProgressByTaskIDRequest>(str);
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
