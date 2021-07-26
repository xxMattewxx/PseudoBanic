using Newtonsoft.Json;
using System;

#pragma warning disable 0649

namespace PseudoBanic.Requests
{
    class ChangeUserLevelRequest
    {
        public string Username;
        public int Level;

        public bool IsValid()
        {
            if (Level < AdminLevels.Banned || Level > AdminLevels.Developer) return false;
            if (Username == null || !Utils.IsValidUsername(Username)) return false;

            return true;
        }

        public static ChangeUserLevelRequest FromJson(string str)
        {
            ChangeUserLevelRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<ChangeUserLevelRequest>(str);
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
