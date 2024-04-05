using System;
using System.Collections.Generic;
using CatboyEngineering.KinkShellClient.Models.Shell;

namespace CatboyEngineering.KinkShellClient.Models.API.Request
{
    public struct ShellAdjustUsersRequest
    {
        public List<ShellNewUser> Users { get; set; }
    }
}
