using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.API.Response.V2
{
    public struct AccountRecoverStartedResponse
    {
        public string VerificationToken { get; set; }
        public string CharacterID { get; set; }
        public string IntegrityToken { get; set; }
    }
}
