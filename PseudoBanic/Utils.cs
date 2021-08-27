using System;
using System.Security.Cryptography;
using System.Text;

namespace PseudoBanic
{
    class Utils
    {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        public static string GenerateAPIKey()
        {
            byte[] buffer = new byte[16];
            rng.GetNonZeroBytes(buffer);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string ValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-";
        public static bool IsValidUsername(string username)
        {
            if (username == null || username.StartsWith("_")) return false;
            if (username.Length < 1 || username.Length > 20) return false;

            for (int i = 0; i < username.Length; i++)
                if (!ValidCharacters.Contains(username[i]))
                    return false;

            return true;
        }


        public static Cache<string, Tuple<bool, string>> SHA256Cache = new Cache<string, Tuple<bool, string>>();
        public static string SHA256String(string str)
        {
            var cache = SHA256Cache.Get(str);
            if (cache != null && cache.Item1)
            {
                SHA256Cache.Refresh(str);
                return cache.Item2;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(str);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;

            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }

            SHA256Cache.Store(str, Tuple.Create(true, hashString), TimeSpan.FromHours(2));
            return hashString;
        }
    }
}
