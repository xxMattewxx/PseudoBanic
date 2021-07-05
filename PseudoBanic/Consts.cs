using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic
{
    public class AdminLevels
    {
        public const int Banned = -2; //can't access anything using api key
        public const int Blocked = -1; //can't retrieve or submit results
        public const int None = 0; //no special permissions. can retrieve tasks and submit results.
        public const int Basic = 1; //can query statistics from other users, like amount of work done
        public const int Moderator = 2; //can generate new api keys
        public const int Developer = 5; //all permissions granted. can only be set up on the server.
    }
}
