using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic.Data
{
    public class TotalProgress
    {
        public int ID;
        public string Name;
        public int TotalDone;
        public int TotalExisting;
        public bool IsValid()
        {
            if (Name == null || Name.Length < 5) return false;
            if (TotalDone <= 0 || TotalExisting <= 0) return false;

            return true;
        }
    }
}
