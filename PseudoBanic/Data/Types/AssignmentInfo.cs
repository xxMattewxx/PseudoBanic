using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic.Data
{
    class AssignmentInfo
    {
        public int AssignmentID { get; set; }
        public int TaskID { get; set; }
        public TaskInfo Task { get; set; }
        public int UserID { get; set; }
        public UserInfo User { get; set; }
        public DateTime Deadline { get; set; }
        public int State { get; set; }
        public string Output { get; set; }
    }
}
