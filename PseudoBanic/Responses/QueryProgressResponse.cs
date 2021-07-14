using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic.Responses
{
    class QueryProgressResponse : BaseResponse
    {
        public int ID;
        public string Name;
        public int TotalDone;
        public int TotalGenerated;
    }
}
