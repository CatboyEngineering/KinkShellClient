using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.API.Request.V2
{
    public struct AccountLoginRequestMigrate
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string CharacterName { get; set; }
        public string CharacterServer { get; set; }
        public string ClientVersionString { get; set; }
    }
}
