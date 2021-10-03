﻿using Newtonsoft.Json;
using System;
namespace PseudoBanic.Requests
{
    class DeleteUserRequest
    {
        public Int64 DiscordID = Int64.MinValue;
        public int UserID = Int32.MinValue;
        public string Username = null;

        public bool IsValid()
        {
            if (UserID > 0) return true;
            if (DiscordID != Int64.MinValue) return true;
            if (Username != null && Username.Length > 0) return true;

            return false;
        }

        public static DeleteUserRequest FromJson(string str)
        {
            DeleteUserRequest ret = null;

            try
            {
                ret = JsonConvert.DeserializeObject<DeleteUserRequest>(str);
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