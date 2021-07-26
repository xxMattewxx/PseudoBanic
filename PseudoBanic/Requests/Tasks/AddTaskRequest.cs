using Newtonsoft.Json;
using PseudoBanic.Data;
using System;

#pragma warning disable 0649

namespace PseudoBanic.Requests
{
    class AddTaskRequest
    {
        public TaskInfo Task;

        public bool IsValid()
        {
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
