using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic.Data.Dto
{
    class MergedHistoricalPoints
    {
        public string Username { get; set; }
        public List<DateTime> Valid { get; set; }
        public List<DateTime> Invalid { get; set; }
    }
}
