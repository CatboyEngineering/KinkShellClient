using CatboyEngineering.KinkShellClient.Models.API.Response.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.API.Response
{
    public struct AccountListResponse
    {
        public List<AccountInfoResponse> Accounts { get; set; }
    }
}
