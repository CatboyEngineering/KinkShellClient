using System;

namespace CatboyEngineering.KinkShellClient.Models.API.Response
{
    public struct AccountAuthenticatedResponse
    {
        public string AuthToken { get; set; }
        public Guid AccountID { get; set; }
        public string DisplayName { get; set; }
    }
}
