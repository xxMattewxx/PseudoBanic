using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic.Data
{
    public class UserInfo
    {
        public int UserID;
        public string Username;
        public string APIKey;
        public int AdminLevel;
        public Int64 DiscordID;

        public bool IsValidForRegister()
        {
            if (Username == null || !Utils.IsValidUsername(Username)) return false;
            if (DiscordID == 0) return false;

            return true;
        }
    }
}
