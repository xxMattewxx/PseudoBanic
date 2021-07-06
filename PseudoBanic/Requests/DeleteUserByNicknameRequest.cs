using System;
using Newtonsoft.Json;

using PseudoBanic.Data;
namespace PseudoBanic.Requests
{
    class DeleteUserByNicknameRequest
    {
        public string APIKey = null;
        public string Username = null;

        public bool IsValid()
        {
            if (APIKey == null || APIKey.Length != 32) return false;
            if (Username == null || Username.Length < 1 || Username.Length > 20) return false;

            return true;
        }

        public static DeleteUserByNicknameRequest FromJson(string str)
        {
            DeleteUserByNicknameRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<DeleteUserByNicknameRequest>(str);
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
