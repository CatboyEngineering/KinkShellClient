using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.API.Request.V2
{
    public struct AccountLoginRequest
    {
        public string CharacterName { get; set; }
        public string CharacterServer { get; set; }
        public string LoginToken { get; set; }
        public string ClientVersion { get; set; }
    }
}
