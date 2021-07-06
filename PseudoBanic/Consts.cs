using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic
{
    public class AdminLevels
    {
        public const int Banned = -2;       //can't access anything using api key
        public const int Blocked = -1;      //can't retrieve or submit results
        public const int None = 0;          //no special permissions. can retrieve tasks and submit results.
        public const int Basic = 1;         //can query statistics from other users, like amount of work done
        public const int Moderator = 2;     //can generate new api keys and ban/unban users
        public const int Administrator = 3; //can delete users and modify access levels
        public const int Manager = 4;       //can pause system services like task retrieval or assimilation.
        public const int Developer = 5;     //all permissions granted. can only be set up on the server.
    }

    public class TaskStatus
    {
        public const int Approval = 0;      //added to the DB but not yet available for processing.
        public const int Distribution = 1;  //task has been approved for distribution and is waiting for contributors.
        public const int Execution = 2;     //task has been distributed in its fullest and results are pending.
        public const int Assimilated = 3;   //task has been fully processed and a quorum was reached.
        public const int QuorumFailure = 4; //task has been fully processed and a quorum was not reached.
    }
}
