using PseudoBanic.Data;
using System.Collections.Generic;

namespace PseudoBanic.Responses
{
    class ListProjectsResponse : BaseResponse
    {
        public List<TaskMeta> Projects { get; set; }
    }
}
