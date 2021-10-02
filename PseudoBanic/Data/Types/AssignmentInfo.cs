using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic.Data
{
    class AssignmentInfo
    {
        public int AssignmentID;
        public int TaskID;
        public TaskInfo Task;
        public int UserID;
        public UserInfo User;
        public DateTime Deadline;
        public int State;
        public string Output;
    }
}
