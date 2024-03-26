using System;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient.Models.API.Request
{
    public struct ShellUpdateUsersRequest
    {
        public List<Guid> Users { get; set; }
    }
}
