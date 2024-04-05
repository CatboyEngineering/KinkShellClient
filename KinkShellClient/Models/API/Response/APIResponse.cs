using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.API.Response
{
    public struct APIResponse<T> where T : struct
    {
        public HttpStatusCode StatusCode { get; set; }
        public JObject Response { get; set; }
        public T? Result { get; set; }
    }
}
