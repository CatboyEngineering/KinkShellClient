using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.API.Response.V2
{
    public struct AccountAuthenticatedResponse
    {
        public string AuthToken { get; set; }
        public Guid AccountID { get; set; }
        public bool IsVerified { get; set; }
        public string CharacterName { get; set; }
        public string CharacterServer { get; set; }
        public string? LoginToken { get; set; }
        public string? VerificationToken { get; set; }
        public string CharacterID { get; set; }
    }
}
