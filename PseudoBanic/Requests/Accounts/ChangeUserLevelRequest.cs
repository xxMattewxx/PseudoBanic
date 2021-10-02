using Newtonsoft.Json;
using System;

#pragma warning disable 0649

namespace PseudoBanic.Requests
{
    class ChangeUserLevelRequest
    {
        public int? UserID = Int32.MinValue;
        public string Username = null;
        public Int64 DiscordID = Int32.MinValue;
        public int? Level = Int32.MinValue;

        public bool IsValid()
        {
            if (Level < AdminLevels.Banned || Level > AdminLevels.Developer) return false;
            if (Username != null && Utils.IsValidUsername(Username)) return true;
            if (UserID != Int32.MinValue) return true;
            if (DiscordID != Int32.MinValue) return true;

            return false;
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
