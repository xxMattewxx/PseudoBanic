using Newtonsoft.Json;
using PseudoBanic.Data;
using System;

#pragma warning disable 0649

namespace PseudoBanic.Requests
{
    class AddTaskMetadataRequest
    {
        public TaskMeta Metadata;

        public bool IsValid()
        {
            if (Metadata == null || !Metadata.IsValid()) return false;

            return true;
        }

        public static AddTaskMetadataRequest FromJson(string str)
        {
            AddTaskMetadataRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<AddTaskMetadataRequest>(str);
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
