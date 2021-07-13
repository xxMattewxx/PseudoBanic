using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PseudoBanic.Data
{
    public class TaskMeta
    {
        public int ID;
        public string Name;
        public string BinaryURL;
        public bool PassByFile;

        public bool IsValid()
        {
            if (Name == null || Name.Length < 5) return false;
            if (BinaryURL == null || !Uri.IsWellFormedUriString(BinaryURL, UriKind.Absolute)) return false;

            return true;
        }
    }
}
