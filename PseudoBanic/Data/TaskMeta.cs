using System;

namespace PseudoBanic.Data
{
    public class TaskMeta
    {
        public int ID;
        public string Name;
        public string BinaryURL;
        public string FileHash;
        public bool PassByFile;

        public bool IsValid()
        {
            if (Name == null || Name.Length < 5) return false;
            if (FileHash == null || FileHash.Length != 32) return false;
            if (BinaryURL == null || !Uri.IsWellFormedUriString(BinaryURL, UriKind.Absolute)) return false;

            return true;
        }
    }
}
