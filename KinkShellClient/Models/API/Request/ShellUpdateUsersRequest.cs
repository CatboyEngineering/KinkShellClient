using System;
using System.Collections.Generic;

namespace KinkShellClient.Models.API.Request
{
    public struct ShellUpdateUsersRequest
    {
        public List<Guid> Users { get; set; }
    }
}
