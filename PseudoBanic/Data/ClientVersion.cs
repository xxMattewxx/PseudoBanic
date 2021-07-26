using System;
namespace PseudoBanic.Data
{
    public class ClientVersion
    {
        public string Codename;
        public int VersionNumber;
        public string BinaryURL;
        public string FileHash;

        public bool IsValid()
        {
            if (Codename == null || Codename.Length < 1) return false;
            if (BinaryURL == null || !Uri.IsWellFormedUriString(BinaryURL, UriKind.Absolute)) return false;
            if (FileHash == null || FileHash.Length != 32) return false;

            return true;
        }
    }
}
