using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic.Data
{
    public class TaskInfo
    {
        public int ID;
        public int Status;
        public int QuorumLeft;
        public string InputData;
        public string Consensus;
        public TaskMeta MetaData;

        public bool IsValid()
        {
            if (MetaData == null) return false;

            return true;
        }
    }
}