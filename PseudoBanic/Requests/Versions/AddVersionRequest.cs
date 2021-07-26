using Newtonsoft.Json;
using PseudoBanic.Data;
using System;

#pragma warning disable 0649

namespace PseudoBanic.Requests
{
    class AddVersionRequest
    {
        public ClientVersion Version;

        public bool IsValid()
        {
            if (Version == null || !Version.IsValid()) return false;

            return true;
        }

        public static AddVersionRequest FromJson(string str)
        {
            AddVersionRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<AddVersionRequest>(str);
            }
            catch (Exception e) { Console.WriteLine(e); }

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
