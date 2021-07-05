using System;
using System.Text;
using System.Security.Cryptography;

namespace PseudoBanic
{
    class Utils
    {
        public static string SHA256String(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;

            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
    }
}
