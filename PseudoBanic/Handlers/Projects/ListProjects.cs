using System.IO;
using System.Net;
using PseudoBanic.Data;
using PseudoBanic.Responses;
using System.Collections.Generic;

namespace PseudoBanic.Handlers.Projects
{
    class ListProjects
    {
        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            List<TaskMeta> ret = TaskMeta.GetAll();
            writer.Write(new ListProjectsResponse
            {
                Success = true,
                Message = "Project list retrieved.",
                Projects = ret
            }.ToJson());
            return;
        }
    }
}
