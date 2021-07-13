using System;
using Newtonsoft.Json;

using PseudoBanic.Data;
namespace PseudoBanic.Responses
{
    class QueryBasicByDiscordIDResponse : BaseResponse
    {
        public UserInfo User;
    }
}
