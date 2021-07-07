using System;
using Newtonsoft.Json;

using PseudoBanic.Data;
namespace PseudoBanic.Responses
{
    class QueryMetadataResponse : BaseResponse
    {
        public TaskMeta Metadata;
    }
}
