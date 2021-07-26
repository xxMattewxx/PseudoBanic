using Newtonsoft.Json;
using System;
namespace PseudoBanic.Requests
{
    class DeleteUserByNicknameRequest
    {
        public string Username = null;

        public bool IsValid()
        {
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
