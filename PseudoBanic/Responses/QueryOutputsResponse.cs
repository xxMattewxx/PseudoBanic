using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using PseudoBanic.Data;
namespace PseudoBanic.Responses
{
    class QueryOutputsResponse : BaseResponse
    {
        public List<String> Outputs;
    }
}
