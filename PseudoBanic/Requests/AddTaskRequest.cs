using System;
using Newtonsoft.Json;
using PseudoBanic.Data;

#pragma warning disable 0649

namespace PseudoBanic.Requests
{
    class AddTaskRequest
    {
        public string APIKey;
        public TaskInfo Task;

        public bool IsValid()
        {
            if (APIKey == null || APIKey.Length != 32) return false;
            if (Task == null || !Task.IsValid()) return false;

            return true;
        }

        public static AddTaskRequest FromJson(string str)
        {
            AddTaskRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<AddTaskRequest>(str);
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
