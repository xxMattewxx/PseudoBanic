using System;
using Newtonsoft.Json;

using PseudoBanic.Data;
namespace PseudoBanic.Requests
{
    class RegisterRequest
    {
        public string APIKey = null;
        public UserInfo User = null;

        public bool IsValid()
        {
            Console.WriteLine(JsonConvert.SerializeObject(this));
            if (APIKey == null || APIKey.Length != 32) return false;
            if (User == null || !User.IsValidForRegister()) return false;

            return true;
        }

        public static RegisterRequest FromJson(string str)
        {
            RegisterRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<RegisterRequest>(str);
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
